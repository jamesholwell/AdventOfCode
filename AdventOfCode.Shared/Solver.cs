using Xunit.Abstractions;

namespace AdventOfCode.Shared;

public interface ISolver {
    string SolvePartOne();
    string SolvePartTwo();
}

/// <summary>
///     Strongly typed base class for solvers returning solutions of type T
/// </summary>
/// <typeparam name="T">solution type, often long or string</typeparam>
public abstract class Solver<T> : ISolver {
    /// <summary>
    ///     Construct the base for puzzle solvers
    /// </summary>
    /// <param name="input">unprocessed contents of the input file</param>
    /// <param name="outputHelper">console-like interface supplied by Xunit / the runner</param>
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

/// <summary>
///     Strongly typed base class for solvers returning integer numbers
/// </summary>
/// <param name="input">unprocessed contents of the input file</param>
/// <param name="outputHelper">console-like interface supplied by Xunit / the runner</param>
public abstract class Solver(string? input, ITestOutputHelper? outputHelper = null)
    : Solver<long>(input, outputHelper);