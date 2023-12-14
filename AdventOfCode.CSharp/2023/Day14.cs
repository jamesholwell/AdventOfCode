using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day14 : Solver {
    public Day14(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var lines = Shared.Split(Input);

        var height = lines.Length;
        var width = lines[0].Length;
        var grid = new char[height, width];
        
        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            grid[y, x] = lines[y][x];

        for (var y = 1; y < height; ++y) {
            for (var x = 0; x < width; ++x) {
                var cy = y;
                while (cy > 0 && grid[cy, x] == 'O' && grid[cy - 1, x] == '.') {
                    grid[cy, x] = '.';
                    grid[cy - 1, x] = 'O';
                    cy--;
                }
            }
        }

        var accumulator = 0;
        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            if (grid[y, x] == 'O')
                accumulator += (height - y);

        return accumulator;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

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
}