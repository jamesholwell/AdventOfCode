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

    public override long SolvePartTwo() {
        var (instructions, left, right) = Parse(Input);
        var modulus = instructions.Length;

        var startingPositions = left.Keys.Where(k => k[2] == 'A').ToArray();
        var endingPositions = left.Keys.Where(k => k[2] == 'Z').ToArray();
        Output.WriteLine($"Found {startingPositions.Length} starting positions and {endingPositions.Length} nodes ending with Z");
        
        var positions = startingPositions.ToArray();
        var multiples = new List<int>();

        for (var g = 0; g < startingPositions.Length; ++g) {
            var i = 0;
            Output.WriteLine($"Ghost {g} starts at {startingPositions[g]}:");

            while (positions[g][2] != 'Z') {
                if (instructions[i++ % modulus])
                    positions[g] = right[positions[g]];
                else
                    positions[g] = left[positions[g]];
            }

            Output.WriteLine($"  ...found Z after {i} steps");
            multiples.Add(i);
            
            for (var j = 0; j < 5; ++j) {
                var initial = i;

                if (instructions[i++ % modulus])
                    positions[g] = right[positions[g]];
                else
                    positions[g] = left[positions[g]];

                while (positions[g][2] != 'Z') {
                    if (instructions[i++ % modulus])
                        positions[g] = right[positions[g]];
                    else
                        positions[g] = left[positions[g]];
                }

                Output.WriteLine($"  ...found Z again after {i - initial} more steps");
            }
        }

        return multiples.Select(i => (long)i).Aggregate(lcm);

        // ReSharper disable once InconsistentNaming - this is the mathematical name
        static long lcm(long a, long b) {
            return (a / gcf(a, b)) * b;
        }

        // ReSharper disable once InconsistentNaming - this is the mathematical name
        static long gcf(long a, long b) {
            while (b != 0) {
                var temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }
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
    
    private const string? ExampleInput3 = @"
LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)
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
    
    [Fact]
    public void SolvesPartTwoExample3() {
        var actual = new Day8(ExampleInput3, Output).SolvePartTwo();
        Assert.Equal(6, actual);
    }
}