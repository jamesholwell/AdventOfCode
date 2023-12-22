using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day21 : Solver {
    public Day21(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() => Solve(64);

    public override long SolvePartTwo() => Solve(26501365);

    private long Solve(int steps) {
        var points = Input.SplitPoints();
        var w = points.Max(p => p.x) + 1;
        var h = points.Max(p => p.y) + 1;
        
        var plots = new HashSet<(int x, int y)>(points.CoordinatesWhere(c => c == '.' || c == 'S'));
        var reachableEvens = new HashSet<(int x, int y)>();
        var reachableOdds = new HashSet<(int x, int y)>();

        var queue = new PriorityQueue<(int x, int y), int>();
        var seenPositions = new HashSet<(int x, int y)>();
        
        var startingPosition = points.Single(p => p.value == 'S');
        var start = (startingPosition.x, startingPosition.y);
        queue.Enqueue(start, 0);
        seenPositions.Add(start);
        
        while (queue.TryDequeue(out var position, out var t)) {
            if (t > steps)
                continue;
            
            if (t % 2 == 0)
                reachableEvens.Add(position);
            else
                reachableOdds.Add(position);
            
            var up = (position.x, position.y - 1);
            if (!seenPositions.Contains(up) && plots.Contains((Maths.wrap(position.x, w), Maths.wrap(position.y - 1, h)))) {
                seenPositions.Add(up);
                queue.Enqueue(up, t + 1);
            }

            var right = (position.x + 1, position.y);
            if (!seenPositions.Contains(right) && plots.Contains((Maths.wrap(position.x + 1, w), Maths.wrap(position.y, h)))) {
                seenPositions.Add(right);
                queue.Enqueue(right, t + 1 );
            }
            
            var down = (position.x, position.y + 1);
            if (!seenPositions.Contains(down) && plots.Contains((Maths.wrap(position.x, w), Maths.wrap(position.y + 1, h)))) {
                seenPositions.Add(down);
                queue.Enqueue(down, t + 1 );
            }
            
            var left = (position.x - 1, position.y);
            if (!seenPositions.Contains(left) && plots.Contains((Maths.wrap(position.x - 1, w), Maths.wrap(position.y, h)))) {
                seenPositions.Add(left);
                queue.Enqueue(left, t + 1);
            }
        }

        var reachable = steps % 2 == 0 ? reachableEvens : reachableOdds;

        // render grid
        if (steps < 51) {
            var grid = new PointGrid<(int x, int y)>(reachable, p => p.x, p => p.y);

            grid.Fill((x, y) =>
                start.x == x && start.y == y ? 'S' :
                reachable.Contains((x, y)) ? 'O' :
                plots.Contains((Maths.wrap(x, w), Maths.wrap(y, h))) ? '.' : '#');

            Trace.WriteLine(grid.Render());
        }

        return reachable.Count;
    }
    
    private long SolveDiagonal(int steps) {
        var points = Input.SplitPoints();
        var sz = points.Max(p => p.x) + 1;
        Debug.Assert(sz == points.Max(p => p.y) + 1);

        // check that it has the magical properties required
        if ((steps % sz) != (sz - 1) / 2)
            return -1;
        
        var plots = new HashSet<(int x, int y)>(points.CoordinatesWhere(c => c == '.' || c == 'S'));
        var reachablePositions = new Dictionary<(int x, int y), int>();

        var queue = new PriorityQueue<(int x, int y), int>();

        var startingPosition = points.Single(p => p.value == 'S');
        var start = (startingPosition.x, startingPosition.y);
        queue.Enqueue(start, 0);
        reachablePositions.Add(start, 0);
        
        while (queue.TryDequeue(out var position, out var t)) {
            if (t > steps)
                continue;
            
            var up = (position.x, position.y - 1);
            if (!reachablePositions.ContainsKey(up) && plots.Contains(up)) {
                reachablePositions.Add(up, t + 1);
                queue.Enqueue(up, t + 1);
            }

            var right = (position.x + 1, position.y);
            if (!reachablePositions.ContainsKey(right) && plots.Contains(right)) {
                reachablePositions.Add(right, t + 1);
                queue.Enqueue(right, t + 1);
            }
            
            var down = (position.x, position.y + 1);
            if (!reachablePositions.ContainsKey(down) && plots.Contains(down)) {
                reachablePositions.Add(down, t + 1);
                queue.Enqueue(down, t + 1);
            }
            
            var left = (position.x - 1, position.y);
            if (!reachablePositions.ContainsKey(left) && plots.Contains(left)) {
                reachablePositions.Add(left, t + 1);
                queue.Enqueue(left, t + 1);
            }
        }

        // model the expanding diamond!
        
        // this is the 'number of rings' around the central piece
        var div = (long)steps / sz;
        
        // this is the 'number of steps remaining' after moving to the centre of the edges
        var rem = (long)steps % sz;

        var evenTiles = (1 + div) * (1 + div);
        var evenTileSpaces = reachablePositions.Count(p => p.Value % 2 == 0);
        
        var oddTiles = div * div;
        var oddTileSpaces = reachablePositions.Count(p => p.Value % 2 == 1);

        var edgeTiles = div;
        var edgeAdditionalSpaces =reachablePositions.Count(p => p.Value % 2 == 1 && p.Value > rem);
        
        var pointyTiles = (div + 1);
        var pointyOvercountedSpaces = reachablePositions.Count(p => p.Value % 2 == 0 && p.Value > rem);

        return evenTiles * evenTileSpaces 
               + oddTiles * oddTileSpaces 
               + edgeTiles * edgeAdditionalSpaces
               - pointyTiles * pointyOvercountedSpaces;
    }

    private const string? ExampleInput = @"
...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........
";
    
    private const string? TrivialInput = @"
.....
.....
..S..
.....
.....
";
    
    private const string? DiagonalInput = @"
.............
.........#.#.
.##........#.
....#........
....#...#....
.............
......S......
.............
..#.....#....
.......#.#...
.##.....#..#.
.##.......##.
.............
";
    
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 6)]
    [InlineData(6, 16)]
    public void SolvesPartOneExamples(int steps, int expected) {
        var actual = new Day21(ExampleInput, Output).Solve(steps);
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData(6, 16)]
    [InlineData(10, 50)]
    [InlineData(50, 1594)]
    [InlineData(100, 6536)]
    [InlineData(500, 167004)]
    //[InlineData(1000, 668697)] - works but is slow
    //[InlineData(5000, 16733044)] - works but is slow
    public void SolvesPartTwoExamples(int steps, int expected) {
        var actual = new Day21(ExampleInput, Output).Solve(steps);
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData(2)]
    [InlineData(7)]
    [InlineData(12)]
    [InlineData(23 * 5 + 2)]
    public void SolvesTrivialExampleDiagonally(int steps) {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var expected = new Day21(TrivialInput, Output).Solve(steps);
        stopwatch.Stop();
        var bfTime = stopwatch.ElapsedTicks;
        
        stopwatch.Reset();
        stopwatch.Start();
        var actual = new Day21(TrivialInput, Output).SolveDiagonal(steps);
        stopwatch.Stop();
        var algoTime = stopwatch.ElapsedTicks;
        
        Assert.Equal(expected, actual);
        Output.WriteLine(
            $"Brute force took {bfTime:N0} ticks, algorithm took {algoTime:N0} ticks ({((float)bfTime / algoTime):N1}x)");
    }
    
    [Theory]
    [InlineData(6)]
    [InlineData(19)]
    [InlineData(32)]
    [InlineData(23 * 13 + 6)]
    public void SolvesDiagonalExample(int steps) {
        var expected = new Day21(DiagonalInput, Output).Solve(steps);
        var actual = new Day21(DiagonalInput, Output).SolveDiagonal(steps);
        Assert.Equal(expected, actual);
    }
}