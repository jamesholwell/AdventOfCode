using AdventOfCode.Core;
using System.CommandLine;
using AdventOfCode.Infrastructure;

namespace AdventOfCode.Functions;

internal class ListSolvers(IConsole console, SolverFactory factory) {
    public void Execute() {
        var solvers = factory.List().ToArray();

        if (!solvers.Any()) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            console.WriteLine("No solvers found");
        }
        else {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            console.WriteLine("# The following solvers are available:");
            console.WriteLine("#   ");

            foreach (var candidate in solvers) {
                console.Write("#   ");
                console.WriteLine(candidate);
            }
        }

        Console.ResetColor();
        console.WriteLine(string.Empty);
    }
}