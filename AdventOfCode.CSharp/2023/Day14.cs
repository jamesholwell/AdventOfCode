using System.Runtime.InteropServices;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day14 : Solver {
    private char[,] grid = null!;
    private int height;
    private int width;

    public Day14(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        InitializeGrid(Input);
        TiltNorth();
        return CurrentLoad;
    }

    public override long SolvePartTwo() {
        InitializeGrid(Input);
        
        var target = 1000000000;
        var history = new Dictionary<string, int>();
        var buffer = new char[width * height];
        int i = 0, matchedIndex = 0, firstMatched = 0;

        Remember();

        while (i++ < target) {
            Spin();

            var rememberedIndex = Remember();
            if (rememberedIndex == 0)
                continue;

            Output.WriteLine($"Found loop {i}={rememberedIndex}");
        
            // calculate skip
            var cycleLength = i - rememberedIndex;
            var cyclesToSkip = (target - i) / cycleLength;
            i += cycleLength * cyclesToSkip;
            Output.WriteLine($"...skipping ahead to {i}");

            history.Clear();
        }

        return CurrentLoad;

        int Remember() {
            var span = MemoryMarshal.CreateReadOnlySpan(ref grid[0, 0], grid.Length);
            span.CopyTo(buffer.AsSpan());
            var key = new string(buffer);
            
            if (history.TryGetValue(key, out var index))
                return index;

            history.Add(key, i);
            return 0;
        }
    }
    
    private void InitializeGrid(string input) {
        var lines = Shared.Split(input);

        height = lines.Length;
        width = lines[0].Length;
        grid = new char[height, width];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            grid[y, x] = lines[y][x];
    }
    
    private long CurrentLoad {
        get {
            var accumulator = 0L;

            for (var y = 0; y < height; ++y)
            for (var x = 0; x < width; ++x)
                if (grid[y, x] == 'O')
                    accumulator += height - y;

            return accumulator;
        }
    }

    private void Spin() {

        TiltNorth();
        TiltWest();
        TiltSouth();
        TiltEast();
    }

    private void TiltNorth() {
        for (var y = 1; y < height; ++y)
        for (var x = 0; x < width; ++x) {
            if (grid[y, x] != 'O' || grid[y - 1, x] != '.') continue;

            var cy = y - 1;
            while (cy > 0 && grid[cy - 1, x] == '.') cy--;

            grid[y, x] = '.';
            grid[cy, x] = 'O';
        }
    }

    private void TiltEast() {
        for (var y = 0; y < height; ++y)
        for (var x = width - 2; x >= 0; --x) {
            if (grid[y, x] != 'O' || grid[y, x + 1] != '.') continue;

            var cx = x + 1;
            while (cx < width - 1 && grid[y, cx + 1] == '.') cx++;

            grid[y, x] = '.';
            grid[y, cx] = 'O';
        }
    }

    private void TiltSouth() {
        for (var y = height - 2; y >= 0; --y)
        for (var x = 0; x < width; ++x) {
            if (grid[y, x] != 'O' || grid[y + 1, x] != '.') continue;

            var cy = y + 1;
            while (cy < height - 1 && grid[cy + 1, x] == '.') cy++;

            grid[y, x] = '.';
            grid[cy, x] = 'O';
        }
    }

    private void TiltWest() {
        for (var y = 0; y < height; ++y)
        for (var x = 1; x < width; ++x) {
            if (grid[y, x] != 'O' || grid[y, x - 1] != '.') continue;

            var cx = x - 1;
            while (cx > 0 && grid[y, cx - 1] == '.') cx--;

            grid[y, x] = '.';
            grid[y, cx] = 'O';
        }
    }

    private string Render() {
        var sb = new StringBuilder(width * height + height + 1);
        sb.Append(Environment.NewLine);

        for (var y = 0; y < height; ++y) {
            for (var x = 0; x < width; ++x) sb.Append(grid[y, x]);

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }

    private const string? ExampleInput = @"
O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....
";
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day14(ExampleInput, Output).SolvePartOne();
        Assert.Equal(136, actual);
    }

    [Fact]
    public void SpinCycleSpinsAsExpectedExample() {
        InitializeGrid(ExampleInput!);

        var r = Render();
        Output.WriteLine(r);

        Spin();
        r = Render();
        Output.WriteLine(r);

        Assert.Equal(@"
.....#....
....#...O#
...OO##...
.OO#......
.....OOO#.
.O#...O#.#
....O#....
......OOOO
#...O###..
#..OO#....
", r);
        
        Spin();
        r = Render();
        Output.WriteLine(r);

        Assert.Equal(@"
.....#....
....#...O#
.....##...
..O#......
.....OOO#.
.O#...O#.#
....O#...O
.......OOO
#..OO###..
#.OOO#...O
", r);
        
        Spin();
        r = Render();
        Output.WriteLine(r);

        Assert.Equal(@"
.....#....
....#...O#
.....##...
..O#......
.....OOO#.
.O#...O#.#
....O#...O
.......OOO
#...O###.O
#.OOO#...O
", r);
    }


    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day14(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(64, actual);
    }
}