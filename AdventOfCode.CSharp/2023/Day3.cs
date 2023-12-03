using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day3 : Solver {
    public Day3(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() => Solve().SumPartNumbers;

    public override long SolvePartTwo() => Solve().SumGearRatios;

    public (int SumPartNumbers, int SumGearRatios) Solve() {
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
        var partNumberAccumulator = 0;
        var asterisks = new Dictionary<int, List<int>>();
        
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
                        isSymbol |= schematic[y-1, ix] != '.'; // above
                        isSymbol |= schematic[y+1, ix] != '.'; // below
                    }

                    if (schematic[y, x] == '*') {
                        // right-inline
                        if (!asterisks.ContainsKey(y * w + x))
                            asterisks[y * w + x] = new List<int>();
                        asterisks[y * w + x].Add(value);
                    }

                    if (schematic[y, x - buffer.Length - 1] == '*') {
                        var ay = y;
                        var ax = x - buffer.Length - 1;
                        // left-inline
                        if (!asterisks.ContainsKey(ay * w + ax))
                            asterisks[ay * w + ax] = new List<int>();
                        asterisks[ay * w + ax].Add(value);
                    }

                    for (var ix = x - buffer.Length - 1; ix <= x; ++ix) {
                        if (schematic[y - 1, ix] == '*') {
                            var ay = y - 1;
                            // above
                            if (!asterisks.ContainsKey(ay * w + ix))
                                asterisks[ay * w + ix] = new List<int>();
                            asterisks[ay * w + ix].Add(value);
                        }
                        
                        if (schematic[y + 1, ix] == '*') {
                            var ay = y + 1;
                            // above
                            if (!asterisks.ContainsKey(ay * w + ix))
                                asterisks[ay * w + ix] = new List<int>();
                            asterisks[ay * w + ix].Add(value);
                        }
                    }
                    
                    //Output.WriteLine(isSymbol);
                    if (isSymbol) partNumberAccumulator += value;
                    buffer.Clear();
                }
            }
        }

        var sumGearRatios = asterisks.Where(a => a.Value.Count == 2)
            .Sum(a => a.Value.First() * a.Value.Last());
        return (partNumberAccumulator, sumGearRatios);
    }
    
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
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day3(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(467835, actual);
    }
}