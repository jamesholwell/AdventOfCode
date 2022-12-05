using AdventOfCode.Core;
using System.CommandLine;
using System.Diagnostics;

namespace AdventOfCode.Functions;

internal class BenchmarkSolvers {
    private readonly IConsole console;

    private readonly SolverFactory factory;

    public BenchmarkSolvers(IConsole console, SolverFactory factory) {
        this.console = console;
        this.factory = factory;
    }

    public void Execute(PuzzleSpecification puzzle) {
        if (!Utilities.TryReadInput(puzzle, out var input, console))
            return;

        var solvers = factory.CreateAll(puzzle.Day, puzzle.Event, input);

        if (!solvers.Any()) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine("No solvers found");
            Console.ResetColor();

            console.WriteLine(string.Empty);

            return;
        }

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

            var solution = puzzle.IsPartTwo switch {
                false => solver.SolvePartOne(),
                true => solver.SolvePartTwo()
            };
            sw.Stop();

            if (sw.ElapsedMilliseconds < 1000) {
                // do warmup for fast solvers
                while (sw.Elapsed.Seconds < 5 && ++runs < 1000)
                    solution = puzzle.IsPartTwo switch {
                        false => solver.SolvePartOne(),
                        true => solver.SolvePartTwo()
                    };

                runs = 0;
                sw.Restart();

                while (sw.Elapsed.Seconds < 10 && ++runs < 10000)
                    solution = puzzle.IsPartTwo switch {
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
}