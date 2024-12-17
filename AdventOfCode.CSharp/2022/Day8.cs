using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day8 : Solver {
    private readonly int[][] map;

    private readonly int h;

    private readonly int w;

    public Day8(string? input = null, ITestOutputHelper? outputHelper = null) : base(input) {
        map = Shared.Split(Input).Select(s => s.ToCharArray().Select(c => c - '0').ToArray()).ToArray();
        h = map.Length;
        w = map[0].Length;
    }

    protected override long SolvePartOne() {
        var visibilities = Enumerable.Range(0, h).Select(_ => new bool[w]).ToArray();

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

    protected override long SolvePartTwo() {
        var scenicScores = Enumerable.Range(0, h).Select(_ => new int[w]).ToArray();

        for (var y = 1; y < h - 1; y++) {
            for (var x = 1; x < w - 1; ++x) {
                scenicScores[y][x] = ScenicScore(x, y);
            }
        }

        return scenicScores.SelectMany(vr => vr).Max();
    }

    private int ScenicScore(int x0, int y0) {
        var threshold = map[y0][x0];
        var accumulator = 0;
        var score = 1;

        // up
        for (var y = y0 - 1; y >= 0; --y) {
            accumulator++;

            if (map[y][x0] >= threshold)
                break;
        }

        score *= accumulator;
        accumulator = 0;

        // down
        for (var y = y0 + 1; y < h; ++y) {
            accumulator++;

            if (map[y][x0] >= threshold)
                break;
        }

        score *= accumulator;
        accumulator = 0;

        // left
        for (var x = x0 - 1; x >= 0; --x) {
            accumulator++;

            if (map[y0][x] >= threshold)
                break;
        }

        score *= accumulator;
        accumulator = 0;

        // right
        for (var x = x0 + 1; x < w; ++x) {
            accumulator++;

            if (map[y0][x] >= threshold)
                break;
        }

        score *= accumulator;

        return score;
    }

    private const string? ExampleInput = @"
30373
25512
65332
33549
35390
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day8(ExampleInput, Output).SolvePartOne();
        Assert.Equal(21, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day8(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(8, actual);
    }
}