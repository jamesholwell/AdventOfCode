using AdventOfCode.Core;
using AdventOfCode.Core.Grid;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day14 : Solver {
    public Day14(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private static (int x, int y, int vx, int vy)[] Parse(string input) {
        return
            Shared.Split(input)
                .Select(l => l.Split(['p', '=', ' ', 'v', ','],
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Select(a => (int.Parse(a[0]), int.Parse(a[1]), int.Parse(a[2]), int.Parse(a[3])))
                .ToArray();
    }

    private static void UpdatePositions((int x, int y, int vx, int vy)[] robots, int width, int height) {
        for (var i = 0; i < robots.Length; ++i)
            robots[i] = robots[i] with {
                x = Mathematics.wrap(robots[i].x + robots[i].vx, width),
                y = Mathematics.wrap(robots[i].y + robots[i].vy, height)
            };
    }

    private static string Render((int x, int y, int vx, int vy)[] robots, int width, int height) {
        var grid = new char[height, width];
        grid.Initialize(' ');

        foreach (var robot in robots)
            grid[robot.y, robot.x] =
                grid[robot.y, robot.x] == ' ' ? '1' : (char)(1 + grid[robot.y, robot.x]);

        return grid.Render();
    }

    private long SafetyFactor((int x, int y, int vx, int vy)[] robots, int width, int height) {
        var halfWidth = (width - 1) / 2;
        var halfHeight = (height - 1) / 2;

        var upLeft = 0;
        var upRight = 0;
        var downLeft = 0;
        var downRight = 0;
        
        foreach (var robot in robots) {
            if (robot.x < halfWidth && robot.y < halfHeight)
                upLeft++;
            else if (robot.x > halfWidth && robot.y < halfHeight)
                upRight++;
            else if (robot.x < halfWidth && robot.y > halfHeight)
                downLeft++;
            else if (robot.x > halfWidth && robot.y > halfHeight)
                downRight++;
        }
        
        return upLeft * upRight * downLeft * downRight;
    }

    protected override long SolvePartOne() => SolvePartOne(101, 103);

    private long SolvePartOne(int width, int height) {
        var robots = Parse(Input);

        for (var t = 0; t < 100; ++t) 
            UpdatePositions(robots, width, height);
        
        Trace.WriteLine(Render(robots, width, height));
        Trace.WriteLine(string.Empty);
        
        return SafetyFactor(robots, width, height);
    }

    protected override long SolvePartTwo() {
        var robots = Parse(Input);

        var set = new HashSet<(int x, int y)>();
        var t = 0;
        
        while (t++ < 10_000) {
            UpdatePositions(robots, 101, 103);

            set.Clear();
            foreach (var robot in robots) 
                set.Add((robot.x, robot.y));
            
            var foundTree = set.Any(
                /*
                 * a Christmas tree looks roughly like: 
                 * 
                 *    1
                 *   111
                 *  11111
                 *    1
                 *
                 * look for the top
                 * 
                 */
                p =>
                     // 111 line
                     set.Contains((p.x - 1, p.y + 1))
                     && set.Contains((p.x, p.y + 1))
                     && set.Contains((p.x + 1, p.y + 1))
                     // 11111 line
                     && set.Contains((p.x - 2, p.y + 2))
                     && set.Contains((p.x - 1, p.y + 2))
                     && set.Contains((p.x, p.y + 2))
                     && set.Contains((p.x + 2, p.y + 2))
                     && set.Contains((p.x + 2, p.y + 2))
                     );

            if (!foundTree) 
                continue;
            
            Trace.WriteLine(Render(robots, 101, 103));
            return t;
        }

        throw new InsufficientlyFestiveException();
    }

    private class InsufficientlyFestiveException : InvalidOperationException {}

    private const string? ExampleInput = @"
p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3
";

    [Fact]
    public void SingleRobotMovesAsExpected() {
        const string? input = "p=2,4 v=2,-3";
        const int width = 11;
        const int height = 7;
        var robots = Parse(input);

        Trace.WriteLine("Initial state:");
        Trace.WriteLine(Render(robots, width, height));
        Trace.WriteLine(string.Empty);

        for (var t = 1; t < 6; ++t) {
            UpdatePositions(robots, width, height);
            
            Trace.WriteLine(t == 1 ? "After 1 second:" : $"After {t} seconds:");
            Trace.WriteLine(Render(robots, width, height));
            Trace.WriteLine(string.Empty);
        }

        Assert.Single(robots);
        Assert.Equal(1, robots[0].x);
        Assert.Equal(3, robots[0].y);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day14(ExampleInput, Output).SolvePartOne(11, 7);
        Assert.Equal(12, actual);
    }
}