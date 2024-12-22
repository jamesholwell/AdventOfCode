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

    protected override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput =
        """
        1
        10
        100
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
    public void SolvesPartOneExample() {
        var actual = new Day22(ExampleInput, Output).SolvePartOne();
        Assert.Equal(37327623, actual);
    }
}