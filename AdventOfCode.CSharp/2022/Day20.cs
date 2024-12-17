using AdventOfCode.Core;
using AdventOfCode.Core.Output;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day20(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    protected override long SolvePartOne() {
        var offsets = Input.SplitInt().Select(i => (long)i).ToArray();
        var indexes = Enumerable.Range(0, offsets.Length).ToArray();
        var contents = offsets.ToArray();

        Trace.WriteLine("Initial arrangement:");
        Trace.WriteLine(string.Join(", ", offsets));
        Trace.WriteLine();

        Mix(offsets, indexes, contents);

        return GetGroveCoords(offsets, indexes, contents);
    }

    protected override long SolvePartTwo() {
        var offsets = Input.SplitInt().Select(i => i * 811589153L).ToArray();
        var indexes = Enumerable.Range(0, offsets.Length).ToArray();
        var contents = offsets.ToArray();

        Trace.WriteLine("Initial arrangement:");
        Trace.WriteLine(string.Join(", ", offsets));
        Trace.WriteLine();

        for (var k = 0; k < 10; ++k) {
            Mix(offsets, indexes, contents);

            Trace.WriteLine($"After {k} rounds of mixing:");
            Trace.WriteLine(string.Join(", ", offsets));
            Trace.WriteLine();
        }

        return GetGroveCoords(offsets, indexes, contents);
    }

    private void Mix(long[] offsets, int[] indexes, long[] contents) {
        var c = offsets.Length;
        if (c < 1) throw new InvalidOperationException("Empty input: no dice");
        var m = c - 1;

        for (var i = 0; i < c; ++i) {
            if (offsets[i] == 0) {
                Trace.WriteLine("0 does not move:");
            }
            else {
                var oldIndex = indexes[i];
                var newIndex = (int)((oldIndex + offsets[i]) % m + m) % m;

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
                Trace.WriteLine(
                    $"{offsets[i]} moves between {contents[((newIndex - 1) % c + c) % c]} and {contents[(newIndex + 1) % c]} (from position {oldIndex + 1} to {newIndex + 1})");
            }

            Trace.WriteLine(string.Join(", ", contents));
            Trace.WriteLine();
        }
    }

    private long GetGroveCoords(long[] offsets, int[] indexes, long[] contents) {
        var originalZeroPosition = Array.IndexOf(offsets, 0);
        var newZeroPosition = indexes[originalZeroPosition];

        var c = offsets.Length;
        var _1000 = contents[(newZeroPosition + 1000 + c) % c];
        var _2000 = contents[(newZeroPosition + 2000 + c) % c];
        var _3000 = contents[(newZeroPosition + 3000 + c) % c];

        Trace.WriteLine($"1000th number is {_1000}");
        Trace.WriteLine($"2000th number is {_2000}");
        Trace.WriteLine($"3000th number is {_3000}");
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

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day20(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(1623178306, actual);
    }
}