using System.CommandLine;
using AdventOfCode.Core;
using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class StageSolver(IConsole console) {
    public void Execute(PuzzleSpecification puzzle, bool isForced) {
        // sanitize and beautify the arguments
        var sanitizedEvent = InputReader.FilenameSanitizer.Replace(puzzle.Event, string.Empty);
        var sanitizedDay = InputReader.FilenameSanitizer.Replace(puzzle.Day, string.Empty).ToLowerInvariant();
        sanitizedDay = new string(new[] { sanitizedDay[0] }).ToUpperInvariant() + new string(sanitizedDay.Skip(1).ToArray());
        
        // set up the paths
        const string targetRoot = "./AdventOfCode.CSharp";
        var targetEventDirectory = $"{targetRoot}/{sanitizedEvent}";
        var targetPath = $"{targetEventDirectory}/{sanitizedDay}.cs";
        var sourcePath = $"./AdventOfCode.Today/{sanitizedEvent}/{sanitizedDay}.cs";

        // ensure the source path is found
        if (!File.Exists(sourcePath)) {
            RaiseError($"{sourcePath} not found");
            return;
        }
        
        // ensure the target path is not already present
        if (File.Exists(targetPath) && !isForced) {
            RaiseError($"{targetPath} already exists");
            return;
        }
        
        // ensure the target root is found
        if (!Directory.Exists(targetRoot)) {
            RaiseError($"Project root {targetRoot} not found");
            return;
        }
        
        // initialize the target event directory if not found
        if (!Directory.Exists(targetEventDirectory)) 
            Directory.CreateDirectory(targetEventDirectory);
        
        // stage the solver
        var contents = File.ReadAllText(sourcePath);
        contents = contents.Replace("namespace AdventOfCode.Today", "namespace AdventOfCode.CSharp");
        File.WriteAllText(targetPath, contents);
        
        // write completion
        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"Staged solver {sourcePath} to {targetPath}");
        Console.ResetColor();
        return;

        void RaiseError(string errorMessage) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine(errorMessage);
            Console.ResetColor();

            console.WriteLine(string.Empty);
        }
    }
}