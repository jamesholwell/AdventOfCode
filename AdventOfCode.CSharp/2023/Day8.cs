using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day8 : Solver {
    public Day8(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var (instructions, left, right) = Parse(Input);

        var position = "AAA";
        var offset = 0;
        var modulus = instructions.Length;

        while (position != "ZZZ") {
            if (instructions[offset++ % modulus])
                position = right[position];
            else
                position = left[position];
        }

        return offset;
    }

    private (bool[] instructions, Dictionary<string, string> left, Dictionary<string, string> right) Parse(string input) {
        var outerParts = input.SplitBy("\n\n");
        var instructions = outerParts[0].ToCharArray().Select(c => c == 'R').ToArray();
        
        var left = new Dictionary<string, string>();
        var right = new Dictionary<string, string>();
        foreach (var line in outerParts[1].Split('\n')) {
            var parts = line.Replace("(", "").Replace(")", "").Replace(" ", "").Split('=');
            var connections = parts[1].Split(',');
            left[parts[0]] = connections[0];
            right[parts[0]] = connections[1];
        }

        return (instructions, left, right);
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput1 = @"
RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)
";
    
    private const string? ExampleInput2 = @"
LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)
";

    [Fact]
    public void SolvesPartOneExample1() {
        var actual = new Day8(ExampleInput1, Output).SolvePartOne();
        Assert.Equal(2, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample2() {
        var actual = new Day8(ExampleInput2, Output).SolvePartOne();
        Assert.Equal(6, actual);
    }
}