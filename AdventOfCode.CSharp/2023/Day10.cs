using System.Transactions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day10 : Solver {
    private readonly char[,] grid;
    private readonly int[,] distances;
    private readonly int sx;
    private readonly int sy;

    public Day10(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) {
        var splitInput = Shared.Split(Input);
        var height = splitInput.Length + 2;
        var width = splitInput[0].Length + 2;
        
        grid = new char[height, width];
        distances = new int[height, width];
        sx = -1;
        sy = -1;
        for (var iy = 0; iy < height; ++iy) {
            for (var ix = 0; ix < width; ++ix) {
                if (iy == 0 || ix == 0 || iy == height - 1 || ix == width - 1)
                    grid[iy, ix] = '.';
                else
                    grid[iy, ix] = splitInput[iy - 1][ix - 1];

                distances[iy, ix] = -1;
                if (grid[iy, ix] == 'S') {
                    sx = ix;
                    sy = iy;
                    distances[iy, ix] = 0;
                }
            }
        }
    }

    public override long SolvePartOne() {
        var exitUp = grid[sy - 1, sx] == '|' || grid[sy - 1, sx] == '7' || grid[sy - 1, sx] == 'F';
        var exitDown = grid[sy + 1, sx] == '|' || grid[sy + 1, sx] == 'J' || grid[sy + 1, sx] == 'L';
        var exitRight = grid[sy, sx + 1] == '-' || grid[sy, sx + 1] == '7' || grid[sy, sx + 1] == 'J';
        var exitLeft = grid[sy, sx - 1] == '-' || grid[sy, sx - 1] == 'F' || grid[sy, sx - 1] == 'L';

        grid[sy, sx] =
            (exitUp && exitDown) ? '|' :
            (exitLeft && exitRight) ? '-' :
            (exitUp && exitRight) ? 'L' :
            (exitUp && exitLeft) ? 'J' :
            (exitDown && exitLeft) ? '7' :
            (exitDown && exitRight) ? 'F' :
            throw new InvalidOperationException("Could not determine S");
        
        ShowGrid();

        int x1 = sx, x2 = sx, y1 = sy, y2 = sy, maxDistance1 = 0, maxDistance2 = 0;
        bool bored1 = false, bored2 = false;

        while (!bored1 || !bored2) {
            if (!bored1) {
                maxDistance1++;
                
                // up
                if (distances[y1 - 1, x1] == -1 &&
                    (grid[y1, x1] == '|' || grid[y1, x1] == 'J' || grid[y1, x1] == 'L')) {

                    distances[y1 - 1, x1] = distances[y1, x1] + 1;
                    y1 -= 1;
                }
                // down
                else if (distances[y1 + 1, x1] == -1 &&
                         (grid[y1, x1] == '|' || grid[y1, x1] == '7' || grid[y1, x1] == 'F')) {
                    
                    distances[y1 + 1, x1] = distances[y1, x1] + 1;
                    y1 += 1;
                }
                // right
                else if (distances[y1, x1 + 1] == -1 &&
                         (grid[y1, x1] == '-' || grid[y1, x1] == 'L' || grid[y1, x1] == 'F')) {
                    
                    distances[y1, x1 + 1] = distances[y1, x1] + 1;
                    x1 += 1;
                }
                // left
                else if (distances[y1, x1 - 1] == -1 &&
                         (grid[y1, x1] == '-' || grid[y1, x1] == 'J' || grid[y1, x1] == '7')) {
                    
                    distances[y1, x1 - 1] = distances[y1, x1] + 1;
                    x1 -= 1;
                }
                else {
                    bored1 = true;
                    maxDistance1--;
                }
            }
            
            if (!bored2) {
                maxDistance2++;
                
                // up
                if (distances[y2 - 1, x2] == -1 &&
                    (grid[y2, x2] == '|' || grid[y2, x2] == 'J' || grid[y2, x2] == 'L')) {

                    distances[y2 - 1, x2] = distances[y2, x2] + 1;
                    y2 -= 1;
                }
                // down
                else if (distances[y2 + 1, x2] == -1 &&
                         (grid[y2, x2] == '|' || grid[y2, x2] == '7' || grid[y2, x2] == 'F')) {
                    
                    distances[y2 + 1, x2] = distances[y2, x2] + 1;
                    y2 += 1;
                }
                // right
                else if (distances[y2, x2 + 1] == -1 &&
                         (grid[y2, x2] == '-' || grid[y2, x2] == 'L' || grid[y2, x2] == 'F')) {
                    
                    distances[y2, x2 + 1] = distances[y2, x2] + 1;
                    x2 += 1;
                }
                // left
                else if (distances[y2, x2 - 1] == -1 &&
                         (grid[y2, x2] == '-' || grid[y2, x2] == 'J' || grid[y2, x2] == '7')) {
                    
                    distances[y2, x2 - 1] = distances[y2, x2] + 1;
                    x2 -= 1;
                }
                else {
                    bored2 = true;
                    maxDistance2--;
                }
            }
        }
        
        ShowDistances();

        return Math.Max(maxDistance1, maxDistance2);
    }

    public override long SolvePartTwo() {
        SolvePartOne();

        var height = grid.GetLength(0);
        var width = grid.GetLength(1);
        var points = new char[height, width];

        for (var y = 0; y < height; ++y) {
            for (var x = 0; x < width; ++x) {
                if (distances[y, x] == -1)
                    grid[y, x] = '.';
            }
        }
        
        var accumulator = 0;
        
        for (var y = 0; y < height; ++y) {
            for (var x = 0; x < width; ++x) {
                points[y, x] = grid[y, x];

                // already on the loop, ignore
                if (distances[y, x] > -1)
                    continue;
                
                // send a ray to the edge
                var intersections = 0;
                
                for (var trace = x; trace >= 0; --trace) {
                    if (grid[y, trace] == '|' || grid[y, trace] == 'L' || grid[y, trace] == 'J')
                        intersections++;
                }
                
                // odd intersections is in the curve
                if (intersections % 2 == 1) {
                    points[y, x] = '#';
                    accumulator++;
                }
            }
        }

        ShowInterior(points); 
        
        return accumulator;
    }

    private void ShowGrid() {
        //return; // debugging only
        foreach (var y in Enumerable.Range(0, grid.GetLength(0)))
            Output.WriteLine(new string(Enumerable.Range(0, grid.GetLength(1)).Select(x => grid[y, x]).ToArray()));
    }

    private void ShowDistances() {
        //return; // debugging only
        Output.WriteLine();
        foreach (var y in Enumerable.Range(0, distances.GetLength(0)))
            Output.WriteLine(new string(Enumerable.Range(0, distances.GetLength(1)).Select(x => distances[y, x] == -9 ? '.' : (char)('0' + distances[y, x] % 10)).ToArray()));
    }
    
    private void ShowInterior(char[,] points) {
        Output.WriteLine();
        foreach (var y in Enumerable.Range(0, points.GetLength(0)))
            Output.WriteLine(new string(Enumerable.Range(0, points.GetLength(1)).Select(x => points[y, x]).ToArray()));
    }

    private const string? ExampleInput1 = @"
-L|F7
7S-7|
L|7||
-L-J|
L|-JF
";
    
    private const string? ExampleInput2 = @"
..F7.
.FJ|.
SJ.L7
|F--J
LJ...
";

    private const string? ExampleInput3 = @"
...........
.S-------7.
.|F-----7|.
.||.....||.
.||.....||.
.|L-7.F-J|.
.|..|.|..|.
.L--J.L--J.
...........
";
    
    private const string? ExampleInput4 = @"
.F----7F7F7F7F-7....
.|F--7||||||||FJ....
.||.FJ||||||||L7....
FJL7L7LJLJ||LJ.L-7..
L--J.L7...LJS7F-7L7.
....F-J..F7FJ|L7L7L7
....L7.F7||L7|.L7L7|
.....|FJLJ|FJ|F7|.LJ
....FJL-7.||.||||...
....L---J.LJ.LJLJ...
";

    private const string? ExampleInput5 = @"
FF7FSF7F7F7F7F7F---7
L|LJ||||||||||||F--J
FL-7LJLJ||||||LJL-77
F--JF--7||LJLJ7F7FJ-
L---JF-JLJ.||-FJLJJ7
|F|F-JF---7F7-L7L|7|
|FFJF7L7F-JF7|JL---7
7-L-JL7||F7|L7F-7F7|
L.L7LFJ|||||FJL7||LJ
L7JLJL-JLJLJL--JLJ.L
";
    
    [Fact]
    public void SolvesPartOneExample1() {
        var actual = new Day10(ExampleInput1, Output).SolvePartOne();
        Assert.Equal(4, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample2() {
        var actual = new Day10(ExampleInput2, Output).SolvePartOne();
        Assert.Equal(8, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample3() {
        var actual = new Day10(ExampleInput3, Output).SolvePartTwo();
        Assert.Equal(4, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample4() {
        var actual = new Day10(ExampleInput4, Output).SolvePartTwo();
        Assert.Equal(8, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample5() {
        var actual = new Day10(ExampleInput5, Output).SolvePartTwo();
        Assert.Equal(10, actual);
    }
}