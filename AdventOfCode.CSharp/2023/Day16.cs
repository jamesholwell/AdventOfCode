using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day16 : Solver {
    private char[,] grid = null!;
    private Direction[,] map = null!;
    private bool[,] energised = null!;
    private int height;
    private int width;
    
    public Day16(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        Initialize(Input);

        // render initial grid
        Trace.WriteHeading("Contraption layout:");
        Trace.WriteLine(grid.Render());
        
        CalculateLightPath();

        // render beam of light grid
        Trace.WriteHeading("Light path:");
        Trace.WriteLine(map.Render(MapRenderer));
        
        // render energized grid
        Trace.WriteHeading("Energized tiles:");
        Trace.WriteLine(energised.Render(EnergizedRenderer));
        
        // generate result
        return energised.Count();
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private void Initialize(string input) {
        // initialise grids
        grid = input.SplitGrid();
        height = grid.Height();
        width = grid.Width();
        map = new Direction[height, width];
        energised = new bool[height, width];
    }

    private void CalculateLightPath() {
        // initialise particles
        var photons = new List<(int x, int y, Direction d)> { (-1, 0, Direction.Right) };
        var nextPhotons = new List<(int x, int y, Direction)>();
        var energizedNewTile = true;
        
        // trace the light paths
        while (energizedNewTile) {
            energizedNewTile = false;
            
            foreach (var photon in photons) {
                if (photon.x == -1) {
                    // initial photon starts "off the map" - skip this step
                    energizedNewTile = true;
                } 
                else {
                    // energize the tile
                    energised[photon.y, photon.x] = true;

                    // update maps (and see if we've passed through this tile in this direction before)
                    if (!map[photon.y, photon.x].HasFlag(photon.d)) {
                        map[photon.y, photon.x] |= photon.d;
                        energizedNewTile = true;
                    }

                    // fall off the edge of the map
                    if (photon.d == Direction.Up && photon.y == 0
                        || photon.d == Direction.Right && photon.x == width - 1
                        || photon.d == Direction.Down && photon.y == height - 1
                        || photon.d == Direction.Left && photon.x == 0)
                        continue;
                }

                // consider up
                if (photon.d == Direction.Up) {
                    var nextPhoton = photon with { y = photon.y - 1 };
                    switch (grid[nextPhoton.y, nextPhoton.x]) {
                        case '.' or '|':
                            nextPhotons.Add(nextPhoton);
                            break;
                        
                        case '/':
                            nextPhotons.Add(nextPhoton with { d = Direction.Right });
                            break;
                        
                        case '\\':
                            nextPhotons.Add(nextPhoton with { d = Direction.Left });
                            break;
                        
                        case '-':
                            nextPhotons.Add(nextPhoton with { d = Direction.Left });
                            nextPhotons.Add(nextPhoton with { d = Direction.Right });
                            break;
                    }
                }
                
                // consider right
                if (photon.d == Direction.Right) {
                    var nextPhoton = photon with { x = photon.x + 1 };
                    switch (grid[nextPhoton.y, nextPhoton.x]) {
                        case '.' or '-':
                            nextPhotons.Add(nextPhoton);
                            break;
                        
                        case '/':
                            nextPhotons.Add(nextPhoton with { d = Direction.Up });
                            break;
                        
                        case '\\':
                            nextPhotons.Add(nextPhoton with { d = Direction.Down });
                            break;
                        
                        case '|':
                            nextPhotons.Add(nextPhoton with { d = Direction.Up });
                            nextPhotons.Add(nextPhoton with { d = Direction.Down });
                            break;
                    }
                }
                
                // consider down
                if (photon.d == Direction.Down) {
                    var nextPhoton = photon with { y = photon.y + 1 };
                    switch (grid[nextPhoton.y, nextPhoton.x]) {
                        case '.' or '|':
                            nextPhotons.Add(nextPhoton);
                            break;
                        
                        case '/':
                            nextPhotons.Add(nextPhoton with { d = Direction.Left });
                            break;
                        
                        case '\\':
                            nextPhotons.Add(nextPhoton with { d = Direction.Right });
                            break;
                        
                        case '-':
                            nextPhotons.Add(nextPhoton with { d = Direction.Left });
                            nextPhotons.Add(nextPhoton with { d = Direction.Right });
                            break;
                    }
                }
                
                // consider left
                if (photon.d == Direction.Left) {
                    var nextPhoton = photon with { x = photon.x - 1 };
                    switch (grid[nextPhoton.y, nextPhoton.x]) {
                        case '.' or '-':
                            nextPhotons.Add(nextPhoton);
                            break;
                        
                        case '/':
                            nextPhotons.Add(nextPhoton with { d = Direction.Down });
                            break;
                        
                        case '\\':
                            nextPhotons.Add(nextPhoton with { d = Direction.Up });
                            break;
                        
                        case '|':
                            nextPhotons.Add(nextPhoton with { d = Direction.Up });
                            nextPhotons.Add(nextPhoton with { d = Direction.Down });
                            break;
                    }
                }
            }
            
            photons.Clear();
            photons.AddRange(nextPhotons);
            nextPhotons.Clear();
        }
    }

    private Func<int, int, Direction, char> MapRenderer =>
        (x, y, _) => 
            grid[y, x] != '.' ? grid[y, x] : (int)map[y, x] switch {
                1 => '^',
                2 => '>',
                4 => 'v',
                8 => '<',
                3 or 5 or 9 or 6 or 10 or 12 => '2',
                7 or 11 or 13 or 14 => '3',
                15 => '4',
                _ => '.'
            };

    private static Func<bool, char> EnergizedRenderer => b => b ? '#' : '.';

    [Flags]
    private enum Direction {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }

    private const string? ExampleInput = @"
.|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|....
";
    
    private const string? ExampleLightPath = @"
>|<<<\....
|v-.\^....
.v...|->>>
.v...v^.|.
.v...v^...
.v...v^..\
.v../2\\..
<->-/vv|..
.|<<<2-|.\
.v//.|.v..
";
    
    private const string? ExampleEnergizedTiles = @"
######....
.#...#....
.#...#####
.#...##...
.#...##...
.#...##...
.#..####..
########..
.#######..
.#...#.#..
";

    [Fact]
    public void RendersInputCorrectly() {
        Initialize(ExampleInput!);
        var actual = Environment.NewLine + grid.Render() + Environment.NewLine;
        Assert.Equal(ExampleInput, actual);
    }
    
    [Fact]
    public void RendersLightPathCorrectly() {
        Initialize(ExampleInput!);
        CalculateLightPath();
        var actual = Environment.NewLine + map.Render(MapRenderer) + Environment.NewLine;
        Assert.Equal(ExampleLightPath, actual);
    }
    
    [Fact]
    public void RendersEnergizedTilesCorrectly() {
        Initialize(ExampleInput!);
        CalculateLightPath();
        var actual = Environment.NewLine + energised.Render(EnergizedRenderer) + Environment.NewLine;
        Assert.Equal(ExampleEnergizedTiles, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day16(ExampleInput, Output).SolvePartOne();
        Assert.Equal(46, actual);
    }
}