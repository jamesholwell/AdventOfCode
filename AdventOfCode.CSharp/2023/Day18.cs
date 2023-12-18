using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day18 : Solver {
    public Day18(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var instructions = Shared.Split(Input).Select(PartOneParser);

        var points = ExecuteInstructions(instructions, true);

        var minX = points.Min(p => p.x);
        var maxX = points.Max(p => p.x);
        var minY = points.Min(p => p.y);
        var maxY = points.Max(p => p.y);

        var grid = new char[1 + maxY - minY, 1 + maxX - minX];
        grid.Initialize('.');
        foreach (var point in points)
            grid[point.y - minY, point.x - minX] = '#';
        
        Trace.WriteLine(grid.Render());
        Trace.WriteLine();

        var fillPoints = new List<(int x, int y)>() { (1 - minX, 1 - minY) };
        var nextPoints = new List<(int x, int y)>();
        var height = grid.Height();
        var width = grid.Width();

        while (fillPoints.Any()) {
            foreach (var point in fillPoints) {
                // up
                if (point.y > 0 && grid[point.y - 1, point.x] == '.') {
                    grid[point.y - 1, point.x] = '#';
                    nextPoints.Add(point with { y = point.y - 1 });
                }
                
                // right
                if (point.x < width - 1 && grid[point.y, point.x + 1] == '.') {
                    grid[point.y, point.x + 1] = '#';
                    nextPoints.Add(point with { x = point.x + 1 });
                }
                
                // down
                if (point.y < height - 1 && grid[point.y + 1, point.x] == '.') {
                    grid[point.y + 1, point.x] = '#';
                    nextPoints.Add(point with { y = point.y + 1 });
                }
                
                // left
                if (point.x > 0 && grid[point.y, point.x - 1] == '.') {
                    grid[point.y, point.x - 1] = '#';
                    nextPoints.Add(point with { x = point.x - 1 });
                }
            }

            fillPoints.Clear();
            fillPoints.AddRange(nextPoints);
            nextPoints.Clear();
        }

        Trace.WriteLine(grid.Render());

        return grid.Count(c => c == '#');
    }

    public override long SolvePartTwo() {
        var instructions = Shared.Split(Input).Select(PartTwoParser);
        var points = ExecuteInstructions(instructions);

        return points.Count;
    }

    private (Heading heading, int distance) PartOneParser(string line) {
        var parts = line.Split(' ', 3);
        
        var heading = parts[0] switch {
            "U" => Heading.Up,
            "R" => Heading.Right,
            "D" => Heading.Down,
            "L" => Heading.Left,
            _ => throw new InvalidOperationException()
        };

        return (heading, int.Parse(parts[1]));
    }
    
    private (Heading heading, int distance) PartTwoParser(string line) {
        var parts = line.Split(' ', 3);
        var hexPart = parts[2];

        var heading = hexPart[7] switch {
            '3' => Heading.Up,
            '0' => Heading.Right,
            '1' => Heading.Down,
            '2' => Heading.Left,
            _ => throw new InvalidOperationException()
        };
        
        var distance = Convert.ToInt32(hexPart.Substring(2, 5), 16);

        return (heading, distance);
    }

    private static List<(int x, int y)> ExecuteInstructions(IEnumerable<(Heading heading, int distance)> instructions, bool walkEachPoint = false) {
        var points = new List<(int x, int y)>();
        int x = 0, y = 0;
        
        if (!walkEachPoint)
            points.Add((x, y));
        
        foreach (var instruction in instructions) {
            var dx = instruction.heading switch {
                Heading.Right => 1,
                Heading.Left => -1,
                _ => 0
            };

            var dy = instruction.heading switch {
                Heading.Up => -1,
                Heading.Down => 1,
                _ => 0
            };

            if (walkEachPoint) {
                for (var i = 0; i < instruction.distance; ++i) {
                    points.Add((x, y));
                    x += dx;
                    y += dy;
                }
            }
            else {
                x += dx * instruction.distance;
                y += dy * instruction.distance;
                points.Add((x, y));
            }
        }

        return points;
    }

    private enum Heading {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }
    
    private const string? ExampleInput = @"
R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)
";
    
    [Fact]
    public void ParsesPartOneExample() {
        var actual = Shared.Split(ExampleInput!).Select(PartOneParser).ToArray();

        Assert.Equal(Heading.Right, actual[0].heading);
        Assert.Equal(6, actual[0].distance);
        
        Assert.Equal(Heading.Down, actual[1].heading);
        Assert.Equal(5, actual[1].distance);
        
        Assert.Equal(Heading.Left, actual[2].heading);
        Assert.Equal(2, actual[2].distance);
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day18(ExampleInput, Output).SolvePartOne();
        Assert.Equal(62, actual);
    }
    
    [Fact]
    public void ParsesPartTwoExample() {
        var actual = Shared.Split(ExampleInput!).Select(PartTwoParser).ToArray();

        Assert.Equal(Heading.Right, actual[0].heading);
        Assert.Equal(461937, actual[0].distance);
        
        Assert.Equal(Heading.Down, actual[1].heading);
        Assert.Equal(56407, actual[1].distance);
        
        Assert.Equal(Heading.Right, actual[2].heading);
        Assert.Equal(356671, actual[2].distance);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day18(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(952408144115, actual);
    }
}