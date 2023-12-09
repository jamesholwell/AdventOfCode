using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day9 : Solver {
    public Day9(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var accumulator = new List<int>();

        foreach (var line in Shared.Split(Input)) {
            var values = line.SplitBy(" ").Select(int.Parse).ToArray();

            var stack = new Stack<int[]>();
            stack.Push(values);

            while (values.Any(v => v != 0)) {
                var c = values.Length - 1;
                var next = new int[c];
                for (var i = 0; i < c; ++i)
                    next[i] = values[i + 1] - values[i];

                values = next;
                stack.Push(next);
            }

            accumulator.Add(stack.Aggregate(0, (acc, el) => acc + el.Last()));
        }

        return accumulator.Sum();
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day9(ExampleInput, Output).SolvePartOne();
        Assert.Equal(114, actual);
    }
}