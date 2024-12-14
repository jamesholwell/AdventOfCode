using System.CommandLine;
using System.Diagnostics;
using AdventOfCode.Core;
using AdventOfCode.Core.Output;
using AdventOfCode.Infrastructure;
using Xunit.Abstractions;

namespace AdventOfCode.Functions;

internal class RunSolver(IConsole console, SolverFactory factory, bool isTracing = false) {
    public void Execute(PuzzleSpecification puzzle) {
        if (!InputReader.TryReadInput(puzzle, out var input, console)) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine("Unable to read puzzle input");
            Console.ResetColor();

            console.WriteLine(string.Empty);
            return;
        }

        // resolve solver
        ISolver? solver;

        try {
            ITestOutputHelper output = isTracing ? new TracingConsoleOutputHelper() : new ConsoleOutputHelper();
            solver = factory.Create(puzzle.Day, puzzle.Event, input, puzzle.SolverHint, output);
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

            console.WriteLine(string.Empty);
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"# Using solver {solver.GetType().FullName}");
        Console.ResetColor();

        var sw = new Stopwatch();
        sw.Start();

        var solution = puzzle.IsPartTwo switch {
            false => solver.SolvePartOne(),
            true => solver.SolvePartTwo()
        };

        sw.Stop();

        Console.ForegroundColor = ConsoleColor.DarkGray;

        console.WriteLine(sw.ElapsedMilliseconds > 0
            ? $"# Runtime {sw.ElapsedMilliseconds}ms"
            : $"# Runtime {sw.ElapsedTicks} ticks");
        Console.ResetColor();

        console.WriteLine(string.Empty);
        console.WriteLine(solution);
    }
}