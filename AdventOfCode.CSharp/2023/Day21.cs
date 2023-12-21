using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day21 : Solver {
    public Day21(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() => SolvePartOne(64);

    private long SolvePartOne(int steps) {
        var points = Input.SplitPoints();
        var plots = new HashSet<(int x, int y)>(points.CoordinatesWhere(c => c == '.'));
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
            if (!seenPositions.Contains(up) && plots.Contains(up)) {
                seenPositions.Add(up);
                queue.Enqueue(up, t + 1);
            }

            var right = (position.x + 1, position.y);
            if (!seenPositions.Contains(right) && plots.Contains(right)) {
                seenPositions.Add(right);
                queue.Enqueue(right, t + 1 );
            }
            
            var down = (position.x, position.y + 1);
            if (!seenPositions.Contains(down) && plots.Contains(down)) {
                seenPositions.Add(down);
                queue.Enqueue(down, t + 1 );
            }
            
            var left = (position.x - 1, position.y);
            if (!seenPositions.Contains(left) && plots.Contains(left)) {
                seenPositions.Add(left);
                queue.Enqueue(left, t + 1);
            }
        }

        var reachable = steps % 2 == 0 ? reachableEvens : reachableOdds;
        
        // render grid
        var grid = new PointGrid<(int x, int y, char c)>(points, p => p.x, p => p.y, p => p.c);
        foreach (var point in reachable)
            grid.Set(point.x, point.y, 'O');
        Trace.WriteLine(grid.Render());
        
        return reachable.Count;
    }
    
    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

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
        var actual = new Day21(ExampleInput, Output).SolvePartOne(steps);
        Assert.Equal(expected, actual);
    }
}