using AdventOfCode.Shared;

namespace AdventOfCode.Infrastructure;

internal static class InputReader {
    public static bool TryReadInput(PuzzleArguments puzzle, out string? input) {
        if (!puzzle.IsValid) {
            input = string.Empty;
            return false;
        }

        var path = Path.GetFullPath(Input.PathFor(puzzle.Event, puzzle.Day));

        if (!Input.TryRead(puzzle.Event, puzzle.Day, out input)) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"# Trying input {path}");
            Console.ResetColor();
            input = string.Empty;
            return false;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"# Using input {Path.GetFileName(path)} ({input!.Length} characters)");
        Console.ResetColor();

        return true;
    }
}