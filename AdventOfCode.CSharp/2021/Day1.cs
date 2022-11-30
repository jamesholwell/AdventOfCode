using AdventOfCode.Core;
using Xunit;

namespace AdventOfCode.CSharp._2021;

public class Day1 : Solver {
    private const string ExampleInput =
        @"199
200
208
210
200
207
240
269
260
263
";

    public Day1(string? input = null) : base(input) { }

    public override long SolvePartOne() {
        var increases = 0;
        var lastDepth = default(int?);

        foreach (var depth in Input.SplitInt()) {
            if (depth > lastDepth) ++increases;
            lastDepth = depth;
        }

        return increases;
    }

    public override long SolvePartTwo() {
        var increases = 0;

        var depths = Input.SplitInt();
        for (var index = 3; index < depths.Length; index++)
            if (depths[index] > depths[index - 3])
                ++increases;

        return increases;
    }


    [Fact]
    public void SolvesPart1Example() {
        var solver = new Day1(ExampleInput);
        var actual = solver.SolvePartOne();

        Assert.Equal(7, actual);
    }

    [Fact]
    public void SolvesPart2Example() {
        var solver = new Day1(ExampleInput);
        var actual = solver.SolvePartTwo();

        Assert.Equal(5, actual);
    }
}