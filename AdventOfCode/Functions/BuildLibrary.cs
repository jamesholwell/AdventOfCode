using AdventOfCode.Services;

namespace AdventOfCode.Functions;

internal class BuildLibrary {
    public void Execute() {
        if (!Workflow.Build()) Console.WriteError($"Build failed");
    }
}