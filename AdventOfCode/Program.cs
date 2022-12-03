using System.CommandLine;
using AdventOfCode;
using AdventOfCode.Core;
using AdventOfCode.FSharp.Examples;

var puzzleArgument = new Argument<PuzzleSpecification>(
    "puzzle",
    description: "Puzzle to solve e.g. 'day1' or '2015 day1' or 'day3 part2'",
    parse: PuzzleSpecification.Parser) {Arity = ArgumentArity.ZeroOrMore};

var solverArgument = new Option<string>(
    "--solver",
    "Solver to use e.g. csharp, fsharp or specify solver name");

var benchmarkArgument = new Option<bool>(
    "--bench",
    "Benchmark solver performance");

var root = new RootCommand {puzzleArgument, solverArgument, benchmarkArgument};

root.SetHandler(context => {
    var puzzle = context.ParseResult.GetValueForArgument(puzzleArgument);
    var solverHint = context.ParseResult.GetValueForOption(solverArgument);
    var isBenchmarking = context.ParseResult.GetValueForOption(benchmarkArgument);

    if (string.IsNullOrWhiteSpace(puzzle.Day) && DateTime.UtcNow.Month == 12)
        puzzle.Day = $"day{DateTime.UtcNow.Day}";

    if (string.IsNullOrWhiteSpace(puzzle.Event))
        puzzle.Event = DateTime.UtcNow.AddYears(DateTime.UtcNow.Month < 12 ? -1 : 0).Year.ToString();

    var factory =
        new SolverFactory()
            .AddAssembly<AdventOfCode.CSharp.Examples.Day0>("csharp")
            .AddAssembly<Day0>("fsharp");

    var cli = new SolverCli(context.Console, factory);

    if (string.IsNullOrWhiteSpace(puzzle.Day))
        cli.ListSolvers();
    else if (isBenchmarking)
        cli.Benchmark(puzzle, solverHint);
    else
        cli.Solve(puzzle, solverHint);
});

await root.InvokeAsync(args);