using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day19 : Solver {
    private readonly string[] patterns;

    private readonly string[] designs;

    private readonly HashSet<string> possibleCache = [];

    private readonly HashSet<string> impossibleCache = [];

    public Day19(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) {
        if (input == null) {
            // allow empty constructor from test runner
            patterns = designs = [];
            return;
        }

        var parts = input.SplitBy("\n\n");
        patterns = parts[0].Split(", ");
        designs = parts[1].Split('\n');
    }

    private bool IsPossible(string design) {
        if (possibleCache.Contains(design)) return true;
        if (impossibleCache.Contains(design)) return false;

        var isPossible = IsPossibleInner(design);

        if (isPossible)
            possibleCache.Add(design);

        if (!isPossible)
            impossibleCache.Add(design);

        return isPossible;
    }

    private bool IsPossibleInner(string design) {
        for (int i = 0, ci = patterns.Length; i < ci; ++i) {
            var pattern = patterns[i];

            if (pattern == design)
                return true;

            if (pattern.Length > design.Length)
                continue;

            for (int j = 0, cj = pattern.Length; j < cj; ++j)
                if (pattern[j] != design[j])
                    goto NoMatch;

            if (IsPossible(design[pattern.Length..]))
                return true;

            NoMatch: ;
        }

        return false;
    }

    protected override long SolvePartOne() => designs.Count(IsPossible);

    protected override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string ExampleInput =
        """
        r, wr, b, g, bwu, rb, gb, br

        brwrr
        bggr
        gbbr
        rrbgbr
        ubwu
        bwurrg
        brgr
        bbrgwb
        """;

    [Fact]
    public void ParseWorks() {
        var solver = new Day19(ExampleInput);

        // ReSharper disable StringLiteralTypo
        var expectedPatterns = new[] {"r", "wr", "b", "g", "bwu", "rb", "gb", "br"};
        var expectedDesigns = new[]
        {
            "brwrr",
            "bggr",
            "gbbr",
            "rrbgbr",
            "ubwu",
            "bwurrg",
            "brgr",
            "bbrgwb"
        };
        // ReSharper restore StringLiteralTypo

        Assert.Equal(expectedPatterns, solver.patterns);
        Assert.Equal(expectedDesigns, solver.designs);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day19(ExampleInput, Output).SolvePartOne();
        Assert.Equal(6, actual);
    }
}