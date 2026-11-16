using AdventOfCode.Services;

namespace AdventOfCode.Functions;

internal class ListSolvers(SolverFactory factory) {
    public void Execute() {
        var solvers = factory.List().ToArray();

        if (solvers.Length == 0) {
            Console.WriteError("No solvers found");
        }
        else {
            Console.WriteMessage("# The following solvers are available:");
            
            foreach (var candidate in solvers) 
                Console.WriteMessage($"#   {candidate}");
            
            Console.WriteLine();
        }
    }
}