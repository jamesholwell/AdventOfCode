using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day22 : Solver {
    /*
     * map is oriented [x, y] and is 0-indexed to create a 1-space buffer of "off map" spaces
     *
     * facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^)
     *
     * turn is right = +1, left = -1, no turn = 0 (last instruction)
     */
    public Day22(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private (char[,] map, (int x, int y, int f) position, (int forward, int turn)[] instructions) Parse(string input) {
        var parts = input.SplitBy("\n\n");

        var lines = parts[0].Split("\n");
        var height = lines.Length + 2;
        var width = lines.Max(l => l.Length) + 2;

        var map = new char[width, height];
        for (var y = 0; y < height; ++y) {
            var lineLength = y == 0 || y == height - 1 ? 0 : lines[y - 1].Length;
            for (var x = 0; x < width; ++x) {
                map[x, y] = x == 0 || x == width - 1 || x > lineLength ? ' ' : lines[y - 1][x - 1];
            }
        }

        (int x, int y, int f) position = (0, 0, 0);
        for (var x = 1; x < width - 1; ++x) {
            if (map[x, 1] == '.') {
                position = (x, 1, 0);
                break;
            }
        }

        var instructions = new List<(int forward, int turn)>();

        var rightRuns = parts[1].Split('R');
        for (var rightIndex = 0; rightIndex < rightRuns.Length; rightIndex++) {
            var isLastRightRun = rightIndex == rightRuns.Length - 1;
            var leftRuns = rightRuns[rightIndex].Split('L');
            for (var leftIndex = 0; leftIndex < leftRuns.Length; leftIndex++) {
                var isLastLeftRun = leftIndex == leftRuns.Length - 1;
                var facing = isLastLeftRun && isLastRightRun ? 0 : isLastLeftRun ? 1 : -1;
                instructions.Add((int.Parse(leftRuns[leftIndex]), facing));
            }
        }

        return (map, position, instructions.ToArray());
    }

    private string Render(char[,] map, (int x, int y, int f)? position = null) {
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        var buffer = new StringBuilder(height * (width + 2));

        for (var y = 1; y < height - 1; ++y) {
            for (var x = 1; x < width - 1; ++x) {
                if (position.HasValue && x == position.Value.x && y == position.Value.y) {
                    buffer.Append(facings[position.Value.f]);
                    continue;
                }
                
                buffer.Append(map[x, y]);
            }

            buffer.Append(Environment.NewLine);
        }

        return buffer.ToString();
    }

    public override long SolvePartOne() {
        var (map, position, instructions) = Parse(Input);

        Output.WriteLine(Render(map, position));

        return 0;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string ExampleInput = @"
        ...#
        .#..
        #...
        ....
...#.......#
........#...
..#....#....
..........#.
        ...#....
        .....#..
        .#......
        ......#.

10R5L5R10L4R5L5
";

    private const string facings = ">v<^";

    [Fact]
    public void ParsesMapCorrectly() {
        var (map, _, _) = Parse(ExampleInput);
        const string expected = @"        ...#    
        .#..    
        #...    
        ....    
...#.......#    
........#...    
..#....#....    
..........#.    
        ...#....
        .....#..
        .#......
        ......#.
";
        Assert.Equal(expected.ReplaceLineEndings(), Render(map));
    }

    [Fact]
    public void ParsesPositionCorrectly() {
        var (_, position, _) = Parse(ExampleInput);
        Assert.Equal(9, position.x);
        Assert.Equal(1, position.y);
        Assert.Equal(0, position.f);
    }

    [Fact]
    public void ParsesInstructionsCorrectly() {
        var (_, _, instructions) = Parse(ExampleInput);
        Assert.Equal(7, instructions.Length);
        Assert.Equal((10, 1), instructions[0]);
        Assert.Equal((5, -1), instructions[1]);
        Assert.Equal((5, 1), instructions[2]);
        Assert.Equal((10, -1), instructions[3]);
        Assert.Equal((4, 1), instructions[4]);
        Assert.Equal((5, -1), instructions[5]);
        Assert.Equal((5, 0), instructions[6]);
    }


    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day22(ExampleInput, Output).SolvePartOne();
        Assert.Equal(0, actual);
    }
}