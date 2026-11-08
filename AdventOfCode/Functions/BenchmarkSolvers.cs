using System.Diagnostics;
using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class BenchmarkSolvers(SolverFactory factory) {
    public void Execute(PuzzleArguments puzzle) {
        if (!InputReader.TryReadInput(puzzle, out var input))
            return;

        var solvers = factory.CreateAll(puzzle.Day, puzzle.Event, input!);

        if (!solvers.Any()) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("No solvers found");
            Console.ResetColor();

            Console.WriteLine(string.Empty);

            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"# Found {solvers.Count} solvers");
        Console.ResetColor();
        Console.WriteLine(string.Empty);
        Console.WriteLine("|--------------------------------|---------------------------|----------------------|");
        Console.WriteLine($"| {"Solver",-30} | {"Solution",-25} | {"Runtime",-20} |");
        Console.WriteLine("|--------------------------------|---------------------------|----------------------|");

        foreach (var pair in solvers) {
            var solverName = pair.Key;
            var solver = pair.Value;

            var sw = new Stopwatch();
            var runs = 1;

            sw.Start();

            var solution = puzzle.Part switch {
                Parts.Part2 => solver.SolvePartTwo(),
                _ => solver.SolvePartOne(),
            };
            sw.Stop();

            if (sw.ElapsedMilliseconds < 1000) {
                // do warmup for fast solvers
                while (sw.Elapsed.Seconds < 5 && ++runs < 1000)
                    solution = puzzle.Part switch {
                        Parts.Part2 => solver.SolvePartTwo(),
                        _ => solver.SolvePartOne(),
                    };

                runs = 0;
                sw.Restart();

                while (sw.Elapsed.Seconds < 10 && ++runs < 10000)
                    solution = puzzle.Part switch {
                        Parts.Part2 => solver.SolvePartTwo(),
                        _ => solver.SolvePartOne(),
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

            Console.WriteLine($"| {solverName,-30} | {solution,-25} | {runtime,20} |");
        }

        Console.WriteLine("|--------------------------------|---------------------------|----------------------|");
        Console.WriteLine(string.Empty);
    }
}