using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day5 : Solver {
    public Day5(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private ((int page, int beforePage)[], int[][] updates) Parse(string input) {
        var parts = input.SplitBy("\n\n");

        var rules = parts[0]
            .Split()
            .Select(s => s.Split("|", 2)).Select(ss => (int.Parse(ss[0]), int.Parse(ss[1])))
            .ToArray();

        var updates = parts[1]
            .Split()
            .Select(s => s.Split(',').Select(int.Parse).ToArray())
            .ToArray();

        return (rules, updates);
    }

    private bool ObeysRules(int[] update, (int page, int beforePage)[] rules) {
        var positions = update.Select((p, i) => new KeyValuePair<int, int>(p, i))
            .ToDictionary(p => p.Key, p => p.Value);

        foreach (var rule in rules) {
            if (!positions.TryGetValue(rule.page, out var pageIndex) ||
                !positions.TryGetValue(rule.beforePage, out var beforePageIndex))
                continue;

            if (pageIndex > beforePageIndex)
                return false;
        }

        return true;
    }

    private static int[] FixUpdate(int[] update, (int page, int beforePage)[] rules) {
        var fixedUpdate = new List<int>(update);

        var positions = fixedUpdate.Select((p, i) => new KeyValuePair<int, int>(p, i))
            .ToDictionary(p => p.Key, p => p.Value);

        var isUnfinished = true;
        while (isUnfinished) {
            isUnfinished = false;

            foreach (var rule in rules) {
                if (!positions.TryGetValue(rule.page, out var pageIndex) ||
                    !positions.TryGetValue(rule.beforePage, out var beforePageIndex))
                    continue;

                if (pageIndex <= beforePageIndex)
                    continue;
                
                
                var movingPage = fixedUpdate[pageIndex];
                fixedUpdate.RemoveAt(pageIndex);
                fixedUpdate.Insert(beforePageIndex, movingPage);

                positions = fixedUpdate.Select((p, i) => new KeyValuePair<int, int>(p, i))
                    .ToDictionary(p => p.Key, p => p.Value);

                isUnfinished = true;
                break;
            }
        }

        return fixedUpdate.ToArray();
    }

    protected override long SolvePartOne() {
        var (rules, updates) = Parse(Input);

        return updates
            .Where(update => ObeysRules(update, rules))
            .Sum(update => update[update.Length / 2]);
    }

    protected override long SolvePartTwo() {
        var (rules, updates) = Parse(Input);

        return updates
            .Where(update => !ObeysRules(update, rules))
            .Select(update => FixUpdate(update, rules))
            .Sum(update => update[update.Length / 2]);
    }

    private const string? ExampleInput = @"
47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day5(ExampleInput, Output).SolvePartOne();
        Assert.Equal(143, actual);
    }


    [Theory]
    [InlineData(new[] { 75, 97, 47, 61, 53 }, new[] { 97, 75, 47, 61, 53 })]
    [InlineData(new[] { 61, 13, 29 }, new[] { 61, 29, 13 })]
    [InlineData(new[] { 97, 13, 75, 29, 47 }, new[] { 97, 75, 47, 29, 13 })]
    public void FixReordersUpdatesCorrectly(int[] update, int[] expected) {
        var (rules, _) = Parse(ExampleInput!);

        Assert.Equal(expected, FixUpdate(update, rules));
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day5(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(123, actual);
    }
}