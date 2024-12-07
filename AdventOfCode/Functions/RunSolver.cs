using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using AdventOfCode.Core;
using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class RunSolver {
    private readonly IConsole console;

    private readonly SolverFactory factory;
    private readonly bool isTracing;

    public RunSolver(IConsole console, SolverFactory factory, bool isTracing = false) {
        this.console = console;
        this.factory = factory;
        this.isTracing = isTracing;
    }

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
        if (isTracing) {
            var baseBaseType = solver.GetType().BaseType?.BaseType;
            var outputProperty = baseBaseType?.GetField("<Output>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            var traceProperty = baseBaseType?.GetField("<Trace>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);

            if (outputProperty == null || traceProperty == null) {
                console.WriteLine($"# Tracing not available");
            }
            else {
                console.WriteLine($"# Enabling tracing");
                traceProperty.SetValue(solver, outputProperty.GetValue(solver));
            }
        }
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