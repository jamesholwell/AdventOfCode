using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using AdventOfCode.Core;
using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class CreateSolver {
    private readonly IConsole console;

    public CreateSolver(IConsole console) {
        this.console = console;
    }

    public void Execute(PuzzleSpecification puzzle) {
        var sanitizedEvent = InputReader.FilenameSanitizer.Replace(puzzle.Event, string.Empty);
        var sanitizedDay = InputReader.FilenameSanitizer.Replace(puzzle.Day, string.Empty).ToLowerInvariant();
        sanitizedDay = new string(new[] { sanitizedDay[0] }).ToUpperInvariant() + new string(sanitizedDay.Skip(1).ToArray());
        var path = $"./AdventOfCode.Today/{sanitizedEvent}/{sanitizedDay}.cs";
        
        if (File.Exists(path)) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine($"{path} already exists");
            Console.ResetColor();

            console.WriteLine(string.Empty);
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"Initialized new solver {path}");
        Console.ResetColor();
    }
}