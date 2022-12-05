using System.CommandLine;
using System.Text.RegularExpressions;
using AdventOfCode.Core;

namespace AdventOfCode;

internal static class Utilities {
    private static readonly Regex FilenameSanitizer = new("[^a-z0-9]", RegexOptions.IgnoreCase);

    public static bool TryReadInput(PuzzleSpecification puzzle, out string input, IConsole console) {
        var path =
            $"./inputs/{FilenameSanitizer.Replace(puzzle.Event, string.Empty)}/{FilenameSanitizer.Replace(puzzle.Day, string.Empty)}.txt";

        if (!File.Exists(path)) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine($"Path not found: {path}");
            Console.ResetColor();
            console.WriteLine(string.Empty);

            input = string.Empty;

            return false;
        }

        input = File.ReadAllText(path);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"# Using input {Path.GetFileName(path)} ({input.Length} characters)");
        Console.ResetColor();

        return true;
    }
}