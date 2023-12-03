using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day3 : Solver {
    public Day3(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var lines = Shared.Split(Input).Select(l => l.ToCharArray()).ToArray();

        var h = lines.Length + 2;
        var w = lines[0].Length + 2;
        var schematic = new char[h, w];

        for (var y = 0; y < h; ++y)
        for (var x = 0; x < w; ++x)
            if (y == 0 || x == 0 || y == h - 1 || x == w - 1)
                schematic[y, x] = '.';
            else
                schematic[y, x] = lines[y - 1][x - 1];

        var buffer = new StringBuilder(w);
        var accumulator = 0;
        
        for (var y = 0; y < h; ++y) {
            buffer.Clear();
                
            for (var x = 0; x < w; ++x) {
                if (char.IsNumber(schematic[y, x])) {
                    buffer.Append(schematic[y, x]);
                    continue;
                }

                if (buffer.Length > 0) {
                    var value = int.Parse(buffer.ToString());
                    //Output.WriteLine(value);

                    var isSymbol = false;
                    isSymbol |= schematic[y, x] != '.'; // right-inline
                    isSymbol |= schematic[y, x - buffer.Length - 1] != '.'; // left-inline
                    for (var ix = x - buffer.Length - 1; ix <= x; ++ix) {
                        isSymbol |= schematic[y-1, ix] != '.'; // aboxe
                        isSymbol |= schematic[y+1, ix] != '.'; // below
                    }
                    
                    //Output.WriteLine(isSymbol);
                    if (isSymbol) accumulator += value;
                    buffer.Clear();
                }
            }
        }

        return accumulator;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day3(ExampleInput, Output).SolvePartOne();
        Assert.Equal(4361, actual);
    }
}