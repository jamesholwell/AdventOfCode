namespace AdventOfCode.Core;

public abstract class Solver : ISolver {
    protected Solver(string? input = null) {
        Input = input ?? string.Empty;
    }

    protected string Input { get; }

    public virtual long SolvePartOne() {
        throw new NotImplementedException();
    }

    public virtual long SolvePartTwo() {
        throw new NotImplementedException();
    }
}