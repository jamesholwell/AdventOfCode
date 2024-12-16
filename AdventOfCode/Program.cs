using System.CommandLine;
using AdventOfCode.Core;
using AdventOfCode.Functions;
using AdventOfCode.Infrastructure;

var factory =
    new SolverFactory()
        .AddAssembly<AdventOfCode.CSharp.TemplateSolver>(string.Empty)
        .AddAssembly<AdventOfCode.FSharp.TemplateSolver>("fsharp");

var puzzleArgument =
    new Argument<PuzzleSpecification>(
        "puzzle",
        description: "Puzzle to solve e.g. 'day1' or '2015 day1' or 'day3 part2'",
        parse: result => {
            var spec = new PuzzleSpecification();
            var taken = 0;

            foreach (var token in result.Tokens) {
                var value = token.Value;
                ++taken;

                if (value.StartsWith("-")) break;

                if (value.StartsWith("day"))
                    spec.Day = value;

                else if (value.StartsWith("pt"))
                    spec.IsPartTwo = "pt2".Equals(value, StringComparison.OrdinalIgnoreCase);

                else if (value.StartsWith("part"))
                    spec.IsPartTwo = "part2".Equals(value, StringComparison.OrdinalIgnoreCase);

                else if (value.StartsWith("20"))
                    spec.Event = value;

                else
                    spec.SolverHint = value;
            }

            result.OnlyTake(taken);

            return spec;
        }) { Arity = ArgumentArity.ZeroOrMore };

var startArgument = new Option<bool>(
    "--start",
    "Initialize a new empty solver in the today project");

var stageArgument = new Option<bool>(
    "--stage",
    "Stage a 'today' solver into the main project");

var traceArgument = new Option<bool>(
    "--trace",
    "Output trace information");

var listArgument = new Option<bool>(
    "--list",
    "Show all available solvers");

var benchmarkArgument = new Option<bool>(
    "--bench",
    "Benchmark solver performance");

var forceArgument = new Option<bool>(
    "--force",
    "Force an operation to continue despite errors (e.g. to overwrite an existing file)");

var root = new RootCommand { puzzleArgument, startArgument, stageArgument, traceArgument, listArgument, benchmarkArgument, forceArgument };

root.SetHandler(context => {
    var puzzle = context.ParseResult.GetValueForArgument(puzzleArgument);

    if (string.IsNullOrWhiteSpace(puzzle.Day) && DateTime.UtcNow.Month == 12)
        puzzle.Day = $"day{DateTime.UtcNow.AddHours(-5).Day}";

    if (string.IsNullOrWhiteSpace(puzzle.Event))
        puzzle.Event = DateTime.UtcNow.AddYears(DateTime.UtcNow.Month < 12 ? -1 : 0).Year.ToString();

    var isStarting = context.ParseResult.GetValueForOption(startArgument);
    if (isStarting) {
        new CreateSolver(context.Console).Execute(puzzle);
        return;
    }
    
    var isStaging = context.ParseResult.GetValueForOption(stageArgument);
    if (isStaging) {
        var isForcing = context.ParseResult.GetValueForOption(forceArgument);
        new StageSolver(context.Console).Execute(puzzle, isForcing);
        return;
    }
    
    var isListing = context.ParseResult.GetValueForOption(listArgument);
    if (isListing ||
        string.IsNullOrWhiteSpace(puzzle.Event) ||
        string.IsNullOrWhiteSpace(puzzle.Day)) {
        new ListSolvers(context.Console, factory).Execute();
        return;
    }
    
    var isTracing = context.ParseResult.GetValueForOption(traceArgument);
    var isBenchmarking = context.ParseResult.GetValueForOption(benchmarkArgument);
    if (isBenchmarking)
        new BenchmarkSolvers(context.Console, factory).Execute(puzzle);
    else
        new RunSolver(context.Console, factory, isTracing).Execute(puzzle);
});

await root.InvokeAsync(args);