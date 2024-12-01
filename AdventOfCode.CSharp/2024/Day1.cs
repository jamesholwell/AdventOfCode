using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day1 : Solver {
    public Day1(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private (int[] left, int[] right) Parse() {
        var lines = Shared.Split(Input);
        var left = new int[lines.Length];
        var right = new int[lines.Length];

        for (int i = 0, c = lines.Length; i < c; ++i) {
            var parts = lines[i].Split("   ");
            left[i] = int.Parse(parts[0]);
            right[i] = int.Parse(parts[1]);
        }

        return (left, right);
    }

    protected override long SolvePartOne() {
        var (left, right) = Parse();

        Array.Sort(left);
        Array.Sort(right);

        return left.Zip(right, (l, r) => Math.Abs(r - l)).Sum();
    }

    protected override long SolvePartTwo() {
        var (left, right) = Parse();
        var rightOccurrences = right.GroupBy(r => r).ToDictionary(g => g.Key, g => g.Count());

        return left.Where(rightOccurrences.ContainsKey).Sum(l => l * rightOccurrences[l]);
    }

    private const string? ExampleInput = @"
3   4
4   3
2   5
1   3
3   9
3   3
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day1(ExampleInput, Output).SolvePartOne();
        Assert.Equal(11, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day1(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(31, actual);
    }
}