using AdventOfCode.Services;

namespace AdventOfCode.Functions;

internal class BuildLibrary {
    public int Execute() {
        if (Workflow.Build()) return 0;
        
        Console.WriteError($"Build failed");
        return -1;
    }
}