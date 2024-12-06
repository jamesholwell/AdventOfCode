using AdventOfCode.Core;
using AdventOfCode.Core.Grid;
using AdventOfCode.Core.Output;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day6 : Solver {
    public Day6(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private static (int x, int y) GetStartPosition(int height, int width, char[,] grid) {
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            if (grid[y, x] == '^')
                return (x, y);

        throw new InvalidOperationException();
    }

    private static (int x, int y) NextPosition(int cx, int cy, Directions direction) {
        return direction switch {
            Directions.North => (cx, cy - 1),
            Directions.East => (cx + 1, cy),
            Directions.South => (cx, cy + 1),
            Directions.West => (cx - 1, cy),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private static Directions RightTurnFor(Directions direction) {
        return direction switch {
            Directions.North => Directions.East,
            Directions.East => Directions.South,
            Directions.South => Directions.West,
            Directions.West => Directions.North,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    protected override long SolvePartOne() {
        var grid = Input.SplitGrid();
        var width = grid.Width();
        var height = grid.Height();

        var (cx, cy) = GetStartPosition(height, width, grid);
        var direction = Directions.North;

        while (true) {
            grid[cy, cx] = 'X';
            
            var (nx, ny) = NextPosition(cx, cy, direction);
            
            if (0 <= nx && nx < width && 0 <= ny && ny < height) {
                if (grid[ny, nx] != '#') {
                    cx = nx;
                    cy = ny;
                    continue;
                }

                direction = RightTurnFor(direction);
                continue;
            }

            break;
        }

        Trace.WriteLine(grid.Render());
        return grid.Count(c => c == 'X');
    }

    protected override long SolvePartTwo() {
        var originalGrid = Input.SplitGrid();
        var width = originalGrid.Width();
        var height = originalGrid.Height();

        var (ox, oy) = GetStartPosition(height, width, originalGrid);
        var sum = 0;
            
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                if (originalGrid[y, x] != '.')
                    continue;

                var foundLoop = false;
                
                // initialize new grid for this obstacle
                var grid = new char[height, width];
                Array.Copy(originalGrid, grid, originalGrid.Length);
                grid[y, x] = 'O';
                
                // initialize where deja-vu grid
                var dejaGrid = new Directions[height, width];
                dejaGrid.Initialize(Directions.None);
                
                // set up start position
                int cx = ox, cy = oy;
                var direction = Directions.North;
                
                // walk the newGrid to see if we find a loop
                while (true) {
                    if (dejaGrid[cy, cx].HasFlag(direction)) {
                        foundLoop = true;
                        break;
                    }
                    
                    dejaGrid[cy, cx] |= direction;
                    
                    var (nx, ny) = NextPosition(cx, cy, direction);
            
                    if (0 <= nx && nx < width && 0 <= ny && ny < height) {
                        if (grid[ny, nx] == '#' || grid[ny, nx] == 'O') {
                            grid[cy, cx] = '+';
                            
                            direction = RightTurnFor(direction);
                            continue;
                        }

                        if (grid[ny, nx] == '.') {
                            grid[ny, nx] = direction switch {
                                Directions.North or Directions.South => '|',
                                Directions.East or Directions.West => '-',
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            
                            cx = nx;
                            cy = ny;
                            continue;
                        }
                        
                        if (grid[ny, nx] == '|' || grid[ny, nx] == '-') {
                            grid[ny, nx] = '+';
                        }

                        cx = nx;
                        cy = ny;
                        continue;
                    }

                    break;
                }

                if (foundLoop) {
                    sum++;
                    Trace.WriteLine(grid.Render());
                    Trace.WriteLine();
                }
            }
        }

        return sum;
    }
    [Flags]
    private enum Directions {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8
    }

    private const string? ExampleInput = @"
....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day6(ExampleInput, Output).SolvePartOne();
        Assert.Equal(41, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day6(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(6, actual);
    }
}