using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class CreateSolver() {
    public void Execute(PuzzleArguments puzzle, bool isForced) {
        if (!puzzle.IsValid) {
            WorkflowHelper.WriteError($"Failed to parse complete puzzle arguments (found: {puzzle})");
            return;
        }
        
        if (!WorkflowHelper.TryInitialiseSolver(puzzle.Event, puzzle.Day, puzzle.Suffix, isForced, out var path)) {
            WorkflowHelper.WriteError($"Failed to create solver at {path}: you made need to use the --force");
            return;
        }
            
        WorkflowHelper.WriteMessage($"Initialized new solver at {path}");
    }
}