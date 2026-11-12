using AdventOfCode.Services;
using AdventOfCode.Shared;

namespace AdventOfCode.Functions;

internal class NextSolver {
    public void Execute() {
        if (!Workflow.TryLocateSolver(out var sourcePath)) {
            Console.WriteError("No solver detected");
            return;
        }

        if (!Workflow.TryStageFile(sourcePath!, true, out var targetPath))
            throw new InvalidOperationException("Failed to stage file during finish");

        // delete the source
        File.Delete(sourcePath!);

        // try to extract the day
        var eventCandidate = Path.GetFileName(Path.GetDirectoryName(sourcePath)!);
        var dayMatch = Puzzle.DayPattern.Match(Path.GetFileNameWithoutExtension(sourcePath!));
        if (int.TryParse(eventCandidate, out var @event) && dayMatch.Success) {
            var day = Puzzle.GetDay(dayMatch);
            var suffix = Puzzle.GetSuffix(dayMatch);
            if (day < 25) {
                if (Workflow.TryInitialiseSolver(@event, day + 1, suffix, false, out _)) {
                    Console.WriteMessage($"Archived solver {sourcePath} and initialized Day {day + 1}");
                    return;
                }
            }
        }

        // otherwise delete the source directory if empty
        var sourceDirectory = Path.GetDirectoryName(sourcePath!)!;
        if (Directory.GetFiles(sourceDirectory).Length == 0)
            Directory.Delete(sourceDirectory);
        
        Console.WriteMessage($"Archived solver {sourcePath} to {targetPath}");
    }
}