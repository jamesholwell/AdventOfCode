using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day15(string? input = null, ITestOutputHelper? outputHelper = null) : Solver(input, outputHelper) {
    private class Sensor {
        private readonly Tuple<int, int> position;

        private readonly int exclusionRange;

        public Sensor(Tuple<int, int> position, Tuple<int, int> nearestBeacon) {
            this.position = position;
            this.exclusionRange = ManhattanDistance(position, nearestBeacon);
        }

        public Tuple<int, int>? ExclusionRangeAtHeight(int y) {
            var offset = Math.Abs(y - position.Item2);
            var width = exclusionRange - offset;
            return width >= 0 ? Tuple.Create(position.Item1 - width, position.Item1 + width) : null;
        }

        private int ManhattanDistance(Tuple<int, int> a, Tuple<int, int> b) {
            return Math.Abs(a.Item1 - b.Item1) + Math.Abs(b.Item2 - a.Item2);
        }
    }

    private Sensor[] Parse() {
        return Shared.Split(Input).Select(s => {
            var parts = s.Split(": ");
            return new Sensor(
                ParseCoordinate(parts[0].Substring("Sensor at ".Length)),
                ParseCoordinate(parts[1].Substring("closest beacon is at ".Length)));
        }).ToArray();
    }

    private Tuple<int, int> ParseCoordinate(string s) {
        var parts = s.Split(", ");

        return Tuple.Create(int.Parse(parts[0].Substring("x=".Length)), int.Parse(parts[1].Substring("y=".Length)));
    }

    protected override long SolvePartOne() => SolvePartOneInner(2000000);

    protected override long SolvePartTwo() => SolvePartTwoInner(4000000);

    public long SolvePartOneInner(int height) {
        var sensors = Parse();

        DrawMap(sensors);

        var exclusions = sensors.Select(p => p.ExclusionRangeAtHeight(height)).Where(r => r != null)
            .Cast<Tuple<int, int>>().ToArray();

        var minX = exclusions.MinBy(e => e.Item1)?.Item1;
        var maxX = exclusions.MaxBy(e => e.Item2)?.Item2;

        var coveredSpaces = 0;
        for (var x = minX; x < maxX; ++x) {
            if (exclusions.Any(e => e.Item1 <= x && x <= e.Item2)) coveredSpaces++;
        }

        return coveredSpaces;
    }

    public long SolvePartTwoInner(int max) {
        var sensors = Parse();

        for (var y = 0; y < max; ++y) {
            if (y % 100000 == 0) Trace.WriteLine($"y = {y}");
            var exclusions = sensors.Select(p => p.ExclusionRangeAtHeight(y)).Where(r => r != null)
                .Cast<Tuple<int, int>>().ToArray();

            var range = new[] { Tuple.Create(0, max) };

            foreach (var e in exclusions) {
                range = range.SelectMany(r => Subtract(r, e)).ToArray();
                if (range.Length == 0) break;
            }

            if (range.Length == 0) continue;

            for (var x = 0; x < max; ++x) {
                if (!exclusions.Any(e => e.Item1 <= x && x <= e.Item2))
                    return 4000000L * x + y;
            }
        }

        return 0;
    }

    private IEnumerable<Tuple<int, int>> Subtract(Tuple<int, int> a, Tuple<int, int> b) {
        if (a.Item1 < b.Item1)
            yield return Tuple.Create(a.Item1, Math.Min(a.Item2, b.Item1));

        if (b.Item2 < a.Item2)
            yield return Tuple.Create(Math.Max(a.Item1, b.Item2 + 1), a.Item2);
    }

    private void DrawMap(Sensor[] sensors, int maxX = 20, int maxY = 20) {
        for (var y = -2; y < 22; ++y) {
            var exclusions = sensors.Select(p => p.ExclusionRangeAtHeight(y)).Where(r => r != null)
                .Cast<Tuple<int, int>>()
                .ToArray();

            var buffer = new string(' ', 31).ToCharArray();

            for (var x = -5; x < 26; ++x) {
                if (x < 0 || x > maxX || y < 0 || y > maxY) continue;
                buffer[5 + x] = exclusions.Any(e => e.Item1 <= x && x <= e.Item2) ? '#' : '.';
            }

            Trace.WriteLine(new string(buffer));
        }
    }

    private const string? ExampleInput = @"
Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day15(ExampleInput, Output).SolvePartOneInner(10);
        Assert.Equal(26, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day15(ExampleInput, Output).SolvePartTwoInner(20);
        Assert.Equal(56000011, actual);
    }
}