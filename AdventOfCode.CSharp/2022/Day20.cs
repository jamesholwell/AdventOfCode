using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day20 : Solver {
    public Day20(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var offsets = Input.SplitInt();
        var c = offsets.Length;
        var m = c - 1;
        if (c < 1) throw new InvalidOperationException("Empty input: no dice");

        var indexes = Enumerable.Range(0, c).ToArray();
        var contents = offsets.ToArray();

        Trace.WriteLine("Initial arrangement:");
        Trace.WriteLine(string.Join(", ", offsets));
        Trace.WriteLine();

        for (var i = 0; i < c; ++i) {
            if (offsets[i] != 0) {
                var oldIndex = indexes[i];
                var newIndex = ((oldIndex + offsets[i]) % m + m) % m;

                // handle the special first case
                if (newIndex == 0) newIndex = m;

                if (newIndex == oldIndex) continue;

                // number moving right, wake moving left
                if (oldIndex < newIndex) {
                    for (var j = 0; j < c; ++j)
                        if (oldIndex < indexes[j] && indexes[j] <= newIndex)
                            indexes[j]--;
                }

                // number moving left, wake moving right
                if (newIndex < oldIndex) {
                    for (var j = 0; j < c; ++j)
                        if (newIndex <= indexes[j] && indexes[j] < oldIndex)
                            indexes[j]++;
                }

                // set number in rightful place
                indexes[i] = newIndex;

                // copy new contents
                for (var j = 0; j < c; ++j) contents[indexes[j]] = offsets[j];
                Trace.WriteLine($"{offsets[i]} moves between {contents[((newIndex - 1) % c + c) % c]} and {contents[(newIndex + 1) % c]} (from position {oldIndex + 1} to {newIndex + 1})");
            }
            else
            {
                Trace.WriteLine("0 does not move:");
            }

            Trace.WriteLine(string.Join(", ", contents));
            Trace.WriteLine();
        }

        var originalZeroPosition = Array.IndexOf(offsets, 0);
        var newZeroPosition = indexes[originalZeroPosition];
        var _1000 = contents[(newZeroPosition + 1000 + c) % c];
        var _2000 = contents[(newZeroPosition + 2000 + c) % c];
        var _3000 = contents[(newZeroPosition + 3000 + c) % c];

        Output.WriteLine($"1000th number is {_1000}");
        Output.WriteLine($"2000th number is {_2000}");
        Output.WriteLine($"3000th number is {_3000}");
        return _1000 + _2000 + _3000;
    }
    
    private const string? ExampleInput = @"
1
2
-3
3
-2
0
4
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day20(ExampleInput, Output).SolvePartOne();
        Assert.Equal(3, actual);
    }
}