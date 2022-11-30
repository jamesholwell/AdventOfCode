using AdventOfCode.Core;
using Xunit;

namespace AdventOfCode.CSharp.Examples;

public class Day0 : Solver {
    public Day0(string? input = null) : base(input) { }

    public override long SolvePartOne() {
        return Input.ToCharArray().Aggregate(1L, (current, c) => current * c);
    }

    [Fact]
    public void SolvesExample() {
        const string? exampleInput = @"foo";

        var solver = new Day0(exampleInput);
        var actual = solver.SolvePartOne();

        Assert.Equal(1256742, actual);
    }
}