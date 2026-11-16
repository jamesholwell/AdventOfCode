using System.Diagnostics;
using AdventOfCode.Services;
using AdventOfCode.Shared;
using Xunit.Abstractions;

namespace AdventOfCode.Functions;

internal class RunSolver(SolverFactory factory, bool isTracing = false) {
    public void Execute(PuzzleArguments puzzle) {
        // resolve input
        var path = Input.PathFor(puzzle.Event, puzzle.Day);
        if (!Input.TryGetContents(puzzle.Event, puzzle.Day, out var input)) {
            Console.WriteError($"Unable to read puzzle input (using: {path})");
            return;
        }
        
        Console.WriteMessage($"# Using input {path} ({input!.Length} characters)");

        // resolve solver
        ITestOutputHelper output = isTracing ? new TracingConsoleOutputHelper() : new ConsoleOutputHelper(); 
        var solver = factory.Create(puzzle.Event, puzzle.Day, puzzle.Suffix, input, output);

        if (solver == null) {
            Console.WriteError("No solvers found");
            return;
        }

        Console.WriteMessage($"# Using solver {solver.GetType().FullName}");

        var sw = new Stopwatch();
        sw.Start();

        var solution = solver.Solve(puzzle);

        sw.Stop();

        Console.WriteMessage(sw.ElapsedMilliseconds > 0
            ? $"# Runtime {sw.ElapsedMilliseconds}ms"
            : $"# Runtime {sw.ElapsedTicks} ticks");
        
        Console.WriteLine();
        Console.WriteLine(solution);
    }
}