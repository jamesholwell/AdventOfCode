using System.CommandLine;
using System.CommandLine.Parsing;
using AdventOfCode.Functions;
using AdventOfCode.Infrastructure;
using AdventOfCode.Shared;

var puzzleArgument =
    new Argument<PuzzleArguments>("puzzle") {
        Description = "Puzzle to solve e.g. 'day1' or '2015 day1' or 'day3 part2'",
        Arity = ArgumentArity.ZeroOrMore,
        CustomParser = PuzzleArguments.Parse
    };

var forceOption = new Option<bool>("--force") { Description = "Continue despite errors (e.g. overwrite an existing file)" };

// basic usage
var traceOption = new Option<bool>("--trace") { Description = "Output trace information" };
var benchmarkOption = new Option<bool>("--bench") { Description = "Benchmark solver performance" };
var run = new Command("run", "Solve a puzzle (default)") {
    Arguments = { puzzleArgument },
    Options = { traceOption, benchmarkOption },
};
Action<ParseResult> runAction = parse => {
    var puzzle = parse.GetValue(puzzleArgument);
    var factory = CreateSolverFactory();

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
start.SetAction(parse => new CreateSolver().Execute(parse.GetValue(puzzleArgument)!, parse.GetValue(forceOption)));

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
    var factory = CreateSolverFactory();
    new ListSolvers(factory).Execute();
});

// build the command tree
var root = new RootCommand("Advent of Code runner") { puzzleArgument, traceOption, benchmarkOption };
root.Subcommands.Add(run);
root.Subcommands.Add(start);
root.Subcommands.Add(stage);
root.Subcommands.Add(finish);
root.Subcommands.Add(next);
root.Subcommands.Add(list);
root.SetAction(runAction);

try {
    return await root.Parse(args).InvokeAsync(new InvocationConfiguration {
        EnableDefaultExceptionHandler = false
    });
}
catch (AmbiguousSolverException e) {
    WorkflowHelper.WriteError(e.Message);

    Console.ForegroundColor = ConsoleColor.DarkGray;
    foreach (var candidate in e.Candidates)
        Console.WriteLine(candidate);

    Console.WriteLine();
    Console.ResetColor();

    return -1;
}
catch (Exception e) {
    WorkflowHelper.WriteError(e.Message);

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

public enum Parts {
    Part1 = 1,
    Part2 = 2
}

public readonly record struct PuzzleArguments {
    public int Event { get; private init; }

    public int Day { get; private init; }

    public Parts Part { get; private init; }

    public string Suffix { get; private init; }

    public bool IsValid => Day is >= 1 and <= 25 && Event is >= 2015 and < 2099 && (Suffix == null || Puzzle.SuffixPattern.IsMatch(Suffix));

    public static PuzzleArguments Parse(ArgumentResult result) {
        var args = CreateDefault();

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
                args = args with { Part = Parts.Part2 };

            // anything else is pushed into the hint
            else
                args = args with { Suffix = $"{args.Suffix}{value}" };
        }
        
        return args;
    }

    private static PuzzleArguments CreateDefault() {
        var now = DateTime.UtcNow.AddHours(-5);

        return new PuzzleArguments {
            Event = now.Month == 12 ? now.Year : now.Year - 1,
            Day = now.Month == 12 ? now.Day : 0,
            Part = Parts.Part1,
            Suffix = string.Empty
        };
    }

    public override string ToString() => $"(Event = {Event}, Day = {Day}" + (Suffix == null ? string.Empty : $", Suffix = {Suffix}") + ")";
}