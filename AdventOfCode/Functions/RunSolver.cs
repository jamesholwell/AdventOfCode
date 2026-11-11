using System.Diagnostics;
using AdventOfCode.Infrastructure;
using AdventOfCode.Shared;
using Xunit.Abstractions;

namespace AdventOfCode.Functions;

internal class RunSolver(SolverFactory factory, bool isTracing = false) {
    public void Execute(PuzzleArguments puzzle) {
        if (!InputReader.TryReadInput(puzzle, out var input)) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Unable to read puzzle input");
            Console.ResetColor();

            Console.WriteLine(string.Empty);
            return;
        }

        // resolve solver
        ISolver? solver;

        try {
            ITestOutputHelper output = isTracing ? new TracingConsoleOutputHelper() : new ConsoleOutputHelper();
            solver = factory.Create(puzzle.Day, puzzle.Event, input!, puzzle.Suffix, output);
        }
        catch (AmbiguousSolverException e) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e.Message);

            foreach (var candidate in e.Candidates) {
                Console.Write("  ");
                Console.WriteLine(candidate);
            }

            Console.ResetColor();

            return;
        }

        if (solver == null) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("No solvers found");
            Console.ResetColor();

            Console.WriteLine(string.Empty);
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"# Using solver {solver.GetType().FullName}");
        Console.ResetColor();

        var sw = new Stopwatch();
        sw.Start();

        var solution = puzzle.Part switch {
            Parts.Part2 => solver.SolvePartTwo(),
            _ => solver.SolvePartOne(),
        };

        sw.Stop();

        Console.ForegroundColor = ConsoleColor.DarkGray;

        Console.WriteLine(sw.ElapsedMilliseconds > 0
            ? $"# Runtime {sw.ElapsedMilliseconds}ms"
            : $"# Runtime {sw.ElapsedTicks} ticks");
        Console.ResetColor();

        Console.WriteLine(string.Empty);
        Console.WriteLine(solution);
    }
}