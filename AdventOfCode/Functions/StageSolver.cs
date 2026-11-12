using AdventOfCode.Services;

namespace AdventOfCode.Functions;

internal class StageSolver {
    public void Execute(bool isForced) {
        if (!Workflow.TryLocateSolver(out var sourcePath)) {
            Console.WriteError("No solver detected");
            return;
        }

        if (!Workflow.TryStageFile(sourcePath!, isForced, out var targetPath)) {
            Console.WriteError($"Failed to stage {sourcePath} to {targetPath}: you made need to use the --force");
            return;
        }

        Console.WriteMessage($"Staged solver {sourcePath} to {targetPath}");
    }
}