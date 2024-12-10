using AdventOfCode.Core;
using AdventOfCode.Core.Grid;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day10 : Solver {
    public Day10(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private static int WalkMap<T>(int height, int width, int[,] grid, T current, T next) where T : ICollection<(int ox, int oy, int x, int y)>, new() {
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            if (grid[y, x] == 9) 
                current.Add((x, y, x, y));

        for (var i = 8; i >= 0; --i) {
            foreach (var pos in current) {
                if (pos.y > 0 && grid[pos.y - 1, pos.x] == i)
                    next.Add(pos with { y = pos.y - 1 });
                    
                if (pos.x < width - 1 && grid[pos.y, pos.x + 1] == i)
                    next.Add(pos with { x = pos.x + 1 });
                
                if (pos.y < height - 1 && grid[pos.y + 1, pos.x] == i)
                    next.Add(pos with { y = pos.y + 1 });
                
                if (pos.x > 0 && grid[pos.y, pos.x - 1] == i)
                    next.Add(pos with { x = pos.x - 1 });
            }

            current = [..next];
            next.Clear();
        }

        return current.Count;
    }

    protected override long SolvePartOne() {
        var grid = Input.SplitGrid(c => c - '0');
        var height = grid.Height();
        var width = grid.Width();
        
        var current = new HashSet<(int ox, int oy, int x, int y)>();
        var next = new HashSet<(int ox, int oy, int x, int y)>();
        return WalkMap(height, width, grid, current, next);
    }

    protected override long SolvePartTwo() {
        var grid = Input.SplitGrid(c => c - '0');
        var height = grid.Height();
        var width = grid.Width();
        
        var current = new List<(int ox, int oy, int x, int y)>();
        var next = new List<(int ox, int oy, int x, int y)>();
        return WalkMap(height, width, grid, current, next);
    }
    
    [Fact]
    public void SolveSimplePartOneExample1() {
        const string simpleInput = @"
0123
1234
8765
9876
";
        
        var actual = new Day10(simpleInput, Output).SolvePartOne();
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void SolveSimplePartOneExample2() {
        const string simpleInput = @"
...0...
...1...
...2...
6543456
7.....7
8.....8
9.....9
";
        
        var actual = new Day10(simpleInput, Output).SolvePartOne();
        Assert.Equal(2, actual);
    }
    
    [Fact]
    public void SolveSimplePartOneExample3() {
        const string simpleInput = @"
..90..9
...1.98
...2..7
6543456
765.987
876....
987....
";
        
        var actual = new Day10(simpleInput, Output).SolvePartOne();
        Assert.Equal(4, actual);
    }
    
    [Fact]
    public void SolveSimplePartOneExample4() {
        const string simpleInput = @"
10..9..
2...8..
3...7..
4567654
...8..3
...9..2
.....01
";
        
        var actual = new Day10(simpleInput, Output).SolvePartOne();
        Assert.Equal(3, actual);
    }
    
    
    private const string? ExampleInput = @"
89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day10(ExampleInput, Output).SolvePartOne();
        Assert.Equal(36, actual);
    }
    
    
    [Fact]
    public void SolveSimplePartTwoExample1() {
        const string simpleInput = @"
.....0.
..4321.
..5..2.
..6543.
..7..4.
..8765.
..9....
";        
        
        var actual = new Day10(simpleInput, Output).SolvePartTwo();
        Assert.Equal(3, actual);
    }
    
    [Fact]
    public void SolveSimplePartTwoExample2() {
        const string simpleInput = @"
..90..9
...1.98
...2..7
6543456
765.987
876....
987....
";        
        
        var actual = new Day10(simpleInput, Output).SolvePartTwo();
        Assert.Equal(13, actual);
    }
    
    [Fact]
    public void SolveSimplePartTwoExample3() {
        const string simpleInput = @"
012345
123456
234567
345678
4.6789
56789.
";        
        
        var actual = new Day10(simpleInput, Output).SolvePartTwo();
        Assert.Equal(227, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day10(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(81, actual);
    }
}