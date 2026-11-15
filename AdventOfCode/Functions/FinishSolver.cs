using AdventOfCode.Services;

namespace AdventOfCode.Functions;

internal class FinishSolver {
    public void Execute() {
        if (!Workflow.TryLocateSolver(out var sourcePath)) {
            Console.WriteError("No solver detected");
            return;
        }

        if (!Workflow.TryStageFile(sourcePath!, true, out var targetPath)) 
            throw new InvalidOperationException("Failed to stage file during finish");
        
        // delete the source
        File.Delete(sourcePath!);
        
        // delete the source directory if empty
        var sourceDirectory = Path.GetDirectoryName(sourcePath!)!;
        if (Directory.GetFiles(sourceDirectory).Length == 0)
            Directory.Delete(sourceDirectory);
        
        // rebuild the library
        if (!Workflow.Build()) {
            Console.WriteError($"Build failed after committing {sourcePath} to library");
            return;
        }
        
        Console.WriteMessage($"Archived solver {sourcePath} to {targetPath}");
    }
}