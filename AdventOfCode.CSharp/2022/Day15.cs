using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day15 : Solver {
    private ITestOutputHelper io;

    public Day15(ITestOutputHelper io, string? input = null) : base(input) {
        this.io = io;
    }

    //public Day15(string? input = null) : base(input) { }

    private class Sensor {
        private readonly Tuple<int, int> position;

        private readonly Tuple<int, int> nearestBeacon;

        private readonly int exclusionRange;

        public Sensor(Tuple<int, int> position, Tuple<int, int> nearestBeacon) {
            this.position = position;
            this.nearestBeacon = nearestBeacon;
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

    public override long SolvePartOne() => Solve(2000000);

    public long Solve(int height) {
        var sensors = Parse();

        DrawMap(sensors);

        var exclusions = sensors.Select(p => p.ExclusionRangeAtHeight(height)).Where(r => r != null)
            .Cast<Tuple<int, int>>().ToArray();

        var minX = exclusions.MinBy(e => e.Item1)!.Item1;
        var maxX = exclusions.MaxBy(e => e.Item2)!.Item2;

        var coveredSpaces = 0;
        for (var x = minX; x < maxX; ++x) {
            if (exclusions.Any(e => e.Item1 <= x && x <= e.Item2)) coveredSpaces++;
        }

        return coveredSpaces;
    }

    private void DrawMap(Sensor[] sensors) {
        for (var y = -2; y < 22; ++y) {
            var exclusions = sensors.Select(p => p.ExclusionRangeAtHeight(y)).Where(r => r != null).Cast<Tuple<int, int>>()
                .ToArray();

            var buffer = new char[31];

            for (var x = -5; x < 26; ++x) {
                buffer[5 + x] = exclusions.Any(e => e.Item1 <= x && x <= e.Item2) ? '#' : '.';
            }

            io.WriteLine(new string(buffer));
        }
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

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
        var actual = new Day15(io, ExampleInput).Solve(10);
        Assert.Equal(26, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day15(io, ExampleInput).Solve(10);
        Assert.Equal(56000011, actual);
    }
}