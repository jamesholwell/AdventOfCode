using AdventOfCode.Shared;

namespace AdventOfCode;

internal static class Extensions {
    extension(Console) {
        internal static void WriteMessage(string message) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        internal static void WriteError(string errorMessage) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(errorMessage);
            Console.ResetColor();

            Console.WriteLine(string.Empty);
        }
    }

    internal static string Solve(this ISolver solver, PuzzleArguments puzzle) =>
        puzzle.Part switch {
            PuzzleArguments.Parts.Part2 => solver.SolvePartTwo(),
            PuzzleArguments.Parts.Part1 => solver.SolvePartOne(),
            _ => throw new ArgumentOutOfRangeException(nameof(puzzle.Part))
        };
}