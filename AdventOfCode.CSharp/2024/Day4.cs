using AdventOfCode.Core;
using AdventOfCode.Core.Grid;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day4 : Solver {
    private readonly char[,] grid;
    private readonly int width;
    private readonly int height;

    public Day4(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) {
        this.grid = Input.SplitGrid();
        this.width = this.grid.Width();
        this.height = this.grid.Height();
    }

    protected override long SolvePartOne() {
        var count = 0;
        var word = "XMAS".ToCharArray();
        
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                count += Search(word, x, y,  0, -1); // N  Upwards
                count += Search(word, x, y,  1, -1); // NE Up-right diagonal
                count += Search(word, x, y,  1,  0); // E  Forwards
                count += Search(word, x, y,  1,  1); // SE Down-right diagonal
                count += Search(word, x, y,  0,  1); // S  Downwards
                count += Search(word, x, y, -1,  1); // SW Down-left diagonal
                count += Search(word, x, y, -1,  0); // E  Backwards
                count += Search(word, x, y, -1, -1); // NW Up-left diagonal
            }
        }

        return count;
    }
    
    protected override long SolvePartTwo() {
        var count = 0;
        var word = "MAS".ToCharArray();
        
        for (var y = 1; y < height - 1; y++) {
            for (var x = 1; x < width - 1; x++) {
                // "\"
                var isDownRight = 1 == Search(word, x - 1, y - 1, 1, 1);
                var isUpLeft    = 1 == Search(word, x + 1, y + 1, -1, -1);

                // "/"
                var isUpRight   = 1 == Search(word, x - 1, y + 1, 1, -1);
                var isDownLeft  = 1 == Search(word, x + 1, y - 1, -1, 1);

                if ((isDownRight || isUpLeft) && (isUpRight || isDownLeft))
                    count += 1;
            }
        }

        return count;
    }

    private int Search(char[] word, int x, int y, int dx, int dy) {
        if (grid[y, x] != word[0]) return 0;

        for (int i = 1, c = word.Length; i < c; ++i) {
            x += dx;
            y += dy;
            
            if (x < 0 || x >= width || y < 0 || y >= height)
                return 0;
            
            if (grid[y, x] != word[i]) 
                return 0;
        }

        return 1;
    }
    
    private const string? SmallExampleInput = @"
..X...
.SAMX.
.A..A.
XMAS.S
.X....
";
    
    private const string? ExampleInput = @"
MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX
";

    [Fact]
    public void SolvesSmallExample() {
        var actual = new Day4(SmallExampleInput, Output).SolvePartOne();
        Assert.Equal(4, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day4(ExampleInput, Output).SolvePartOne();
        Assert.Equal(18, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day4(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(9, actual);
    }
}