using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day2 : Solver {
    public Day2(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private int[][] Parse() {
        return Shared.Split(Input)
            .Select(s => s.Split(" ").Select(int.Parse).ToArray())
            .ToArray();
    }

    private static bool IsSafe(int[] reports) {
        var isIncreasing = reports[1] > reports[0];

        return reports.Pairwise().All(t =>
            t.Item1 != t.Item2 && Math.Abs(t.Item2 - t.Item1) < 4 && t.Item2 > t.Item1 == isIncreasing);
    }

    private static bool IsSafeWithDampener(int[] reports) {
        return 
            IsSafe(reports) ||
            Enumerable.Range(0, reports.Length)
            .Any(i => IsSafe(reports.Take(i).Concat(reports.Skip(i + 1)).ToArray()));
    }

    protected override long SolvePartOne() {
        var lines = Parse();

        return lines.Count(IsSafe);
    }

    protected override long SolvePartTwo() {
        var lines = Parse();

        return lines.Count(IsSafeWithDampener);
    }

    private const string? ExampleInput = @"
7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day2(ExampleInput, Output).SolvePartOne();
        Assert.Equal(2, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day2(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(4, actual);
    }
    
    [Theory]
    [InlineData(new[] { 7, 6, 4, 2, 1 }, true)]
    [InlineData(new[] { 1, 2, 7, 8, 9 }, false)]
    [InlineData(new[] { 9, 7, 6, 2, 1 }, false)]
    [InlineData(new[] { 1, 3, 2, 4, 5 }, true)]
    [InlineData(new[] { 8, 6, 4, 4, 1 }, true)]
    [InlineData(new[] { 1, 3, 6, 7, 9 }, true)]
    public void SolvesPartTwoLines(int[] line, bool expected) {
        Assert.Equal(expected, IsSafeWithDampener(line));
    }
}