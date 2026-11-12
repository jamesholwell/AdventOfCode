using System.CommandLine;
using AdventOfCode;
using AdventOfCode.Services;

try {
    return await
        CommandLine.BuildRoot(CreateSolverFactory)
            .Parse(args)
            .InvokeAsync(new InvocationConfiguration {
                EnableDefaultExceptionHandler = false
            });
}
catch (AmbiguousSolverException e) {
    Console.WriteError(e.Message);

    Console.ForegroundColor = ConsoleColor.DarkGray;
    foreach (var candidate in e.Candidates)
        Console.WriteLine(candidate);

    Console.WriteLine();
    Console.ResetColor();

    return -1;
}
catch (Exception e) {
    Console.WriteError(e.Message);

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine(e.ToString());
    Console.WriteLine(string.Empty);
    return -1;
}

SolverFactory CreateSolverFactory() {
    var solverFactory =
        new SolverFactory()
            .AddAssembly<AdventOfCode.CSharp.TemplateSolver>(string.Empty)
            .AddAssembly<AdventOfCode.FSharp.TemplateSolver>("fsharp");
    return solverFactory;
}