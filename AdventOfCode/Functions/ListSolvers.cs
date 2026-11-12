using AdventOfCode.Infrastructure;
using AdventOfCode.Services;

namespace AdventOfCode.Functions;

internal class ListSolvers(SolverFactory factory) {
    public void Execute() {
        var solvers = factory.List().ToArray();

        if (!solvers.Any()) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("No solvers found");
        }
        else {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("# The following solvers are available:");
            Console.WriteLine("#   ");

            foreach (var candidate in solvers) {
                Console.Write("#   ");
                Console.WriteLine(candidate);
            }
        }

        Console.ResetColor();
        Console.WriteLine(string.Empty);
    }
}