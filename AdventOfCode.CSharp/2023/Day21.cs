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
        var startingPosition = points.Single(p => p.value == 'S');
        var start = (startingPosition.x, startingPosition.y);
        
        var queue = new PriorityQueue<(int x, int y), int>();
        queue.Enqueue(start, 0);
        
        var reachablePositions = WalkPlots(queue, plots, w, h, steps);

        // render grid
        Render(steps, reachablePositions, start, plots, w, h);

        return reachablePositions.Count(p => p.Value % 2 == steps % 2);
    }

    private static Dictionary<(int x, int y), int> WalkPlots(PriorityQueue<(int x, int y), int> queue, HashSet<(int x, int y)> plots, int w, int h, int steps) {
        var seenPositions = new HashSet<(int x, int y)>();
        var reachablePositions = new Dictionary<(int x, int y), int>();
        
        while (queue.TryDequeue(out var position, out var t)) {
            if (t > steps)
                continue;

            reachablePositions[position] = t;
            
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

        return reachablePositions;
    }

    private void Render(int steps, Dictionary<(int x, int y), int> reachablePositions, (int x, int y) start, HashSet<(int x, int y)> plots, int w, int h) {
        var grid = new PointGrid<(int x, int y)>(reachablePositions.Keys, p => p.x, p => p.y);
            
        grid.Fill((x, y) =>
            start.x == x && start.y == y ? 'S' :
            reachablePositions.TryGetValue((x, y), out var i) ? (char)(i % 10 + '0') :
            plots.Contains((Maths.wrap(x, w), Maths.wrap(y, h))) ? '.' : '#');
            
        Trace.WriteLine("Walking distances");
        Trace.WriteLine(grid.Render());

        var possibleEndPositions = reachablePositions.Where(p => p.Value % 2 == steps % 2).Select(p => p.Key);
        grid.Fill((x, y) =>
            start.x == x && start.y == y ? 'S' :
            possibleEndPositions.Contains((x, y)) ? 'O' :
            plots.Contains((Maths.wrap(x, w), Maths.wrap(y, h))) ? '.' : '#');

        Trace.WriteLine();
        Trace.WriteLine("Possible end positions");
        Trace.WriteLine(grid.Render());
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
}