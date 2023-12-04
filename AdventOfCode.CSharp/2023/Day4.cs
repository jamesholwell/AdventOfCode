using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day4 : Solver {
    public Day4(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var accumulator = 0;

        foreach (var line in Shared.Split(Input)) {
            var outerparts = line.Split(':'); 
            var parts = outerparts[1].Split('|');
            var winningNumbers = parts[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            var myNumbers = parts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var matches = myNumbers.Where(n => winningNumbers.Contains(n));
            var numberOfMatches = matches.Count();
            
            if (numberOfMatches > 0)
                accumulator += (int)Math.Pow(2, numberOfMatches - 1);
        }

        return accumulator;
    }

    public override long SolvePartTwo() {
        var lines = Shared.Split(Input);
        var accumulator = Enumerable.Repeat(1, lines.Length).ToArray();

        for (var i = 0; i < lines.Length; ++ i) {
            var line = lines[i];
            var outerparts = line.Split(':'); 
            var parts = outerparts[1].Split('|');
            var winningNumbers = parts[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            var myNumbers = parts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var matches = myNumbers.Where(n => winningNumbers.Contains(n));
            var numberOfMatches = matches.Count();

            if (numberOfMatches > 0) {
                for (var ii = i + 1; ii < i + 1 + numberOfMatches; ++ii) {
                    accumulator[ii] += accumulator[i];
                }
            }
        }

        return accumulator.Sum();
    }
    private const string? ExampleInput = @"
Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day4(ExampleInput, Output).SolvePartOne();
        Assert.Equal(13, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day4(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(30, actual);
    }
}