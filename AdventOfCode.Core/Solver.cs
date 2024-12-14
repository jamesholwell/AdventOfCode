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
        Output = outputHelper ?? new NullOutputHelper();
        
        // disable tracing in default cli usage, but keep it enabled for tests
        Trace = outputHelper is ConsoleOutputHelper ? new NullOutputHelper() : Output;
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

public abstract class Solver(string? input, ITestOutputHelper? outputHelper = null)
    : Solver<long>(input, outputHelper);