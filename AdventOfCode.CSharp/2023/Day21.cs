using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day21 : Solver {
    public Day21(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var (plots, s, w, h) = Initialize(Input);
        var reachable = BruteForceSolve(plots, 64, (s.x, s.y), w, h);
        
        // render visualisation
        var grid = new PointGrid<(int x, int y)>(reachable, p => p.x, p => p.y);

        grid.Fill((x, y) =>
            s.x == x && s.y == y ? 'S' :
            reachable.Contains((x, y)) ? 'O' :
            plots.Contains((Maths.wrap(x, w), Maths.wrap(y, h))) ? '.' : '#');

        Trace.WriteLine(grid.Render());
            
        // return the answer
        return reachable.Count();
    }

    public override long SolvePartTwo() {
        var (plots, s, w, h) = Initialize(Input);
        
        // heuristic only works for squares
        // ReSharper disable once InlineTemporaryVariable - for readability!
        var sz = w;
        Debug.Assert(sz == h);
        
        // heuristic only works for n-traversals + 1 half
        var r = (sz - 1) / 2;
        Debug.Assert((26501365 % sz) == r);
        
        Output.WriteLine("Beginning drift detection:");

        var variances = new List<long>();
        for (var i = 0; i < 5; ++i) {
            var steps = sz * i + r;
            var bf = BruteForceSolve(plots, steps, s, sz, sz).Count;
            var algo = HeuristicSolve(plots, steps, s, sz);

            var variance = bf - algo;
            variances.Add(variance);
            Output.WriteLine($"n = {i} ({steps} steps): bf = {bf}, algo = {algo}, variance = {variance}");
        }

        var pairwiseVariance = variances.Pairwise().Select(vp => vp.Item2 - vp.Item1).ToArray();
        if (pairwiseVariance.Distinct().Count() > 1) {
            Output.WriteLine($"... drift detection failed: {string.Join(", ", variances)}");
            Output.WriteLine();
            return -1;
        }

        var initialVariance = variances.First();
        var drift = pairwiseVariance.First();
        Output.WriteLine($"... detected drift of {drift}");
        Output.WriteLine();
        
        Output.WriteLine("Beginning test:");
        var testSteps = sz * 10 + r;
        var bfTest = BruteForceSolve(plots, testSteps, s, sz, sz).Count;
        var algoTest = HeuristicSolve(plots, testSteps, s, sz) + initialVariance + 10 * drift;
        var varianceTest = algoTest - bfTest;
        Output.WriteLine($"n = 10 ({testSteps} steps): bf = {bfTest}, algo = {algoTest}, variance = {varianceTest}");

        if (varianceTest != 0) {
            Output.WriteLine("... test run failed");
            Output.WriteLine();
            return -1;
        }

        var n = 26501365 / sz;
        return HeuristicSolve(plots, 26501365, s, sz) + initialVariance + n * drift;
    }

    private (HashSet<(int x, int y)> plots, (int x, int y) startingPosition, int w, int h) Initialize(string input) {
        var points = input.SplitPoints();
        var plots = new HashSet<(int x, int y)>(points.CoordinatesWhere(c => c == '.' || c == 'S'));
        var startingPosition = points.Single(p => p.value == 'S');
        var w = points.Max(p => p.x) + 1;
        var h = points.Max(p => p.y) + 1;
        return (plots, (startingPosition.x, startingPosition.y), w, h);
    }

    private IReadOnlySet<(int x, int y)> BruteForceSolve(IReadOnlySet<(int x, int y)> plots, int steps,
        (int x, int y) start, int w, int h) {
        var reachableEvens = new HashSet<(int x, int y)>();
        var reachableOdds = new HashSet<(int x, int y)>();

        var queue = new PriorityQueue<(int x, int y), int>();
        var seenPositions = new HashSet<(int x, int y)>();
        
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

        return steps % 2 == 0 ? reachableEvens : reachableOdds;
    }
    
    private long HeuristicSolve(IReadOnlySet<(int x, int y)> plots, int steps, (int x, int y) start, int sz) {
        // initialize the queue of points to explore and the path distances
        var reachablePositions = new Dictionary<(int x, int y), int>();
        var queue = new PriorityQueue<(int x, int y), int>();
        queue.Enqueue(start, 0);
        reachablePositions.Add(start, 0);
        
        // we don't wrap, because we just extrapolate based on the first one
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
        
        // for our algorithm, work out the size of the central region
        var div = (long)steps / sz;
        
        // this is the 'number of steps remaining' after moving to the centre of the edge tile
        var rem = (long)steps % sz;

        // count up the central region
        var evenTiles = (1 + div) * (1 + div);
        var evenTileSpaces = reachablePositions.Count(p => p.Value % 2 == 0);
        
        var oddTiles = div * div;
        var oddTileSpaces = reachablePositions.Count(p => p.Value % 2 == 1);

        // count up the tiles on the long edges
        var edgeTiles = div;
        var edgeAdditionalSpaces =reachablePositions.Count(p => p.Value % 2 == 1 && p.Value > rem);
        
        // subtract the over-counting on the pointy edges
        var pointyTiles = (div + 1);
        var pointyOverCountedSpaces = reachablePositions.Count(p => p.Value % 2 == 0 && p.Value > rem);

        // return our algorithmic result
        return evenTiles * evenTileSpaces 
               + oddTiles * oddTileSpaces 
               + edgeTiles * edgeAdditionalSpaces
               - pointyTiles * pointyOverCountedSpaces;
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
    
    private const string TrivialInput = @"
.....
.....
..S..
.....
.....
";
    
    private const string DiagonalInput = @"
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
        var (plots, s, w, h) = Initialize(ExampleInput!);
        var actual = BruteForceSolve(plots, steps, s, w, h);
        Assert.Equal(expected, actual.Count);
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
        var (plots, s, w, h) = Initialize(ExampleInput!);
        var actual = BruteForceSolve(plots, steps, s, w, h);
        Assert.Equal(expected, actual.Count);
    }
    
    [Theory]
    [InlineData(2)]
    [InlineData(7)]
    [InlineData(12)]
    [InlineData(23 * 5 + 2)]
    public void SolvesTrivialExampleDiagonally(int steps) {
        var (plots, s, w, h) = Initialize(TrivialInput);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var expected = BruteForceSolve(plots, steps, s, w, h).Count;
        stopwatch.Stop();
        var bfTime = stopwatch.ElapsedTicks;
        
        stopwatch.Reset();
        stopwatch.Start();
        var actual = HeuristicSolve(plots, steps, s, w);
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
        var (plots, s, w, h) = Initialize(DiagonalInput);
        var expected = BruteForceSolve(plots, steps, s, w, h).Count;
        var actual = HeuristicSolve(plots, steps, s, w);

        Assert.Equal(expected, actual);
    }
}