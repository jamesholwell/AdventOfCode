using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day15 : Solver {
    public Day15(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        return Input
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .SplitBy(",")
            .Sum(Hash);
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private int Hash(string s) {
        return s.Aggregate(0, (current, c) => (17 * (current + c)) % 256);
    }
    
    private const string? ExampleInput = @"
rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
";

    [Fact]
    public void HashesHashAsExpected() {
        var actual = Hash("HASH");
        Assert.Equal(52, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day15(ExampleInput, Output).SolvePartOne();
        Assert.Equal(1320, actual);
    }
}