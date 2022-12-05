using System.CommandLine;
using System.Diagnostics;
using AdventOfCode.Core;

namespace AdventOfCode.Functions;

internal class RunSolver {
    private readonly IConsole console;

    private readonly SolverFactory factory;

    public RunSolver(IConsole console, SolverFactory factory) {
        this.console = console;
        this.factory = factory;
    }

    public void Execute(PuzzleSpecification puzzle) {
        if (!Utilities.TryReadInput(puzzle, out var input, console)) return;

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