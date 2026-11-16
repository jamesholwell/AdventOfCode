using System.CommandLine;
using AdventOfCode;
using AdventOfCode.Services;

try {
    return await
        CommandLine.BuildRoot()
            .Parse(args)
            .InvokeAsync(new InvocationConfiguration {
                EnableDefaultExceptionHandler = false
            });
}
catch (AmbiguousSolverException e) {
    Console.WriteError(e.Message);

    foreach (var candidate in e.Candidates)
        Console.WriteMessage(candidate);

    Console.WriteLine();
    return -1;
}
catch (Exception e) {
    Console.WriteError(e.Message);

    Console.WriteMessage(e.ToString());
    Console.WriteLine();
    return -1;
}