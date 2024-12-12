// ReSharper disable StringLiteralTypo

using AdventOfCode.Core;
using AdventOfCode.Core.Grid;
using AdventOfCode.Core.Points;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day12 : Solver {
    public Day12(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private List<HashSet<(int x, int y)>> ExtractRegions() {
        var grid = Input.SplitGrid();
        var regions = new List<HashSet<(int x, int y)>>();

        var unexplored = Input.SplitCoordinates(_ => true).ToHashSet();
        while (unexplored.Count > 0) {
            var unexploredCoordinate = unexplored.First();
            var plant = grid.At(unexploredCoordinate);

            var region = new HashSet<(int, int)>();
            var next = new Queue<(int, int)>();
            next.Enqueue(unexploredCoordinate);

            while (next.TryDequeue(out var coordinate)) {
                if (!region.Add(coordinate)) continue;
                unexplored.Remove(coordinate);

                if (grid.MaybeAt(coordinate.Up()) == plant)
                    next.Enqueue(coordinate.Up());

                if (grid.MaybeAt(coordinate.Right()) == plant)
                    next.Enqueue(coordinate.Right());

                if (grid.MaybeAt(coordinate.Down()) == plant)
                    next.Enqueue(coordinate.Down());

                if (grid.MaybeAt(coordinate.Left()) == plant)
                    next.Enqueue(coordinate.Left());
            }

            regions.Add(region);
        }

        return regions;
    }

    private static int PerimeterOf(HashSet<(int x, int y)> region) =>
        region.Sum(r =>
            (region.Contains(r.Up()) ? 0 : 1)
            + (region.Contains(r.Right()) ? 0 : 1)
            + (region.Contains(r.Down()) ? 0 : 1)
            + (region.Contains(r.Left()) ? 0 : 1)
        );

    private static int NumberOfSides(HashSet<(int x, int y)> region) {
        var upFences = new HashSet<(int x, int y)>();
        var rightFences = new HashSet<(int x, int y)>();
        var downFences = new HashSet<(int x, int y)>();
        var leftFences = new HashSet<(int x, int y)>();

        // figure out the fence locations
        foreach (var plot in region) {
            if (!region.Contains(plot.Up()))
                upFences.Add(plot);

            if (!region.Contains(plot.Right()))
                rightFences.Add(plot);

            if (!region.Contains(plot.Down()))
                downFences.Add(plot);

            if (!region.Contains(plot.Left()))
                leftFences.Add(plot);
        }

        // figure out the runs
        var upRuns = upFences.Count(f => upFences.Contains(f.Right()));
        var rightRuns = rightFences.Count(f => rightFences.Contains(f.Down()));
        var downRuns = downFences.Count(f => downFences.Contains(f.Left()));
        var leftRuns = leftFences.Count(f => leftFences.Contains(f.Up()));

        return upFences.Count + rightFences.Count + downFences.Count + leftFences.Count
               - upRuns - rightRuns - downRuns - leftRuns;
    }

    protected override long SolvePartOne() => ExtractRegions().Sum(region => region.Count * PerimeterOf(region));

    protected override long SolvePartTwo() => ExtractRegions().Sum(region => region.Count * NumberOfSides(region));

    [Fact]
    public void SolvesSimplePartOneExample() {
        const string simpleExample =
            """
            AAAA
            BBCD
            BBCC
            EEEC
            """;
        
        var actual = new Day12(simpleExample, Output).SolvePartOne();
        Assert.Equal(4 * 10 + 4 * 8 + 4 * 10 + 3 * 8 + 1 * 4, actual);
    }

    [Fact]
    public void SolvesSimplePartTwoExample1() {
        const string simpleExample =
            """
            AAAA
            BBCD
            BBCC
            EEEC
            """;
        
        var actual = new Day12(simpleExample, Output).SolvePartTwo();
        Assert.Equal(80, actual);
    }

    [Fact]
    public void SolvesSimplePartTwoExample2() {
        const string simpleExample =
            """
            OOOOO
            OXOXO
            OOOOO
            OXOXO
            OOOOO
            """;
        
        var actual = new Day12(simpleExample, Output).SolvePartTwo();
        Assert.Equal(436, actual);
    }

    [Fact]
    public void SolvesSimplePartTwoExample3() {
        const string simpleExample =
            """
            EEEEE
            EXXXX
            EEEEE
            EXXXX
            EEEEE
            """;
        
        var actual = new Day12(simpleExample, Output).SolvePartTwo();
        Assert.Equal(236, actual);
    }

    [Fact]
    public void SolvesSimplePartTwoExample4() {
        const string simpleExample =
            """
            AAAAAA
            AAABBA
            AAABBA
            ABBAAA
            ABBAAA
            AAAAAA
            """;
        
        var actual = new Day12(simpleExample, Output).SolvePartTwo();
        Assert.Equal(368, actual);
    }

    private const string? ExampleInput =
        """
        RRRRIICCFF
        RRRRIICCCF
        VVRRRCCFFF
        VVRCCCJFFF
        VVVVCJJCFE
        VVIVCCJJEE
        VVIIICJJEE
        MIIIIIJJEE
        MIIISIJEEE
        MMMISSJEEE
        """;

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day12(ExampleInput, Output).SolvePartOne();
        Assert.Equal(1930, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day12(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(1206, actual);
    }
}