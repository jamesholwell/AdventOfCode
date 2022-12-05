namespace AdventOfCode.Core;

public abstract class Solver<T> : ISolver {
    protected Solver(string? input = null) {
        Input = input ?? string.Empty;
    }

    protected string Input { get; }

    string ISolver.SolvePartOne() {
        return Convert.ToString(SolvePartOne()) ?? throw new NotImplementedException();
    }

    string ISolver.SolvePartTwo() {
        return Convert.ToString(SolvePartTwo()) ?? throw new NotImplementedException();
    }

    public abstract T SolvePartOne();

    public abstract T SolvePartTwo();
}

public abstract class Solver : Solver<long> {
    protected Solver(string? input) : base(input) { }

    public override long SolvePartOne() {
        throw new NotImplementedException();
    }

    public override long SolvePartTwo() {
        throw new NotImplementedException("Solve part 1 first");
    }
}