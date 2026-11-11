using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class StageSolver() {
    public void Execute(bool isForced) {
        if (!WorkflowHelper.TryLocateSolver(out var sourcePath)) {
            WorkflowHelper.WriteError("No solver detected");
            return;
        }

        if (!WorkflowHelper.TryStageFile(sourcePath!, isForced, out var targetPath)) {
            WorkflowHelper.WriteError($"Failed to stage {sourcePath} to {targetPath}: you made need to use the --force");
            return;
        }

        WorkflowHelper.WriteMessage($"Staged solver {sourcePath} to {targetPath}");
    }
}