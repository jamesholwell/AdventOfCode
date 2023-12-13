using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day12 : Solver {
    public Day12(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var accumulator = 0;
        
        foreach (var line in Shared.Split(Input)) {
            var parts = line.Split(' ', 2, StringSplitOptions.TrimEntries);

            var row = parts[0];
            var groups = parts[1].Split(',').Select(int.Parse).ToArray();

            var combinations = Combinations(string.Empty, row, groups);
            Trace.WriteLine($"Found {combinations} combinations for {line}");

            accumulator += combinations;
        }

        return accumulator;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private int Combinations(string prefix, string line, int[] groups) {
        // if there's nothing left to guess, we can quit early
        if (groups.Length == 0) {
            if (line.Contains('#')) return 0; // this was an invalid combination
            Trace.WriteLine(prefix + line.Replace('?', '.'));
            return 1;
        }

        // calculate the minimum row length based on each group with exactly one dividing space
        var groupInherentLength = groups.Length - 1;
        foreach (var group in groups) groupInherentLength += group;
        
        // calculate the slack remaining (the amount of 'choice' of where to place groups)
        var rowLength = line.Length;
        var slack = rowLength - groupInherentLength;

        var accumulator = 0;
        
        // consider the position of the first group:
        //   it cannot start before the beginning
        //   it cannot start after the point where there is exactly enough room for the rest of the groups
        for (var candidateStart = 0; candidateStart <= slack; ++candidateStart) {
            // we can't start if there's not an unknown
            if (line[candidateStart] == '.') continue;

            // we can't be followed by a broken spring (groups must be divided)
            var firstPositionAfterGroup = candidateStart + groups[0];
            if (firstPositionAfterGroup < rowLength && line[firstPositionAfterGroup] == '#')
                continue;
            
            // walk the span to check there are no known-good springs in the way
            var isValid = true;
            var i = candidateStart;
            while (isValid && ++i < firstPositionAfterGroup) {
                if (line[i] == '.')
                    isValid = false;
            }
            
            // now consider all the possible following combinations
            if (isValid) {
                if (firstPositionAfterGroup >= rowLength) {
                    // we are the last position, no more work to do
                    accumulator++;
                    break;
                }
                
                var nextPrefix = prefix + new string('.', candidateStart) + new string('#', groups[0]) + ".";
                var nextLine = line[(firstPositionAfterGroup + 1)..];
                accumulator += Combinations(nextPrefix, nextLine, groups[1..]);
            }
            
            // we can't start any later than the first broken spring (by definition, we're the first group!)
            if (line[candidateStart] == '#') break;
        }

        return accumulator;
    }

    private const string? ExampleInput = @"
???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1
";

    [Theory]
    [InlineData("???.###", new[] { 1, 1, 3 }, 1)]
    [InlineData(".??..??...?##.", new[] { 1, 1, 3 }, 4)]
    [InlineData("?#?#?#?#?#?#?#?", new[] { 1, 3, 1, 6 }, 1)]
    [InlineData("????.#...#...", new[] { 4, 1, 1 }, 1)]
    [InlineData("????.######..#####.", new[] { 1, 6, 5 }, 4)]
    [InlineData("?###????????", new[] { 3, 2, 1 }, 10)]
    public void SolvesPartOneExampleLines(string line, int[] groups, int expected) {
        var actual = Combinations(string.Empty, line, groups);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day12(ExampleInput, Output).SolvePartOne();
        Assert.Equal(21, actual);
    }
}