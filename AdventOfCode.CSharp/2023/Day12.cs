﻿using System.Text.RegularExpressions;
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

            var combinations = Combinations(row, groups);
            Trace.WriteLine($"Found {combinations} combinations for {line}");

            accumulator += combinations;
        }

        return accumulator;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private readonly Regex SlotRegex = new Regex("([^\\.])+", RegexOptions.Compiled);
    
    private int Combinations(string row, int[] groups) {
        // consider the no groups case
        if (groups.Length == 0) {
            // if any broken springs are present this is an invalid combination
            if (row.Contains('#')) return 0;
            
            // otherwise there's exactly one solution - all '.'
            return 1;
        }
        
        // find a slot to put the group into
        var slots = SlotRegex.Matches(row);
        if (!slots.Any())
            return 0;
        
        var groupLength = groups[0];
        var slot = slots.FirstOrDefault(s => s.Length >= groupLength);
        if (slot == null)
            return 0;
        
        var accumulator = 0;

        // now consider the position of the first broken spring within the slot
        for (var candidateStart = 0; candidateStart <= slot.Length - groupLength; ++candidateStart) {
            var startPosition = slot.Index + candidateStart;
            
            // we can't be followed by a broken spring (groups must be divided)
            var firstPositionAfterGroup = startPosition + groupLength;
            if (firstPositionAfterGroup < row.Length && row[firstPositionAfterGroup] == '#')
                continue;

            // otherwise, it's a valid selection, and now we recurse
            if (firstPositionAfterGroup < row.Length)
                accumulator += Combinations(row[(firstPositionAfterGroup + 1)..], groups[1..]);
            else if (groups.Length == 1)
                accumulator++;
            
            // the start can't be after a broken string (that wouldn't be the start)
            if (row[startPosition] == '#')
                break;
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
    // no groups into example strings
    [InlineData("", new int[0], 1)]
    [InlineData(".", new int[0], 1)]
    [InlineData("?", new int[0], 1)]
    [InlineData("#", new int[0], 0)]
    // single group into example strings
    [InlineData("", new[] { 1 }, 0)]
    [InlineData(".", new[] { 1 }, 0)]
    [InlineData("?", new[] { 1 }, 1)]
    [InlineData("#", new[] { 1 }, 1)]
    [InlineData("#?", new[] { 1 }, 1)]
    [InlineData("?#", new[] { 1 }, 1)]
    [InlineData("??", new[] { 1 }, 2)]
    [InlineData("?????", new[] { 1 }, 5)]
    public void SolvesTrivialExamples(string line, int[] groups, int expected) {
        var actual = Combinations(line, groups);
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("???.###", new[] { 1, 1, 3 }, 1)]
    [InlineData(".??..??...?##.", new[] { 1, 1, 3 }, 4)]
    [InlineData("?#?#?#?#?#?#?#?", new[] { 1, 3, 1, 6 }, 1)]
    [InlineData("????.#...#...", new[] { 4, 1, 1 }, 1)]
    [InlineData("????.######..#####.", new[] { 1, 6, 5 }, 4)]
    [InlineData("?###????????", new[] { 3, 2, 1 }, 10)]
    public void SolvesPartOneExampleLines(string line, int[] groups, int expected) {
        var actual = Combinations(line, groups);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day12(ExampleInput, Output).SolvePartOne();
        Assert.Equal(21, actual);
    }
}