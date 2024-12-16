using System.CommandLine;
using AdventOfCode.Core;
using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class FinishSolver(IConsole console) {
    public void Execute(PuzzleSpecification puzzle) {
        // sanitize and beautify the arguments
        var sanitizedEvent = InputReader.FilenameSanitizer.Replace(puzzle.Event, string.Empty);
        var sanitizedDay = InputReader.FilenameSanitizer.Replace(puzzle.Day, string.Empty).ToLowerInvariant();
        sanitizedDay = new string(new[] { sanitizedDay[0] }).ToUpperInvariant() + new string(sanitizedDay.Skip(1).ToArray());
        
        // set up the paths
        const string targetRoot = "./AdventOfCode.CSharp";
        var targetEventDirectory = $"{targetRoot}/{sanitizedEvent}";
        var targetPath = $"{targetEventDirectory}/{sanitizedDay}.cs";
        var sourceEventDirectory = $"./AdventOfCode.Today/{sanitizedEvent}";
        var sourcePath = $"{sourceEventDirectory}/{sanitizedDay}.cs";

        // ensure the source path is found
        if (!File.Exists(sourcePath)) {
            RaiseError($"{sourcePath} not found");
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
        
        // archive any existing file in the target path
        if (File.Exists(targetPath)) {
            File.Move(targetPath, $"{targetEventDirectory}/{sanitizedDay}_{Guid.NewGuid().ToString()[..8]}.cs");
        }
        
        // copy the solver to the target
        var contents = File.ReadAllText(sourcePath);
        contents = contents.Replace("namespace AdventOfCode.Today", "namespace AdventOfCode.CSharp");
        File.WriteAllText(targetPath, contents);
        
        // tidy up the source
        File.Delete(sourcePath);
        if (Directory.GetFiles(sourceEventDirectory).Length == 0)
            Directory.Delete(sourceEventDirectory);
        
        // write completion
        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"Archived solver {sourcePath} to {targetPath}");
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