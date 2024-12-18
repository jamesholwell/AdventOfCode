using AdventOfCode.Core;
using AdventOfCode.Core.Algorithms;
using AdventOfCode.Core.Grid;
using AdventOfCode.Core.Points;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day18(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    private static (int x, int y)[] Parse(string input) =>
        Shared.Split(input)
            .Select(s => s.Split(','))
            .Select(s => (int.Parse(s[0]), int.Parse(s[1])))
            .ToArray();

    private long SolvePartOne(string input, int numberOfBytes, int width, int height) {
        var coords = Parse(input);
        
        var grid = new char[height, width];
        grid.Initialize('.');

        for (var i = 0; i < numberOfBytes; i++) 
            grid[coords[i].y, coords[i].x] = '#';
        
        Trace.WriteLine(grid.Render());
        
        var traversableAddresses = new HashSet<(int, int)>(grid.Where(c => c == '.'));

        var d = Algorithm.Dijkstra(
            startNode: (0, 0),
            nodes: traversableAddresses,
            labelFunc: p => p,
            connectionFunc: TraversableNeighbors);
        
        return d[(width - 1, height - 1)];

        IEnumerable<((int x, int y), int d)> TraversableNeighbors((int x, int y) p) {
            if (traversableAddresses.Contains(p.Up())) yield return (p.Up(), 1);
            if (traversableAddresses.Contains(p.Right())) yield return (p.Right(), 1);
            if (traversableAddresses.Contains(p.Down())) yield return (p.Down(), 1);
            if (traversableAddresses.Contains(p.Left())) yield return (p.Left(), 1);
        }
    }

    protected override long SolvePartOne() => SolvePartOne(Input, 1024, 71, 71);

    protected override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string ExampleInput = 
        """
        5,4
        4,2
        4,5
        3,0
        2,1
        6,3
        2,4
        1,5
        0,6
        3,3
        2,6
        5,1
        1,2
        5,5
        2,5
        6,5
        1,4
        0,4
        6,4
        1,1
        6,1
        1,0
        0,5
        1,6
        2,0
        """;

    [Fact]
    public void SolvesPartOneExample() {
        var actual = SolvePartOne(ExampleInput, 12, 7, 7);
        Assert.Equal(22, actual);
    }
}