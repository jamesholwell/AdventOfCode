using System.Runtime.CompilerServices;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day22(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long NextSecretNumber(long current) {
        // mul 64 (2^5); mix; prune
        current = ((current << 6) ^ current) % 16777216;

        // div 32 (2^4); mix; prune
        current = ((current >> 5) ^ current) % 16777216;

        // mul 2048 (2^11); mix; prune
        current = ((current << 11) ^ current) % 16777216;

        return current;
    }

    private static long TwoThousandthEvolution(long n) {
        for (var i = 0; i < 2000; ++i)
            n = NextSecretNumber(n);

        return n;
    }

    protected override long SolvePartOne() => Shared.Split(Input).Select(long.Parse).Sum(TwoThousandthEvolution);

    private static IEnumerable<int> Prices(long n) {
        for (var i = 0; i < 2001; ++i) {
            yield return (int)(n % 10);
            n = NextSecretNumber(n);
        }
    }

    private static (int price, int change) Change(Tuple<int, int> arg) => (arg.Item2, arg.Item2 - arg.Item1);

    private static ((int, int, int, int) changes, int price)[] Windows((int price, int change)[] seq) {
        var r = new ((int, int, int, int) changes, int price)[seq.Length - 3];
        
        for (int i = 3, c = seq.Length; i < c; ++i)
            r[i - 3] = ((seq[i - 3].change, seq[i - 2].change, seq[i - 1].change, seq[i].change), seq[i].price);
        
        return r;
    }

    protected override long SolvePartTwo() {
        // parse input to price/change lists
        var monkeys = 
            Shared.Split(Input)
                  .Select(s => Prices(long.Parse(s)).Pairwise().Select(Change).ToArray());

        // determine the individual sequence->price dictionaries
        var sellingPrices = monkeys.Select(pairs =>
            Windows(pairs)
                .Where(p => p.price > 0) // we only care if we can get a half-decent price
                .GroupBy(p => p.changes)
                .ToDictionary(p => p.Key, p => p.First().price)).ToArray();

        var allSequences = sellingPrices
            .SelectMany(d => d.Keys)
            .Distinct()
            .ToArray();
        
        return allSequences.Max(seq => 
            sellingPrices.Sum(sp => sp.GetValueOrDefault(seq, 0)));
    }

    private const string? ExampleInput =
        """
        1
        10
        100
        2024
        """;
    
    private const string? ExampleInputPartTwo =
        """
        1
        2
        3
        2024
        """;

    [Fact]
    public void CheckBitShifts() {
        Assert.Equal(64, 1 << 6);
        Assert.Equal(1, 32 >> 5);
        Assert.Equal(2048, 1 << 11);
    }

    [Fact]
    public void CheckXorOperation() {
        Assert.Equal(37, 42 ^ 15);
    }

    [Fact]
    public void CheckMixOperation() {
        Assert.Equal(16113920, 100000000 % 16777216);
    }

    [Fact]
    public void SolvesNextTenSecretNumbers() {
        var current = 123L;

        int[] expected = [
            15887950,
            16495136,
            527345,
            704524,
            1553684,
            12683156,
            11100544,
            12249484,
            7753432,
            5908254
        ];

        for (var i = 0; i < 10; ++i) {
            Trace.WriteLine((current = NextSecretNumber(current)).ToString());
            Assert.Equal(expected[i], current);
        }
    }

    [Fact]
    public void CalculatesFirstTenPricesCorrectly() {
        int[] expected = [
            3,
            0,
            6,
            5,
            4,
            4,
            6,
            4,
            4,
            2
        ];

        Assert.Equal(expected, Prices(123L).Take(10));
    }

    [Fact]
    public void CalculatesFirstNineChangesCorrectly() {
        (int,int)[] expected = [
            (0, -3),
            (6, 6),
            (5, -1),
            (4, -1),
            (4, 0),
            (6, 2),
            (4, -2),
            (4, 0),
            (2, -2)
        ];

        Assert.Equal(expected, Prices(123L).Take(10).Pairwise().Select(Change));
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day22(ExampleInput, Output).SolvePartOne();
        Assert.Equal(37327623, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day22(ExampleInputPartTwo, Output).SolvePartTwo();
        Assert.Equal(23, actual);
    }
}