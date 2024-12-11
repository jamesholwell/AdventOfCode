using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day11 : Solver {
    public Day11(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private static List<long> Blink(ICollection<long> stones) {
        var nextStones = new List<long>((int) Math.Ceiling(stones.Count * 1.25));

        foreach (var stone in stones) {
            switch (stone) {
                case 0:
                    nextStones.Add(1);
                    continue;
                    
                // 12 -> 1, 2
                case >= 10 and < 100: {
                    var l = stone / 10;
                    var r = stone - 10 * l;
                    nextStones.Add(l);
                    nextStones.Add(r);
                    continue;
                }
                    
                // 1234 -> 12, 34
                case >= 10_00 and < 100_00: {
                    var l = stone / 100;
                    var r = stone - 100 * l;
                    nextStones.Add(l);
                    nextStones.Add(r);
                    continue;
                }
                    
                // 123456 -> 123, 456
                case >= 10_00_00 and < 100_00_00: {
                    var l = stone / 1000;
                    var r = stone - 1000 * l;
                    nextStones.Add(l);
                    nextStones.Add(r);
                    continue;
                }
                    
                // 12345678 -> 1234, 5678
                case >= 10_00_00_00 and < 100_00_00_00: {
                    var l = stone / 10000;
                    var r = stone - 10000 * l;
                    nextStones.Add(l);
                    nextStones.Add(r);
                    continue;
                }
                    
                // 1234567890 -> 12345, 67890
                case >= 10_00_00_00_00 and < 100_00_00_00_00: {
                    var l = stone / 100000;
                    var r = stone - 100000 * l;
                    nextStones.Add(l);
                    nextStones.Add(r);
                    continue;
                }
                    
                // 123456789012 -> 123456, 789012
                case >= 10_00_00_00_00_00 and < 100_00_00_00_00_00: {
                    var l = stone / 1000000;
                    var r = stone - 1000000 * l;
                    nextStones.Add(l);
                    nextStones.Add(r);
                    continue;
                }
                
                // 12345678901234 -> 1234567, 8901234
                case >= 10_00_00_00_00_00_00 and < 100_00_00_00_00_00_00: {
                    var l = stone / 10000000;
                    var r = stone - 10000000 * l;
                    nextStones.Add(l);
                    nextStones.Add(r);
                    continue;
                }

                case >= 100_00_00_00_00_00:
                    throw new NotImplementedException();
                    
                default:
                    nextStones.Add(stone * 2024);
                    break;
            }
        }

        return nextStones;
    }

    protected override long SolvePartOne() {
        var stones = Input.SplitInt(" ").Select(i => (long)i).ToList();

        for (var i = 0; i < 25; ++i) {
            var nextStones = Blink(stones);

            stones = nextStones;
            Trace.WriteLine(string.Join(" ", stones));
        }

        return stones.Count;
    }
    
    protected override long SolvePartTwo() => Solve(Input, 75);

    private static long Solve(string input, int steps) {
        return input.SplitInt(" ")
            .Select(i => (engraving: (long) i, remaining: steps))
            .Select(CachedSolveInner)
            .Sum();
    }

    private static readonly Dictionary<(long, int), long> SolutionCache = new();

    private static long CachedSolveInner((long engraving, int remaining) stone) {
        if (SolutionCache.TryGetValue(stone, out var cachedAnswer))
            return cachedAnswer;

        return SolutionCache[stone] = SolveInner(stone);
    }
    
    private static long SolveInner((long engraving, int remaining) stone) {
        if (stone.remaining == 0)
            return 1;
        
        // first rule matches
        if (stone.engraving == 0)
            return CachedSolveInner((1, stone.remaining - 1));

        // second rule *doesn't* match
        var digits = 1 + (int)Math.Log10(stone.engraving);
        if (digits % 2 != 0) 
            return CachedSolveInner((2024 * stone.engraving, stone.remaining - 1));
        
        // split into two halves
        var exp = (int) Math.Pow(10, digits >> 1);
        var r = stone.engraving / exp;
        var l = stone.engraving - exp * r;
        return CachedSolveInner((r, stone.remaining - 1)) 
               + CachedSolveInner((l, stone.remaining - 1));
    }

    [Fact]
    public void BlinkReturnsExpectedResults() {
        var simpleExample = new long[] {0, 1, 10, 99, 999};
        var actual = Blink(simpleExample);

        var expected = new long[] {1, 2024, 1, 0, 9, 9, 2021976};
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(125, 1)]
    [InlineData(17, 2)]
    public void SolveInnerReturnsExpectedResults(int engraving, int expected) {
        var actual = SolveInner((engraving, 1));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SolvesSimplePartOneExampleUsingFastAlgorithm() {
        const string simpleExample = "0 1 10 99 999";
        var actual = Solve(simpleExample, 1);
        Assert.Equal(7, actual);
    }
    
    private const string? ExampleInput = @"
125 17
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day11(ExampleInput, Output).SolvePartOne();
        Assert.Equal(55312, actual);
    }
    
    
    [Fact]
    public void SolvesPartOneExampleUsingFastAlgorithm() {
        var actual = Solve(ExampleInput!, 25);
        Assert.Equal(55312, actual);
    }
}