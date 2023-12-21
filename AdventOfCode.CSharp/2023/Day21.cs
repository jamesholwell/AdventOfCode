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