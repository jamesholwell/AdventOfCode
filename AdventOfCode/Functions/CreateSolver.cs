using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using AdventOfCode.Core;
using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class CreateSolver(IConsole console) {
    public void Execute(PuzzleSpecification puzzle) {
        // sanitize and beautify the arguments
        var sanitizedEvent = InputReader.FilenameSanitizer.Replace(puzzle.Event, string.Empty);
        var sanitizedDay = InputReader.FilenameSanitizer.Replace(puzzle.Day, string.Empty).ToLowerInvariant();
        sanitizedDay = new string(new[] { sanitizedDay[0] }).ToUpperInvariant() + new string(sanitizedDay.Skip(1).ToArray());
        
        // set up the paths
        const string projectRoot = "./AdventOfCode.Today";
        const string templateSolverPath = "./AdventOfCode.CSharp/TemplateSolver.cs";
        var eventDirectory = $"{projectRoot}/{sanitizedEvent}";
        var solverPath = $"{eventDirectory}/{sanitizedDay}.cs";

        // ensure the file is not already present
        if (File.Exists(solverPath)) {
            RaiseError($"{solverPath} already exists");
            return;
        }
        
        // ensure the root directory is found
        if (!Directory.Exists(projectRoot)) {
            RaiseError($"Project root {projectRoot} not found");
            return;
        }
        
        // ensure the template solver is found
        if (!File.Exists(templateSolverPath)) {
            RaiseError($"Template {templateSolverPath} not found");
            return;
        }

        // initialize the event directory if not found
        if (!Directory.Exists(eventDirectory)) 
            Directory.CreateDirectory(eventDirectory);
        
        // initialize the solver
        var templateContents = File.ReadAllText(templateSolverPath);
        templateContents = templateContents
                .Replace("namespace AdventOfCode.CSharp;", $"namespace AdventOfCode.Today._{sanitizedEvent};")
                .Replace("TemplateSolver", $"{sanitizedDay}");
        File.WriteAllText(solverPath, templateContents);
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        console.WriteLine($"Initialized new solver {solverPath}");
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