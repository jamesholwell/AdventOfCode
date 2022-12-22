using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day22 : Solver {
    /*
     * map is oriented [y][x] and is 0-indexed to create a 1-space buffer of "off map" spaces
     *
     * facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^)
     *
     * turn is right = +1, left = -1, no turn = 0 (last instruction)
     */
    public Day22(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private (string[] map, (int x, int y, int f) position, (int forward, int turn)[] instructions) Parse(string input) {
        var parts = input.SplitBy("\n\n");

        var lines = parts[0].Split("\n");
        var height = lines.Length + 2;
        var width = lines.Max(l => l.Length) + 2;

        var map = new string[height];
        for (var y = 0; y < height; ++y) {
            map[y] = y == 0 || y == height - 1
                ? new string(' ', width)
                : " " + lines[y - 1] + new string(' ', width - 1 - lines[y - 1].Length);
        }

        (int x, int y, int f) position = (0, 0, 0);
        for (var x = 1; x < width - 1; ++x) {
            if (map[1][x] == '.') {
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

    private string Render(string[] map, int?[,]? positionHistory = null) {
        var width = map[0].Length;
        var height = map.Length;
        var buffer = new StringBuilder(height * (width + 2));

        for (var y = 1; y < height - 1; ++y) {
            for (var x = 1; x < width - 1; ++x) {
                if (positionHistory?[x, y] != null) {
                    buffer.Append(facings[positionHistory[x, y]!.Value]);
                    continue;
                }
                
                buffer.Append(map[y][x]);
            }

            buffer.Append(Environment.NewLine);
        }

        return buffer.ToString();
    }

    public override long SolvePartOne() {
        var (map, position, instructions) = Parse(Input);

        Trace.WriteLine(Render(map));

        var (finalPosition, positionHistory) = Walk(map, position, instructions);

        Output.WriteLine(Render(map, positionHistory));
        
        return 1000 * finalPosition.y + 4 * finalPosition.x + finalPosition.f;
    }

    private ((int x, int y, int f), int?[,]) Walk(string[] map, (int x, int y, int f) initialPosition, (int forward, int turn)[] instructions) {
        var width = map[0].Length;
        var height = map.Length;

        var positionHistory = new int?[width, height];
        var position = initialPosition;
        positionHistory[position.x, position.y] = position.f;

        foreach (var instruction in instructions) {
            for (var i = 0; i < instruction.forward; ++i) {
                var newX = position.x + (position.f == 0 ? 1 : position.f == 2 ? -1 : 0);
                var newY = position.y + (position.f == 1 ? 1 : position.f == 3 ? -1 : 0);
                
                if (map[newY][newX] == ' ') {
                    switch (position.f) {
                        case 0:
                            newX = 1;
                            while (map[newY][newX] == ' ') newX++;
                            break;

                        case 1:
                            newY = 1;
                            while (map[newY][newX] == ' ') newY++;
                            break;

                        case 2:
                            newX = width - 1;
                            while (map[newY][newX] == ' ') newX--;
                            break;

                        case 3:
                            newY = height - 1;
                            while (map[newY][newX] == ' ') newY--;
                            break;
                    }
                }

                if (map[newY][newX] == '#') {
                    continue;
                }

                position = (newX, newY, position.f);
                positionHistory[position.x, position.y] = position.f;
            }

            position.f = ((position.f + instruction.turn) % 4 + 4) % 4;
            positionHistory[position.x, position.y] = position.f;
        }

        return (position, positionHistory);
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
    public void WalksMapCorrectly() {
        var (map, position, instructions) = Parse(ExampleInput);
        var (finalPosition, positionHistory) = Walk(map, position, instructions);

        const string expected = @"        >>v#    
        .#v.    
        #.v.    
        ..v.    
...#...v..v#    
>>>v...>#.>>    
..#v...#....    
...>>>>v..#.    
        ...#....
        .....#..
        .#......
        ......#.
";
        var actual = Render(map, positionHistory);
        Output.WriteLine(actual);

        Assert.Equal(6, finalPosition.y);
        Assert.Equal(8, finalPosition.x);
        Assert.Equal(0, finalPosition.f);
        Assert.Equal(expected.ReplaceLineEndings(), actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day22(ExampleInput, Output).SolvePartOne();
        Assert.Equal(6032, actual);
    }
}