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

    public override long SolvePartTwo() {
        
        var accumulator = new List<int>();

        foreach (var line in Shared.Split(Input)) {
            var values = line.SplitBy(" ").Select(int.Parse).ToArray();

            var queue = new Queue<int[]>();
            queue.Enqueue(values);

            while (values.Any(v => v != 0)) {
                var c = values.Length - 1;
                var next = new int[c];
                for (var i = 0; i < c; ++i)
                    next[i] = values[i + 1] - values[i];

                values = next;
                queue.Enqueue(next);
            }

            var seed = queue.Dequeue()[0];
            var j = 0;
            accumulator.Add(queue.Aggregate(seed, (acc, el) => acc + el[0] * (1 - 2 * (++j % 2))));
        }

        return accumulator.Sum();
    }

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
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day9(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(2, actual);
    }
}