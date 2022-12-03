using System.CommandLine;
using AdventOfCode;
using AdventOfCode.Core;
using AdventOfCode.FSharp.Examples;

var puzzleArgument = new Argument<PuzzleSpecification>(
    "puzzle",
    description: "Puzzle to solve e.g. 'day1' or '2015 day1' or 'day3 part2'",
    parse: PuzzleSpecification.Parser) {Arity = ArgumentArity.ZeroOrMore};

var listArgument = new Option<bool>(
    "--list",
    "Show all available solvers");

var benchmarkArgument = new Option<bool>(
    "--bench",
    "Benchmark solver performance");

var root = new RootCommand {puzzleArgument, listArgument, benchmarkArgument};

root.SetHandler(context => {
    var factory =
        new SolverFactory()
            .AddAssembly<AdventOfCode.CSharp.Examples.Day0>("csharp")
            .AddAssembly<Day0>("fsharp");

    var cli = new SolverCli(context.Console, factory);

    // if we are listing we don't need to parse the puzzle
    var isListing = context.ParseResult.GetValueForOption(listArgument);
    if (isListing) {
        cli.ListSolvers();
        return;
    }

    var puzzle = context.ParseResult.GetValueForArgument(puzzleArgument);
    var isBenchmarking = context.ParseResult.GetValueForOption(benchmarkArgument);

    if (string.IsNullOrWhiteSpace(puzzle.Day) && DateTime.UtcNow.Month == 12)
        puzzle.Day = $"day{DateTime.UtcNow.Day}";

    if (string.IsNullOrWhiteSpace(puzzle.Event))
        puzzle.Event = DateTime.UtcNow.AddYears(DateTime.UtcNow.Month < 12 ? -1 : 0).Year.ToString();

    if (isBenchmarking)
        cli.Benchmark(puzzle);
    else
        cli.Solve(puzzle);
});

await root.InvokeAsync(args);