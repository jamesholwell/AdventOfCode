using AdventOfCode.Core;
using Xunit;

namespace AdventOfCode.CSharp._2022;

public class Day8 : Solver {
    private readonly int[][] map;

    private readonly int h;

    private readonly int w;

    public Day8(string? input = null) : base(input) {
        map = Shared.Split(Input).Select(s => s.ToCharArray().Select(c => c - (int)'0').ToArray()).ToArray();
        h = map.Length;
        w = map[0].Length;
    }

    public override long SolvePartOne() {
        var visibilities = Enumerable.Range(0, h).Select(i => new bool[w]).ToArray();

        for (var y = 0; y < h; y++) {
            for (var x = 0; x < w; ++x) {
                visibilities[y][x] =
                    (x == 0 || y == 0 || x == w - 1 || y == h - 1) || 
                    SliceMax(x, 0, x, y - 1) < map[y][x]
                    || SliceMax(x, y + 1, x, h - 1) < map[y][x]
                    || SliceMax(0, y, x - 1, y) < map[y][x]
                    || SliceMax(x + 1, y, w - 1, y) < map[y][x];

            }
        }

        return visibilities.SelectMany(vr => vr).Count(b => b);
    }

    private int SliceMax(int x1, int y1, int x2, int y2) {
        var max = 0;

        for (var y = y1; y <= y2; y++) {
            for (var x = x1; x <= x2; ++x) {
                max = map[y][x] > max ? map[y][x] : max;
            }
        }

        return max;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
30373
25512
65332
33549
35390
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day8(ExampleInput).SolvePartOne();
        Assert.Equal(21, actual);
    }
}