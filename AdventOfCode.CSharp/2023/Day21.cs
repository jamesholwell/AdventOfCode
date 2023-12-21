using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day21 : Solver {
    public Day21(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() => SolvePartOne(64);

    public long SolvePartOne(int maximumSteps) {
        var positions = Input.SplitPoints();
        var startingPosition = positions.Single(p => p.value == 'S');
        var plots = new HashSet<(int x, int y)>(positions.CoordinatesWhere(c => c == '.' || c == 'S'));

        var queue = new Queue<(int x, int y, int t)>();
        var seenPositions = new HashSet<(int x, int y)>();
        var start = (startingPosition.x, startingPosition.y, 0);
        queue.Enqueue(start);
        seenPositions.Add((startingPosition.x, startingPosition.y));
        var lastT = 0;
        
        while (queue.TryDequeue(out var position)) {
            if (position.t > lastT) {
                lastT = position.t;
                seenPositions.Clear();
                Trace.WriteLine($"t = {lastT}; Queue has {queue.Count} elements");
            }
            seenPositions.Add((position.x, position.y));
            
            if (position.t >= maximumSteps)
                continue;

            var up = (position.x, position.y - 1);
            if (!seenPositions.Contains(up) && plots.Contains(up))
                queue.Enqueue(position with { y = position.y - 1, t = position.t + 1 });
            
            var right = (position.x + 1, position.y);
            if (!seenPositions.Contains(right) && plots.Contains(right))
                queue.Enqueue(position with { x = position.x + 1, t = position.t + 1 });
            
            var down = (position.x, position.y + 1);
            if (!seenPositions.Contains(down) && plots.Contains(down))
                queue.Enqueue(position with { y = position.y + 1, t = position.t + 1 });
            
            var left = (position.x - 1, position.y);
            if (!seenPositions.Contains(left) && plots.Contains(left))
                queue.Enqueue(position with { x = position.x - 1, t = position.t + 1});
        }
        
        // var minX = plots.Min(p => p.x);
        // var maxX = plots.Max(p => p.x);
        // var minY = plots.Min(p => p.y);
        // var maxY = plots.Max(p => p.y);
        //
        // var grid = new char[1 + maxY - minY, 1 + maxX - minX];
        // grid.Initialize('#');
        //
        // foreach (var point in plots)
        //     grid[point.y - minY, point.x - minX] = '.';
        //
        // foreach (var point in seenPositions)
        //     if (point.t == maximumSteps)
        //         grid[point.y - minY, point.x - minX] = 'O';
        //
        // Output.WriteLine(grid.Render());
        
        return seenPositions.Count;
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