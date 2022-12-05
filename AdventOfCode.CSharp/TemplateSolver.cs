using AdventOfCode.Core;
using Xunit;

namespace AdventOfCode.CSharp;

public class TemplateSolver : Solver {
    public TemplateSolver(string? input = null) : base(input) { }

    public override long SolvePartOne() => 0;

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
foo
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new TemplateSolver(ExampleInput).SolvePartOne();
        Assert.Equal(0, actual);
    }
}