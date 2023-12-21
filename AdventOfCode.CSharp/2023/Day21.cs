using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day21 : Solver {
    public Day21(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() => SolvePartOne(64);

    private long SolvePartOne(int maximumSteps) {
        if (maximumSteps % 2 != 0)
            throw new NotImplementedException("Algorithm only considers evenly reachable spaces");
        
        var points = Input.SplitPoints();
        var plots = new HashSet<(int x, int y)>(points.CoordinatesWhere(c => c == '.'));
        var reachablePositions = new HashSet<(int x, int y)>();

        var queue = new PriorityQueue<(int x, int y), int>();
        var seenPositions = new HashSet<(int x, int y)>();
        
        var startingPosition = points.Single(p => p.value == 'S');
        var start = (startingPosition.x, startingPosition.y);
        queue.Enqueue(start, 0);
        seenPositions.Add(start);
        
        while (queue.TryDequeue(out var position, out var t)) {
            if (t > maximumSteps)
                continue;

            if (t % 2 == 0)
                reachablePositions.Add(position);
            
            var up = (position.x, position.y - 1);
            if (!seenPositions.Contains(up) && plots.Contains(up)) {
                seenPositions.Add(up);
                queue.Enqueue(position with { y = position.y - 1 }, t + 1);
            }

            var right = (position.x + 1, position.y);
            if (!seenPositions.Contains(right) && plots.Contains(right)) {
                seenPositions.Add(right);
                queue.Enqueue(position with { x = position.x + 1}, t + 1 );
            }
            
            var down = (position.x, position.y + 1);
            if (!seenPositions.Contains(down) && plots.Contains(down)) {
                seenPositions.Add(down);
                queue.Enqueue(position with { y = position.y + 1}, t + 1 );
            }
            
            var left = (position.x - 1, position.y);
            if (!seenPositions.Contains(left) && plots.Contains(left)) {
                seenPositions.Add(left);
                queue.Enqueue(position with { x = position.x - 1}, t + 1);
            }
        }
        
        var grid = new PointGrid<(int x, int y, char c)>(points, p => p.x, p => p.y, p => p.c);
        
        foreach (var point in reachablePositions)
            grid.Set(point.x, point.y, 'O');
        
        Trace.WriteLine(grid.Render());
        
        return reachablePositions.Count;
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

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day21(ExampleInput, Output).SolvePartOne(6);
        Assert.Equal(16, actual);
    }
}