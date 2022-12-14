using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day14(string? input = null, ITestOutputHelper? outputHelper = null) : Solver(input, outputHelper) {
    private static Tuple<int, int>[][] Parse(string input) =>
        Shared.Split(input).Select(l => l.Split(" -> ").Select(PairToTuple).ToArray()).ToArray();

    private static Tuple<int, int> PairToTuple(string p) {
        var parts = p.Split(",");
        return Tuple.Create(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private long Solve(bool isPartTwo = false) {
        var paths = Parse(Input);

        var flatPaths = paths.SelectMany(t => t).ToArray();
        var xs = flatPaths.Select(t => t.Item1).ToArray();
        var ys = flatPaths.Select(t => t.Item2).ToArray();
        var minX = xs.Min();
        var maxX = xs.Max();
        var maxY = ys.Max();
        
        var padding = isPartTwo ? maxY - 1 : 2;
        var offsetX = minX - padding + (isPartTwo ? 2 : 0);
        var w = maxX - minX + padding + padding;
        var h = maxY + 5;

        var grid = Enumerable.Range(0, h).Select(_ => Enumerable.Range(offsetX, w).Select(_ => '.').ToArray())
            .ToArray();

        foreach (var path in paths) {
            foreach (var wall in path.Zip(path.Skip(1), Tuple.Create)) {
                var x1 = Math.Min(wall.Item1.Item1, wall.Item2.Item1);
                var x2 = Math.Max(wall.Item1.Item1, wall.Item2.Item1);
                var y1 = Math.Min(wall.Item1.Item2, wall.Item2.Item2);
                var y2 = Math.Max(wall.Item1.Item2, wall.Item2.Item2);

                for (var y = y1; y <= y2; ++y)
                for (var x = x1; x <= x2; ++x)
                    grid[y][x - offsetX] = '#';
            }
        }

        // foreach (var row in grid) 
        //     Trace.WriteLine(new string(row));

        var particles = 0;
        while (grid[0][500 - offsetX] == '.') {
            particles++;
            var particleX = 500 - offsetX;
            var particleY = 0;

            while (true) {
                if (particleY > maxY) {
                    if (!isPartTwo) {
                        foreach (var row in grid)
                            Trace.WriteLine(new string(row));
                        
                        return particles - 1; // this one escaped forever
                    }
                    
                    grid[particleY][particleX] = 'o';
                    break;
                }

                if (grid[particleY + 1][particleX] == '.') {
                    particleY++;
                    continue;
                }

                if (grid[particleY + 1][particleX - 1] == '.') {
                    particleY++;
                    particleX--;
                    continue;
                }

                if (grid[particleY + 1][particleX + 1] == '.') {
                    particleY++;
                    particleX++;
                    continue;
                }

                grid[particleY][particleX] = 'o';
                break;
            }
        }

        foreach (var row in grid) 
            Trace.WriteLine(new string(row));

        return particles;
    }

    protected override long SolvePartOne() => Solve();

    protected override long SolvePartTwo() => Solve(true);

    private const string? ExampleInput = @"
498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day14(ExampleInput, Output).SolvePartOne();
        Assert.Equal(24, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day14(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(93, actual);
    }
}