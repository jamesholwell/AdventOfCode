using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day15 : Solver {
    public Day15(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        return Input
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .SplitBy(",")
            .Sum(Hash);
    }

    public override long SolvePartTwo() {
        List<(string label, int focalLength)>[] boxes = Enumerable.Range(0, 256)
            .Select(_ => new List<(string label, int lens)>()).ToArray();

        var instructions = Input
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .SplitBy(",");

        var splitter = new Regex("^([a-z]+)(=|-)([0-9]?)$", RegexOptions.Compiled);

        foreach (var instruction in instructions) {
            var matches = splitter.Match(instruction);
            var label = matches.Groups[1].Value;
            var focalLength = matches.Groups[3].Value == "" ? -1 : int.Parse(matches.Groups[3].Value);

            var box = boxes[Hash(label)];
            var maybeIndex = box.FindIndex(p => p.label == label);

            if (maybeIndex > -1)
                box.RemoveAt(maybeIndex);

            if (focalLength > -1) {
                if (maybeIndex > -1)
                    box.Insert(maybeIndex, (label, focalLength));
                else
                    box.Add((label, focalLength));
            }
        }

        return boxes.SelectMany((b, i) => b.Select((p, ii) => (i + 1) * (ii + 1) * p.focalLength)).Sum();
    }

    private int Hash(string s) {
        return s.Aggregate(0, (current, c) => (17 * (current + c)) % 256);
    }

    private const string? ExampleInput = @"
rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
";

    [Fact]
    public void HashesHashAsExpected() {
        var actual = Hash("HASH");
        Assert.Equal(52, actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day15(ExampleInput, Output).SolvePartOne();
        Assert.Equal(1320, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day15(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(145, actual);
    }
}