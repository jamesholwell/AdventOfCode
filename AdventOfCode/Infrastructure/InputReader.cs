using System.CommandLine;
using System.Text.RegularExpressions;
using AdventOfCode.Core;

namespace AdventOfCode.Infrastructure;

internal static class InputReader {
    internal static readonly Regex FilenameSanitizer = new("[^a-z0-9]", RegexOptions.IgnoreCase);

    public static bool TryReadInput(PuzzleSpecification puzzle, out string input, IConsole console) {
        if (string.IsNullOrWhiteSpace(puzzle.Event) || string.IsNullOrWhiteSpace(puzzle.Day)) {
            input = string.Empty;
            return false;
        }

        var path =
            $"./inputs/{FilenameSanitizer.Replace(puzzle.Event, string.Empty)}/{FilenameSanitizer.Replace(puzzle.Day, string.Empty)}.txt";

        if (!File.Exists(path)) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            console.WriteLine($"# Trying input {Path.GetFullPath(path)}");
            Console.ResetColor();
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