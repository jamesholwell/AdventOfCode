using System.CommandLine;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Core;

namespace AdventOfCode;

// TODO: replace colours with System.CommandLine.Rendering
internal class SolverCli {
    private readonly IConsole console;

    private readonly SolverFactory factory;

    private readonly Regex filenameSanitizer = new("[^a-z0-9]", RegexOptions.IgnoreCase);

    public SolverCli(IConsole console, SolverFactory factory) {
        this.console = console;
        this.factory = factory;
    }

    public void Solve(PuzzleSpecification puzzle) {
        if (!TryReadInput(puzzle, out var input)) return;

        // resolve solver
        ISolver? solver;
        try {
            solver = factory.Create(puzzle.Day, puzzle.Event, input, puzzle.SolverHint);
        }
        catch (AmbiguousSolverException e) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine(e.Message);
            foreach (var candidate in e.Candidates) {
                console.Write("  ");
                console.WriteLine(candidate);
            }
            Console.ResetColor();

            return;
        }

        if (solver == null) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine("No solvers found");
            Console.ResetColor();

            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"# Using solver {solver.GetType().FullName}");
        Console.ResetColor();

        var sw = new Stopwatch();
        sw.Start();

        var solution = puzzle.IsPart2 switch {
            false => solver.SolvePartOne(),
            true => solver.SolvePartTwo()
        };

        sw.Stop();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        if (sw.ElapsedMilliseconds > 0)
            console.WriteLine($"# Runtime {sw.ElapsedMilliseconds}ms");
        else
            console.WriteLine($"# Runtime {sw.ElapsedTicks} ticks");
        Console.ResetColor();

        console.WriteLine(string.Empty);
        console.WriteLine(solution.ToString());
    }

    public void Benchmark(PuzzleSpecification puzzle) {
        if (!TryReadInput(puzzle, out var input)) return;

        var solvers = factory.CreateAll(puzzle.Day, puzzle.Event, input);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"# Found {solvers.Count} solvers");
        Console.ResetColor();

        console.WriteLine(string.Empty);
        console.WriteLine("|--------------------------------|---------------------------|----------------------|");
        console.WriteLine($"| {"Solver",-30} | {"Solution",-25} | {"Runtime",-20} |");
        console.WriteLine("|--------------------------------|---------------------------|----------------------|");

        foreach (var pair in solvers) {
            var solverName = pair.Key;
            var solver = pair.Value;

            var sw = new Stopwatch();
            var runs = 1;

            sw.Start();
            var solution = puzzle.IsPart2 switch {
                false => solver.SolvePartOne(),
                true => solver.SolvePartTwo()
            };
            sw.Stop();

            if (sw.ElapsedMilliseconds < 1000) {
                // do warmup for fast solvers
                while (sw.Elapsed.Seconds < 5 && ++runs < 1000)
                    solution = puzzle.IsPart2 switch {
                        false => solver.SolvePartOne(),
                        true => solver.SolvePartTwo()
                    };

                runs = 0;
                sw.Restart();

                while (sw.Elapsed.Seconds < 10 && ++runs < 10000)
                    solution = puzzle.IsPart2 switch {
                        false => solver.SolvePartOne(),
                        true => solver.SolvePartTwo()
                    };

                sw.Stop();
            }

            var averageMs = sw.ElapsedMilliseconds / (double) runs;
            var runtime = averageMs switch {
                > 5000 => $"{averageMs / 1000:N1}s",
                > 1000 => $"{averageMs / 1000:N2}s",
                > 50 => $"{averageMs:N0}ms",
                _ => $"{sw.ElapsedTicks / (double) runs} ticks"
            };

            console.WriteLine($"| {solverName,-30} | {solution,-25} | {runtime,20} |");
        }

        console.WriteLine("|--------------------------------|---------------------------|----------------------|");
        console.WriteLine(string.Empty);
    }

    public void ListSolvers() {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine("# The following solvers are available:");
        console.WriteLine("#   ");
        foreach (var candidate in factory.List().OrderBy(k => k)) {
            console.Write("#   ");
            console.WriteLine(candidate);
        }
        
        console.WriteLine(string.Empty);
        Console.ResetColor();
    }

    private bool TryReadInput(PuzzleSpecification puzzle, out string input) {
        var path = $"./inputs/{filenameSanitizer.Replace(puzzle.Event, string.Empty)}/{filenameSanitizer.Replace(puzzle.Day, string.Empty)}.txt";

        if (!File.Exists(path)) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine($"Path not found: {path}");
            Console.ResetColor();

            input = string.Empty;
            return false;
        }

        input = File.ReadAllText(path);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"# Using input {Path.GetFileName(path)} ({input.Length} characters)");
        Console.ResetColor();

        return true;
    }
}