using AdventOfCode.Core;
using AdventOfCode.Core.Grid;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day8 : Solver {
    public Day8(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    protected override long SolvePartOne() {
        var grid = Input.SplitGrid();
        var height = grid.Height();
        var width = grid.Width();

        var outputGrid = grid.Duplicate();
        var antiNodes = new char[height, width];
        antiNodes.Initialize('.');

        foreach (var antennas in GetAntennaGroups(height, width, grid)) {
            foreach (var (left, right) in antennas.Value.Combinations()) {
                var vector = (dx: right.x - left.x, dy: right.y - left.y);

                var antiNode1 = (x: left.x - vector.dx, y: left.y - vector.dy);
                if (0 <= antiNode1.x && antiNode1.x < width && 0 <= antiNode1.y && antiNode1.y < height) {
                    antiNodes[antiNode1.y, antiNode1.x] = '#';
                    if (outputGrid[antiNode1.y, antiNode1.x] == '.')
                        outputGrid[antiNode1.y, antiNode1.x] = '#';
                }

                var antiNode2 = (x: right.x + vector.dx, y: right.y + vector.dy);
                if (0 <= antiNode2.x && antiNode2.x < width && 0 <= antiNode2.y && antiNode2.y < height) {
                    antiNodes[antiNode2.y, antiNode2.x] = '#';
                    
                    if (outputGrid[antiNode2.y, antiNode2.x] == '.')
                        outputGrid[antiNode2.y, antiNode2.x] = '#';
                }
            }
        }

        Trace.WriteLine(outputGrid.Render());
        return antiNodes.Count(c => c == '#');
    }

    protected override long SolvePartTwo() {
        var grid = Input.SplitGrid();
        var height = grid.Height();
        var width = grid.Width();

        var outputGrid = grid.Duplicate();
        var antiNodes = new char[height, width];
        antiNodes.Initialize('.');

        foreach (var antennas in GetAntennaGroups(height, width, grid)) {
            foreach (var (left, right) in antennas.Value.Combinations()) {
                var vector = (dx: right.x - left.x, dy: right.y - left.y);
                
                var antiNode1 = left;
                while (0 <= antiNode1.x && antiNode1.x < width && 0 <= antiNode1.y && antiNode1.y < height) {
                    antiNodes[antiNode1.y, antiNode1.x] = '#';
                    if (outputGrid[antiNode1.y, antiNode1.x] == '.')
                        outputGrid[antiNode1.y, antiNode1.x] = '#';
                    
                    antiNode1 = (x: antiNode1.x - vector.dx, y: antiNode1.y - vector.dy);
                }
                
                var antiNode2 = (x: left.x + vector.dx, y: left.y + vector.dy);
                while (0 <= antiNode2.x && antiNode2.x < width && 0 <= antiNode2.y && antiNode2.y < height) {
                    antiNodes[antiNode2.y, antiNode2.x] = '#';
                    
                    if (outputGrid[antiNode2.y, antiNode2.x] == '.')
                        outputGrid[antiNode2.y, antiNode2.x] = '#';
                    
                    antiNode2 = (x: antiNode2.x + vector.dx, y: antiNode2.y + vector.dy);
                }
            }
        }

        Trace.WriteLine(outputGrid.Render());
        return antiNodes.Count(c => c == '#');
    }
    
    private static Dictionary<char, List<(int x, int y)>> GetAntennaGroups(int height, int width, char[,] grid) {
        var antennaGroups = new Dictionary<char, List<(int x, int y)>>();
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                if (grid[y, x] == '.') 
                    continue;
                
                if (!antennaGroups.ContainsKey(grid[y, x]))
                    antennaGroups[grid[y, x]] = new List<(int x, int y)>() { (x, y) };
                else
                    antennaGroups[grid[y, x]].Add((x, y));
            }
        }

        return antennaGroups;
    }

    [Fact]
    public void SolvesSimplePartOneExample() {
        const string simpleInput = @"
..........
..........
..........
....a.....
..........
.....a....
..........
..........
..........
..........
";
        
        var actual = new Day8(simpleInput, Output).SolvePartOne();
        Assert.Equal(2, actual);
    }

    [Fact]
    public void SolvesSimplePartTwoExample() {
        const string simpleInput = @"
T.........
...T......
.T........
..........
..........
..........
..........
..........
..........
..........
";
        
        var actual = new Day8(simpleInput, Output).SolvePartTwo();
        Assert.Equal(9, actual);
    }
    
    private const string? ExampleInput = @"
............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day8(ExampleInput, Output).SolvePartOne();
        Assert.Equal(14, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day8(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(34, actual);
    }
}