using AdventOfCode.Core;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp;

public abstract class Solver<T> : ISolver {
    protected Solver(string? input = null, ITestOutputHelper? outputHelper = null) {
        Input = input ?? string.Empty;
        Output = outputHelper ?? new ConsoleOutputHelper();
        Trace = outputHelper ?? new NullOutputHelper();
    }

    protected string Input { get; }

    protected ITestOutputHelper Output { get; }

    protected ITestOutputHelper Trace { get; }
    
    string ISolver.SolvePartOne() => Convert.ToString(SolvePartOne()) ?? throw new NotImplementedException();

    string ISolver.SolvePartTwo() => Convert.ToString(SolvePartTwo()) ?? throw new NotImplementedException();

    public abstract T SolvePartOne();

    public abstract T SolvePartTwo();
}

public abstract class Solver : Solver<long> {
    protected Solver(string? input, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() => throw new NotImplementedException();

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");
}