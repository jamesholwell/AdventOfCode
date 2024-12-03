using System.Text.RegularExpressions;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day3 : Solver {
    public Day3(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    protected override long SolvePartOne() {
        var regex = new Regex(@"mul\(([0-9]{1,3}),([0-9]{1,3})\)");
        return regex.Matches(Input)
                    .Sum(m => 
                        int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));
    }

    protected override long SolvePartTwo() {
        var regex = new Regex(@"mul\(([0-9]{1,3}),([0-9]{1,3})\)|don't()|do()");
        var isEnabled = true;
        var sum = 0;

        foreach (Match m in regex.Matches(Input)) {
            if (m.Value.StartsWith("don't")) {
                isEnabled = false;
                continue;
            }
            
            if (m.Value.StartsWith("do")) {
                isEnabled = true;
                continue;
            }
            
            if (isEnabled)
                sum += int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value);
        }

        return sum;
    }
    
    private const string? ExampleInput = @"
xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))
";
    
    private const string? ExamplePart2Input = @"
xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day3(ExampleInput, Output).SolvePartOne();
        Assert.Equal(161, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day3(ExamplePart2Input, Output).SolvePartTwo();
        Assert.Equal(48, actual);
    }
}