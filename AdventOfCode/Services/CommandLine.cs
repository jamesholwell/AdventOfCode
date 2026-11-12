using System.CommandLine;
using System.CommandLine.Parsing;
using AdventOfCode.Functions;

namespace AdventOfCode.Services;

internal static class CommandLine {
    internal static RootCommand BuildRoot(Func<SolverFactory> solverFactoryFactory) {
        var puzzleArgument =
            new Argument<PuzzleArguments>("puzzle") {
                Description = "Puzzle to solve e.g. 'day1' or '2015 day1' or 'day3 part2'",
                Arity = ArgumentArity.ZeroOrMore,
                CustomParser = Parse
            };

        var forceOption = new Option<bool>("--force")
            { Description = "Continue despite errors (e.g. overwrite an existing file)" };

        // basic usage
        var traceOption = new Option<bool>("--trace") { Description = "Output trace information" };
        var benchmarkOption = new Option<bool>("--bench") { Description = "Benchmark solver performance" };
        var run = new Command("run", "Solve a puzzle (default)") {
            Arguments = { puzzleArgument },
            Options = { traceOption, benchmarkOption },
        };
        Action<ParseResult> runAction = parse => {
            var puzzle = parse.GetValue(puzzleArgument);
            var factory = solverFactoryFactory();

            if (!puzzle.IsValid)
                new ListSolvers(factory).Execute();
            else if (parse.GetValue(benchmarkOption))
                new BenchmarkSolvers(factory).Execute(puzzle);
            else
                new RunSolver(factory, parse.GetValue(traceOption)).Execute(puzzle);
        };
        run.SetAction(runAction);

        // workflow
        var start = new Command("start", "Initialize a new solver.") {
            Arguments = { puzzleArgument },
            Options = { forceOption }
        };
        start.SetAction(parse =>
            new CreateSolver().Execute(parse.GetValue(puzzleArgument)!, parse.GetValue(forceOption)));

        var stage = new Command("stage", "Copies the current solver to the library.") {
            Options = { forceOption }
        };
        stage.SetAction(parse => new StageSolver().Execute(parse.GetValue(forceOption)));

        var finish = new Command("finish", "Moves the current solver to the library and deletes the original.");
        finish.SetAction(_ => new FinishSolver().Execute());

        var next = new Command("next", "Finishes the current puzzle and starts the next day.");
        next.SetAction(_ => new NextSolver().Execute());

        var list = new Command("list", "Show all available solvers");
        list.SetAction(_ => {
            var factory = solverFactoryFactory();
            new ListSolvers(factory).Execute();
        });

        // build the command tree
        var rootCommand = new RootCommand("Advent of Code runner") { puzzleArgument, traceOption, benchmarkOption };
        rootCommand.Subcommands.Add(run);
        rootCommand.Subcommands.Add(start);
        rootCommand.Subcommands.Add(stage);
        rootCommand.Subcommands.Add(finish);
        rootCommand.Subcommands.Add(next);
        rootCommand.Subcommands.Add(list);
        rootCommand.SetAction(runAction);
        return rootCommand;
    }

    private static PuzzleArguments Parse(ArgumentResult result) {
        var args = PuzzleArguments.CreateDefault();

        foreach (var token in result.Tokens) {
            var value = token.Value;

            // skip options
            if (value.StartsWith('-') || value.StartsWith('/')) break;

            // detect event
            if (value.StartsWith("20") && value.Length == 4 && int.TryParse(value, out var @event))
                args = args with { Event = @event };

            // detect day
            else if (value.StartsWith("day") && value.Length > 3 && int.TryParse(value[3..], out var day))
                args = args with { Day = day };

            // detect part (part 1 by default)
            else if (value.Equals("pt2", StringComparison.OrdinalIgnoreCase)
                     || value.Equals("part2", StringComparison.OrdinalIgnoreCase))
                args = args with { Part = PuzzleArguments.Parts.Part2 };

            // anything else is pushed into the hint
            else
                args = args with { Suffix = $"{args.Suffix}{value}" };
        }

        return args;
    }
}