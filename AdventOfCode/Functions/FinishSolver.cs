using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class FinishSolver() {
    public void Execute() {
        if (!WorkflowHelper.TryLocateSolver(out var sourcePath)) {
            WorkflowHelper.WriteError("No solver detected");
            return;
        }

        if (!WorkflowHelper.TryStageFile(sourcePath!, true, out var targetPath)) 
            throw new InvalidOperationException("Failed to stage file during finish");
        
        // delete the source
        File.Delete(sourcePath!);
        
        // delete the source directory if empty
        var sourceDirectory = Path.GetDirectoryName(sourcePath!)!;
        if (Directory.GetFiles(sourceDirectory).Length == 0)
            Directory.Delete(sourceDirectory);
        
        WorkflowHelper.WriteMessage($"Archived solver {sourcePath} to {targetPath}");
    }
}