using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day25 : Solver<string> {
    public Day25(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private readonly char[] chars = "210-=".ToCharArray();

    private readonly long[] values = {2L, 1L, 0L, -1L, -2L};

    private long Decode(string s) {
        var sum = 0L;
        var exp = 1L;

        foreach (var c in s.Trim().ToCharArray().Reverse()) {
            sum += exp * values[Array.IndexOf(chars, c)];
            exp *= 5;
        }

        return sum;
    }

    private string Encode(long n) {
        if (n == 0) return "0";

        var buffer = new List<char>();

        while (n > 0) {
            var rem = n % 5;

            if (rem > 2) rem -= 5;

            buffer.Add(chars[Array.IndexOf(values, rem)]);

            n -= rem;
            n /= 5;
        }

        buffer.Reverse();

        return new string(buffer.ToArray());
    }

    public override string SolvePartOne() => Encode(Shared.Split(Input).Select(i => Decode(i.Trim())).Sum());

    public override string SolvePartTwo() => throw new InvalidOperationException("Start the smoothie maker yourself");

    private const string? ExampleInput = @"
1=-0-2
12111
2=0=
21
2=01
111
20012
112
1=-1=
1-12
12
1=
122
";

    [Fact]
    public void DecodesSnafuToDecimal() {
        Assert.Equal(1, Decode("            1".Trim()));
        Assert.Equal(2, Decode("            2".Trim()));
        Assert.Equal(3, Decode("           1=".Trim()));
        Assert.Equal(4, Decode("           1-".Trim()));
        Assert.Equal(5, Decode("           10".Trim()));
        Assert.Equal(6, Decode("           11".Trim()));
        Assert.Equal(7, Decode("           12".Trim()));
        Assert.Equal(8, Decode("           2=".Trim()));
        Assert.Equal(9, Decode("           2-".Trim()));
        Assert.Equal(10, Decode("           20".Trim()));
        Assert.Equal(15, Decode("          1=0".Trim()));
        Assert.Equal(20, Decode("          1-0".Trim()));
        Assert.Equal(2022, Decode("       1=11-2".Trim()));
        Assert.Equal(12345, Decode("      1-0---0".Trim()));
        Assert.Equal(314159265, Decode("1121-1110-1=0".Trim()));
    }
    
    [Fact]
    public void EncodesDecimalToSnafu() {
        Assert.Equal("            1".Trim(), Encode(1));
        Assert.Equal("            2".Trim(), Encode(2));
        Assert.Equal("           1=".Trim(), Encode(3));
        Assert.Equal("           1-".Trim(), Encode(4));
        Assert.Equal("           10".Trim(), Encode(5));
        Assert.Equal("           11".Trim(), Encode(6));
        Assert.Equal("           12".Trim(), Encode(7));
        Assert.Equal("           2=".Trim(), Encode(8));
        Assert.Equal("           2-".Trim(), Encode(9));
        Assert.Equal("           20".Trim(), Encode(10));
        Assert.Equal("          1=0".Trim(), Encode(15));
        Assert.Equal("          1-0".Trim(), Encode(20));
        Assert.Equal("       1=11-2".Trim(), Encode(2022));
        Assert.Equal("      1-0---0".Trim(), Encode(12345));
        Assert.Equal("1121-1110-1=0".Trim(), Encode(314159265));
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day25(ExampleInput, Output).SolvePartOne();
        Assert.Equal("2=-1=0", actual);
    }
}