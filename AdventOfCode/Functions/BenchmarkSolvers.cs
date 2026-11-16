using System.Diagnostics;
using AdventOfCode.Services;
using AdventOfCode.Shared;

namespace AdventOfCode.Functions;

internal class BenchmarkSolvers(SolverFactory factory) {
    public void Execute(PuzzleArguments puzzle) {
        if (!Input.TryGetContents(puzzle.Event, puzzle.Day, out var input)) {
            Console.WriteError($"Input not found (using: {Input.PathFor(puzzle.Event, puzzle.Day)})");
            return;
        }

        var solvers = factory.CreateAll(puzzle.Event, puzzle.Day, input!).ToArray();
        if (!solvers.Any()) {
            Console.WriteError("No solvers found");
            return;
        }

        Console.WriteMessage($"# Found {solvers.Length} solvers");
        Console.WriteLine();
        
        Console.WriteLine("|--------------------------------|---------------------------|----------------------|");
        Console.WriteLine($"| {"Solver",-30} | {"Solution",-25} | {"Runtime",-20} |");
        Console.WriteLine("|--------------------------------|---------------------------|----------------------|");

        // preparations
        int i, j, c = solvers.Length;
        var durations = new TimeSpan[c];
        var executions = new int[c];
        var solutions = new string[c];
        var isFast = new bool[c];

        // run each solver in turn
        for (i = 0; i < c; ++i) {
            var sw = new Stopwatch();
            var runs = 1;
            sw.Start();

            // solve and record answer
            var solver = solvers[i].Value;
            solutions[i] = solver.Solve(puzzle);
            
            // keep solving in a loop to smooth the average
            while (sw.ElapsedMilliseconds < 500 && ++runs < 10_000)
                solver.Solve(puzzle);

            sw.Stop();

            // if very fast, throw these results away and signal the second loop
            if (runs == 10_000) {
                isFast[i] = true;
                continue;
            }
            
            executions[i] = runs;
            durations[i] = sw.Elapsed;
        }

        // now rerun fast solvers in a double loop to smooth out scheduling noise
        var outerSw = new Stopwatch();
        var innerSw = new Stopwatch();
        
        var totalRuns = 0;
        var offset = 0;
        outerSw.Start();
        
        while (outerSw.ElapsedMilliseconds < 5_000 && totalRuns < 1_000_000) {
            ++offset;
            
            for (j = 0; j < c; ++j) {
                // rotate the execution ordering each outer loop
                i = (j + offset) % c; 
                
                if (!isFast[i]) continue;
                var solver = solvers[i].Value;
                
                innerSw.Restart();
                
                var runs = 0;
                do {
                    solver.Solve(puzzle);
                } while (++runs < 10_000);

                innerSw.Stop();

                totalRuns += runs;
                executions[i] += runs;
                durations[i] += innerSw.Elapsed;
            }
        }

        // export the results
        for (i = 0; i < c; ++i) {
            var averageMs = durations[i].TotalMilliseconds / executions[i];
            var runtime = averageMs switch {
                > 5000 => $"{averageMs / 1000:N1}s",
                > 1000 => $"{averageMs / 1000:N2}s",
                > 50 => $"{averageMs:N0}ms",
                _ => $"{durations[i].Ticks / (double) executions[i]:N4} ticks"
            };

            Console.WriteLine($"| {solvers[i].Key,-30} | {solutions[i],-25} | {runtime,20} |");
        }

        Console.WriteLine("|--------------------------------|---------------------------|----------------------|");
        Console.WriteLine();
    }
}