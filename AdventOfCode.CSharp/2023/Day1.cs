using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day1 : Solver {
    public Day1(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var acc = 0;
        var split = Shared.Split(Input);

        foreach (var line in split) {
            var firstNumber = line.ToCharArray().First(c => c is >= '0' and <= '9');
            var lastNumber = line.ToCharArray().Last(c => c is >= '0' and <= '9');
            acc += int.Parse(firstNumber.ToString() + lastNumber.ToString());
        }

        return acc;
    }

    public override long SolvePartTwo() {
        var acc = 0;
        var split = Shared.Split(Input);
        foreach (var line in split)
        {
            var firstNumber = new Regex("([0-9]|one|two|three|four|five|six|seven|eight|nine)").Matches(line).First()
                .Value;
            var rline = new string(line.Reverse().ToArray());
            var lastNumber = new Regex("([0-9]|enin|thgie|neves|xis|evif|ruof|eerht|owt|eno)").Matches(rline).First()
                .Value;

            var firstDigit = firstNumber
                .Replace("one", "1")
                .Replace("two", "2")
                .Replace("three", "3")
                .Replace("four", "4")
                .Replace("five", "5")
                .Replace("six", "6")
                .Replace("seven", "7")
                .Replace("eight", "8")
                .Replace("nine", "9");

            var lastDigit = lastNumber
                .Replace("eno", "1")
                .Replace("owt", "2")
                .Replace("eerht", "3")
                .Replace("ruof", "4")
                .Replace("evif", "5")
                .Replace("xis", "6")
                .Replace("neves", "7")
                .Replace("thgie", "8")
                .Replace("enin", "9");

            acc += int.Parse(firstDigit.ToString() + lastDigit.ToString());

            Console.WriteLine($"{firstDigit}{lastDigit} {line}");
        }

        return acc;
    }

    private const string? ExampleInput = @"
1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet
";

    private const string? ExampleInput2 = @"
two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen
";

    private const string? ExampleInput3 = @"
2911threeninesdvxvheightwobm
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day1(ExampleInput, Output).SolvePartOne();
        Assert.Equal(142, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day1(ExampleInput2, Output).SolvePartTwo();
        Assert.Equal(281, actual);
    }


    [Fact]
    public void SolvesEdgeCase() {
        var actual = new Day1(ExampleInput3, Output).SolvePartTwo();
        Assert.Equal(22, actual);
    }
}