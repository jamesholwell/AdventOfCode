using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day24 : Solver {
    public Day24(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() => SolvePartOne(Parse(Input), 200000000000000L, 400000000000000L, Trace);

    private long SolvePartOne(Hailstone[] stones, long areaMin, long areaMax, ITestOutputHelper trace) {
        var accumulator = 0;

        var pairs = stones.Combinations();
        foreach (var (left, right) in pairs) {
            trace.WriteLine();
            trace.WriteLine($"Hailstone A: {left}");
            trace.WriteLine($"Hailstone B: {right}");

            decimal x1 = left.px;
            decimal x2 = left.px + left.vx;
            decimal y1 = left.py;
            decimal y2 = left.py + left.vy;

            decimal x3 = right.px;
            decimal x4 = right.px + right.vx;
            decimal y3 = right.py;
            decimal y4 = right.py + right.vy;

            var det = ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

            if (det == 0) {
                trace.WriteLine("Hailstones' paths are parallel; they never intersect.");
                continue;
            }

            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / det;
            var u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / det;

            if (t < 0 && u < 0) {
                trace.WriteLine("Hailstones' paths crossed in the past for both hailstones.");
                continue;
            }
            else if (t < 0) {
                trace.WriteLine("Hailstones' paths crossed in the past for hailstone A.");
                continue;
            }
            else if (u < 0) {
                trace.WriteLine("Hailstones' paths crossed in the past for hailstone B.");
                continue;
            }

            var ix = decimal.Round(left.px + t * left.vx, 3);
            var iy = decimal.Round(left.py + t * left.vy, 3);

            if (areaMin <= ix && ix <= areaMax && areaMin <= iy && iy <= areaMax) {
                trace.WriteLine($"Hailstones' paths will cross inside the test area (at x={ix}, y={iy}).");
                accumulator++;
            }
            else {
                trace.WriteLine($"Hailstones' paths will cross outside the test area (at x={ix}, y={iy}).");
            }
        }
        
        return accumulator;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private Hailstone[] Parse(string input) {
        var lines = Shared.Split(input);
        var stones = new Hailstone[lines.Length];

        for (var i = 0; i < lines.Length; ++i) {
            var parts = lines[i].Replace('@', ',').Split(',', StringSplitOptions.TrimEntries);
            stones[i] = new Hailstone
            {
                px = long.Parse(parts[0]),
                py = long.Parse(parts[1]),
                pz = long.Parse(parts[2]),
                vx = int.Parse(parts[3]),
                vy = int.Parse(parts[4]),
                vz = int.Parse(parts[5]),
            };
        }

        return stones;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")] // these are their puzzle names
    private record Hailstone {
        public long px;
        public long py;
        public long pz;
        public int vx;
        public int vy;
        public int vz;

        public override string ToString() {
            return $"{px}, {py}, {pz} @ {vx}, {vy}, {vz}";
        }
    }

    private const string? ExampleInput = @"
19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3
";

    private const string ExampleTrace = @"
Hailstone A: 19, 13, 30 @ -2, 1, -2
Hailstone B: 18, 19, 22 @ -1, -1, -2
Hailstones' paths will cross inside the test area (at x=14.333, y=15.333).

Hailstone A: 19, 13, 30 @ -2, 1, -2
Hailstone B: 20, 25, 34 @ -2, -2, -4
Hailstones' paths will cross inside the test area (at x=11.667, y=16.667).

Hailstone A: 19, 13, 30 @ -2, 1, -2
Hailstone B: 12, 31, 28 @ -1, -2, -1
Hailstones' paths will cross outside the test area (at x=6.2, y=19.4).

Hailstone A: 19, 13, 30 @ -2, 1, -2
Hailstone B: 20, 19, 15 @ 1, -5, -3
Hailstones' paths crossed in the past for hailstone A.

Hailstone A: 18, 19, 22 @ -1, -1, -2
Hailstone B: 20, 25, 34 @ -2, -2, -4
Hailstones' paths are parallel; they never intersect.

Hailstone A: 18, 19, 22 @ -1, -1, -2
Hailstone B: 12, 31, 28 @ -1, -2, -1
Hailstones' paths will cross outside the test area (at x=-6, y=-5).

Hailstone A: 18, 19, 22 @ -1, -1, -2
Hailstone B: 20, 19, 15 @ 1, -5, -3
Hailstones' paths crossed in the past for both hailstones.

Hailstone A: 20, 25, 34 @ -2, -2, -4
Hailstone B: 12, 31, 28 @ -1, -2, -1
Hailstones' paths will cross outside the test area (at x=-2, y=3).

Hailstone A: 20, 25, 34 @ -2, -2, -4
Hailstone B: 20, 19, 15 @ 1, -5, -3
Hailstones' paths crossed in the past for hailstone B.

Hailstone A: 12, 31, 28 @ -1, -2, -1
Hailstone B: 20, 19, 15 @ 1, -5, -3
Hailstones' paths crossed in the past for both hailstones.
";
    
    [Fact]
    public void ParsesExampleCorrectly() {
        var actual = Parse(ExampleInput!);

        Assert.Equal(5, actual.Length);
        Assert.Equal(19, actual[0].px);
        Assert.Equal(13, actual[0].py);
        Assert.Equal(30, actual[0].pz);
        Assert.Equal(-2, actual[0].vx);
        Assert.Equal(1, actual[0].vy);
        Assert.Equal(-2, actual[0].vz);
    }

    [Fact]
    public void TracesPartOneExampleCorrectly() {
        var buffer = new StringBuilderOutputHelper();
        SolvePartOne(Parse(ExampleInput!), 7, 27, buffer);
        Assert.Equal(ExampleTrace, buffer.ToString());
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = SolvePartOne(Parse(ExampleInput!), 7, 27, Trace);
        Assert.Equal(2, actual);
    }
}