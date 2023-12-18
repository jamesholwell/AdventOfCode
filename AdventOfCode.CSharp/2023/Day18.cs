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

    public override long SolvePartTwo() => Solve(Input, PartTwoParser);
    
    private long Solve(string input, Func<string, (Heading heading, int distance)> parser) {
        var instructions = Shared.Split(input).Select(parser);
        var points = ExecuteInstructions(instructions);
        
        // determine edges
        var edges = 
            points
                .Pairwise() // all edges
                .Select(p => {
                    // orient edges so that x1, y1 is upper-left
                    var minX = Math.Min(p.Item1.x, p.Item2.x);
                    var maxX = Math.Max(p.Item1.x, p.Item2.x);
                    var minY = Math.Min(p.Item1.y, p.Item2.y);
                    var maxY = Math.Max(p.Item1.y, p.Item2.y);

                    return (x1: minX, y1: minY, x2: maxX, y2: maxY);
                })
                .ToArray();

        long edgeLength = edges.Sum(p => p.x2 - p.x1 + p.y2 - p.y1);
        Trace.WriteLine($"Edge length is {edgeLength:N0}");
        
        var horizontalEdges = edges.Where(e => e.y1 == e.y2).ToArray();
        
        // determine candidate rectangles
        var xs = points.Select(p => p.x).Distinct().OrderBy(x => x).ToArray();
        var xPairs = xs.Select(x => (x, x)).Concat(
            xs.Pairwise().Where(p => p.Item1 + 1 < p.Item2).Select(p => (p.Item1 + 1, p.Item2 - 1)))
            .OrderBy(p => p.Item1).ToArray();
        
        var ys = points.Select(p => p.y).Distinct().OrderBy(x => x).ToArray();
        var yPairs = ys.Select(y => (y, y)).Concat(
            ys.Pairwise().Where(p => p.Item1 + 1 < p.Item2).Select(p => (p.Item1 + 1, p.Item2 - 1)))
            .OrderBy(p => p.Item1).ToArray();

        var rectangles =
            from xp in xPairs
            from yp in yPairs
            select (x1: xp.Item1, y1: yp.Item1, x2: xp.Item2, y2: yp.Item2);
        
        // determine if each rectangle is an interior area
        long accumulator = 0;
        foreach (var rectangle in rectangles) {
            var onEdge = edges.Any(e =>
                e.x1 <= rectangle.x1 && rectangle.x1 <= e.x2 && 
                e.y1 <= rectangle.y1 && rectangle.y1 <= e.y2);
            if (onEdge)
                continue; // already counted as part of the trench
            
            var edgesAbove = horizontalEdges.Count(e =>
                e.x1 < rectangle.x1 && rectangle.x2 <= e.x2
                                     && e.y1 < rectangle.y1);
            if (edgesAbove % 2 == 0)
                continue; // rectangle is exterior to the trench

            var area = (long)(1 + rectangle.x2 - rectangle.x1) * (1 + rectangle.y2 - rectangle.y1);
            Trace.WriteLine($"Rectangle ({rectangle.x1}, {rectangle.y1})-({rectangle.x2}, {rectangle.y2}) adds area {area:N0}");

            accumulator += area;
        }
        
        Trace.WriteLine($"Interior area is {accumulator:N0}");        
        Trace.WriteLine($"Total area is {edgeLength + accumulator:N0}");

        return edgeLength + accumulator;
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
    
    /*
     * 'Trivial example' of two rectangles
     *
     * Total area: 261, comprising:
     * 
     *   Edge length: 80
     *   Interior 1: 81
     *   Interior 2: 9
     *   Interior 3: 9
     *   Interior 4: 1
     *   Interior 5: 9
     *   Interior 6: 9
     *   Interior 7: 63
     * 
    
                     10
            ^ > > > > > > > > > >                    
            ^ 1 1 1 1 1 1 1 1 1 v                    
            ^ 1 1 1 1 1 1 1 1 1 v                    
            ^ 1 1 1 1 1 1 1 1 1 v                    
            ^ 1 1 1 1 1 1 1 1 1 v                    
        12  ^ 1 1 1 1 1 1 1 1 1 v  10                  
            ^ 1 1 1 1 1 1 1 1 1 v                    
            ^ 1 1 1 1 1 1 1 1 1 v                    
            ^ 1 1 1 1 1 1 1 1 1 v                    
            ^ 1 1 1 1 1 1 1 1 1 v        10              
            ^ 2 2 2 2 2 2 2 2 2 v > > > > > > > > > >
            ^ 3 3 3 3 3 3 3 3 3 4 5 5 5 5 5 5 5 5 5 v
            < < < < < < < < < < ^ 6 6 6 6 6 6 6 6 6 v
                     10         ^ 7 7 7 7 7 7 7 7 7 v
                                ^ 7 7 7 7 7 7 7 7 7 v
                                ^ 7 7 7 7 7 7 7 7 7 v 
                            8   ^ 7 7 7 7 7 7 7 7 7 v  10
                                ^ 7 7 7 7 7 7 7 7 7 v
                                ^ 7 7 7 7 7 7 7 7 7 v
                                ^ 7 7 7 7 7 7 7 7 7 v
                                < < < < < < < < < < v
                                          10
                                          
     */
    private const string TrivialExampleInput = @"
_ _ (#0000a0)
_ _ (#0000a1)
_ _ (#0000a0)
_ _ (#0000a1)
_ _ (#0000a2)
_ _ (#000083)
_ _ (#0000a2)
_ _ (#0000c3)
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
    public void ParsesPartTwoTrivialExample() {
        var actual = Shared.Split(TrivialExampleInput).Select(PartTwoParser).ToArray();

        Assert.Equal(Heading.Right, actual[0].heading);
        Assert.Equal(10, actual[0].distance);
        
        Assert.Equal(Heading.Down, actual[1].heading);
        Assert.Equal(10, actual[1].distance);
        
        Assert.Equal(Heading.Right, actual[2].heading);
        Assert.Equal(10, actual[2].distance);
        
        Assert.Equal(Heading.Down, actual[3].heading);
        Assert.Equal(10, actual[3].distance);

        Assert.Equal(Heading.Left, actual[4].heading);
        Assert.Equal(10, actual[4].distance);
        
        Assert.Equal(Heading.Up, actual[5].heading);
        Assert.Equal(8, actual[5].distance);
        
        Assert.Equal(Heading.Left, actual[6].heading);
        Assert.Equal(10, actual[6].distance);
        
        Assert.Equal(Heading.Up, actual[7].heading);
        Assert.Equal(12, actual[7].distance);
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
    public void SolvesPartOneExample() {
        var actual = new Day18(ExampleInput, Output).SolvePartOne();
        Assert.Equal(62, actual);
    }
    
    [Fact]
    public void SolvesPartOneExampleUsingPartTwo() {
        var actual = Solve(ExampleInput!, PartOneParser);
        Assert.Equal(62, actual);
    }

    [Fact]
    public void SolvesPartTwoTrivialExample() {
        var actual = new Day18(TrivialExampleInput, Output).SolvePartTwo();
        Assert.Equal(261, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day18(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(952408144115, actual);
    }
}