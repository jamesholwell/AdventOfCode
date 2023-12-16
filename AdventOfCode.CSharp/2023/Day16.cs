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
        
        CalculateLightPath((0, 0, Direction.Right));

        // render beam of light grid
        Trace.WriteHeading("Light path:");
        Trace.WriteLine(map.Render(MapRenderer));
        
        // render energized grid
        Trace.WriteHeading("Energized tiles:");
        Trace.WriteLine(energised.Render(EnergizedRenderer));
        
        // generate result
        return energised.Count();
    }

    public override long SolvePartTwo() {
        Initialize(Input);

        var downPhotons = Enumerable.Range(0, width).Select(x => (x, 0, Direction.Down));
        var upPhotons = Enumerable.Range(0, width).Select(x => (x, height - 1, Direction.Up));
        var rightPhotons = Enumerable.Range(0, height).Select(y => (0, y, Direction.Right));
        var leftPhotons = Enumerable.Range(0, height).Select(y => (width - 1, y, Direction.Left));
        var initialPhotons =
            downPhotons.Concat(upPhotons).Concat(rightPhotons).Concat(leftPhotons);

        var results = new List<long>();
        foreach (var initialPhoton in initialPhotons) {
            CalculateLightPath(initialPhoton);
            var count = energised.Count();
            results.Add(count);
            
            Trace.WriteLine($"Light path from {initialPhoton.Item1}, {initialPhoton.Item2} energizes {count} tiles");
            
            // reset grids
            Array.Clear(energised);
            Array.Clear(map);
        }

        return results.Max();
    }

    private void Initialize(string input) {
        // initialise grids
        grid = input.SplitGrid();
        height = grid.Height();
        width = grid.Width();
        map = new Direction[height, width];
        energised = new bool[height, width];
    }

    private void CalculateLightPath((int x, int y, Direction d) initialPhoton) {
        // initialise particles
        var photons = new List<(int x, int y, Direction d)> { initialPhoton };
        var nextPhotons = new List<(int x, int y, Direction)>();
        var energizedNewTile = true;
        
        // trace the light paths
        while (energizedNewTile) {
            energizedNewTile = false;
            
            foreach (var photon in photons) {
                // ignore any photon that has travelled off map
                if (photon.y < 0 || photon.y >= height || photon.x < 0 || photon.x >= width)
                    continue;
                
                // energize the tile
                energised[photon.y, photon.x] = true;

                // update maps (and see if we've passed through this tile in this direction before)
                if (!map[photon.y, photon.x].HasFlag(photon.d)) {
                    map[photon.y, photon.x] |= photon.d;
                    energizedNewTile = true;
                }

                // consider up
                if (photon.d == Direction.Up) {
                    if (grid[photon.y, photon.x] is '.' or '|')
                        nextPhotons.Add(photon with { y = photon.y - 1 });
                    
                    if (grid[photon.y, photon.x] is '/' or '-')
                        nextPhotons.Add(photon with { x = photon.x + 1, d = Direction.Right });
                        
                    if (grid[photon.y, photon.x] is '\\' or '-') 
                        nextPhotons.Add(photon with { x = photon.x - 1, d = Direction.Left });
                }
                
                // consider right
                if (photon.d == Direction.Right) {
                    if (grid[photon.y, photon.x] is '.' or '-')
                        nextPhotons.Add(photon with { x = photon.x + 1 });
                    
                    if (grid[photon.y, photon.x] is '/' or '|')
                        nextPhotons.Add(photon with { y = photon.y - 1, d = Direction.Up });
                        
                    if (grid[photon.y, photon.x] is '\\' or '|') 
                        nextPhotons.Add(photon with { y = photon.y + 1, d = Direction.Down });
                }
                
                // consider down
                if (photon.d == Direction.Down) {
                    if (grid[photon.y, photon.x] is '.' or '|')
                        nextPhotons.Add(photon with { y = photon.y + 1 });
                    
                    if (grid[photon.y, photon.x] is '/' or '-')
                        nextPhotons.Add(photon with { x = photon.x - 1, d = Direction.Left });
                        
                    if (grid[photon.y, photon.x] is '\\' or '-') 
                        nextPhotons.Add(photon with { x = photon.x + 1, d = Direction.Right });
                }
                
                // consider left
                if (photon.d == Direction.Left) {
                    if (grid[photon.y, photon.x] is '.' or '-')
                        nextPhotons.Add(photon with { x = photon.x - 1 });
                    
                    if (grid[photon.y, photon.x] is '/' or '|')
                        nextPhotons.Add(photon with { y = photon.y + 1, d = Direction.Down });
                        
                    if (grid[photon.y, photon.x] is '\\' or '|') 
                        nextPhotons.Add(photon with { y = photon.y - 1, d = Direction.Up });
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
        CalculateLightPath((0, 0, Direction.Right));
        var actual = Environment.NewLine + map.Render(MapRenderer) + Environment.NewLine;
        Assert.Equal(ExampleLightPath, actual);
    }
    
    [Fact]
    public void RendersEnergizedTilesCorrectly() {
        Initialize(ExampleInput!);
        CalculateLightPath((0, 0, Direction.Right));
        var actual = Environment.NewLine + energised.Render(EnergizedRenderer) + Environment.NewLine;
        Assert.Equal(ExampleEnergizedTiles, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day16(ExampleInput, Output).SolvePartOne();
        Assert.Equal(46, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day16(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(51, actual);
    }
}