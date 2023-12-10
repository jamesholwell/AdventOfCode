using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day10 : Solver {
    public Day10(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var splitInput = Shared.Split(Input);
        var height = splitInput.Length + 2;
        var width = splitInput[0].Length + 2;
        
        var grid = new char[height, width];
        var distances = new int[height, width];
        int sx = -1, sy = -1;
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
        
        ShowGrid(grid);

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
        
        ShowDistances(distances);

        return Math.Max(maxDistance1, maxDistance2);
    }

    private void ShowGrid(char[,] grid) {
        return; // debugging only
        foreach (var y in Enumerable.Range(0, grid.GetLength(0)))
            Output.WriteLine(new string(Enumerable.Range(0, grid.GetLength(1)).Select(x => grid[y, x]).ToArray()));
    }
    
    private void ShowDistances(int[,] grid) {
        return; // debugging only
        Output.WriteLine();
        foreach (var y in Enumerable.Range(0, grid.GetLength(0)))
            Output.WriteLine(new string(Enumerable.Range(0, grid.GetLength(1)).Select(x => grid[y, x] == -1 ? '.' : (char)('0' + grid[y, x])).ToArray()));
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

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
}