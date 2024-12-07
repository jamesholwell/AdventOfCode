using AdventOfCode.Core.Output;
using Xunit.Abstractions;

namespace AdventOfCode.Core;

public interface ISolver {
    string SolvePartOne();
    string SolvePartTwo();
}

public abstract class Solver<T> : ISolver {
    protected Solver(string? input = null, ITestOutputHelper? outputHelper = null) {
        Input = input ?? string.Empty;
        Trace = Output = new TestOutputHelper(outputHelper);
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global - used by downstream classes
    protected string Input { get; }

    protected ITestOutputHelper Output { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global - used by downstream classes
    protected ITestOutputHelper Trace { get; }

    string ISolver.SolvePartOne() => Convert.ToString(SolvePartOne()) ?? throw new NotImplementedException();

    string ISolver.SolvePartTwo() => Convert.ToString(SolvePartTwo()) ?? throw new NotImplementedException();

    protected abstract T SolvePartOne();

    protected abstract T SolvePartTwo();
}

public abstract class Solver : Solver<long> {
    protected Solver(string? input, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }
}