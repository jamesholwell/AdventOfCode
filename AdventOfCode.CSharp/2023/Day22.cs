using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day22 : Solver {
    public Day22(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var bricks = Parse(Input);

        Trace.WriteLine(Render(bricks));
        
        return bricks.Length;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private (int x1, int x2, int y1, int y2, int z1, int z2, string label)[] Parse(string input) {
        var lines = Shared.Split(input);

        var l = 0;
        Func<int, string> label = lines.Length < 26 ? ShortLabel : LongLabel;
        
        var bricks = lines
            .Select(line => {
                var parts = line.Replace('~', ',').Split(',', 6);
                return (
                    x1: int.Parse(parts[0]),
                    x2: int.Parse(parts[3]),
                    y1: int.Parse(parts[1]),
                    y2: int.Parse(parts[4]),
                    z1: int.Parse(parts[2]),
                    z2: int.Parse(parts[5]),
                    label: label(l++));

            })
            .OrderBy(b => b.z1)
            .ToArray();
        
        return bricks;
    }

    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    private string Render((int x1, int x2, int y1, int y2, int z1, int z2, string label)[] bricks) {
        //var minX = bricks.Min(b => b.x1);
        var maxX = bricks.Max(b => b.x2);
        //var minY = bricks.Min(b => b.y1);
        var maxY = bricks.Max(b => b.y2);
        //var minZ = bricks.Min(b => b.z1);
        var maxZ = bricks.Max(b => b.z2);

        var sb = new StringBuilder();
        sb.Append(Environment.NewLine);

        var labelLength = bricks[0].label.Length;
        const string gap = "    ";
        var pad = new string(' ', labelLength - 1);
        var dot = new string('.', labelLength);
        var unk = new string('?', labelLength);
        
        // header
        sb.Append(">  z");
        sb.Append(gap);
        sb.Append(new string(' ', labelLength * maxX / 2)); 
        sb.Append(pad);
        sb.Append("x");
        sb.Append(new string(' ', labelLength * maxX / 2));
        sb.Append(gap);
        sb.Append(new string(' ', labelLength * maxY / 2));
        sb.Append(pad);
        sb.Append("y");
        sb.Append(new string(' ', labelLength * maxY / 2));
        sb.Append(Environment.NewLine);
        
        // x and y labels
        sb.Append("    ");
        sb.Append(gap);
        
        for (var x = 0; x <= maxX; ++x) {
            sb.Append(pad);
            sb.Append((char)(x % 10 + '0'));
        }

        sb.Append(gap);
        
        for (var y = 0; y <= maxY; ++y) {
            sb.Append(pad);
            sb.Append((char)(y % 10 + '0'));
        }
        
        sb.Append(Environment.NewLine);

        // each row
        for (var z = maxZ; z > 0; --z) {
            sb.Append($"{z,4}");        
            sb.Append(gap);

            for (var x = 0; x <= maxX; ++x) {
                var p = bricks.Where(b => b.z1 <= z && z <= b.z2 && b.x1 <= x & x <= b.x2).ToArray();
                sb.Append(p.Length == 0 ? dot : p.Length > 1 ? unk : p.Single().label);
            }

            sb.Append(gap);

            for (var y = 0; y <= maxY; ++y) {
                var p = bricks.Where(b => b.z1 <= z && z <= b.z2 && b.y1 <= y & y <= b.y2).ToArray();
                sb.Append(p.Length == 0 ? dot : p.Length > 1 ? unk : p.Single().label);
            }

            sb.Append(Environment.NewLine);
        }
        sb.Append("   0");
        sb.Append(gap);
        sb.Append(new string('-', labelLength * (maxX + 1)));
        sb.Append(gap);
        sb.Append(new string('-', labelLength * (maxY + 1)));
        sb.Append(Environment.NewLine);

        return sb.ToString();
    }

    private string ShortLabel(int i) => new string(new[] { (char)(i + 'A') });

    private string LongLabel(int i) => i > 675
        ? new(new[] { (char)((i - 676) / 26 + 'a'), (char)(i % 26 + 'A') })
        : new(new[] { (char)(i / 26 + 'A'), (char)(i % 26 + 'A') });

    private const string? ExampleInput = @"
1,0,1~1,2,1
0,0,2~2,0,2
0,2,3~2,2,3
0,0,4~0,2,4
2,0,5~2,2,5
0,1,6~2,1,6
1,1,8~1,1,9
";

    [Fact]
    public void ParsesExampleCorrectly() {
        var actual = Parse(ExampleInput!);

        Assert.Equal(7, actual.Length);
        Assert.Equal((1, 1, 0, 2, 1, 1, "A"), actual[0]);
        Assert.Equal((0, 2, 0, 0, 2, 2, "B"), actual[1]);
        Assert.Equal((0, 2, 2, 2, 3, 3, "C"), actual[2]);
        Assert.Equal((0, 0, 0, 2, 4, 4, "D"), actual[3]);
        Assert.Equal((2, 2, 0, 2, 5, 5, "E"), actual[4]);
        Assert.Equal((0, 2, 1, 1, 6, 6, "F"), actual[5]);
        Assert.Equal((1, 1, 1, 1, 8, 9, "G"), actual[6]);
    }
    
    [Theory]
    [InlineData(0, "AA")]
    [InlineData(25, "AZ")]
    [InlineData(26, "BA")]
    [InlineData(27, "BB")]
    [InlineData(196, "HO")]
    [InlineData(676, "aA")]
    public void GeneratesExpectedLabels(int i, string expected) {
        var actual = LongLabel(i);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GeneratesUniqueLabels() {
        var hashSet = new HashSet<string>();
        
        for (var i = 0; i < 1350; ++i)
            Assert.True(hashSet.Add(LongLabel(i)));
    }
    
    [Fact]
    public void RendersExampleCorrectly() {
        const string expected = @"
>  z     x      y 
        012    012
   9    .G.    .G.
   8    .G.    .G.
   7    ...    ...
   6    FFF    .F.
   5    ..E    EEE
   4    D..    DDD
   3    CCC    ..C
   2    BBB    B..
   1    .A.    AAA
   0    ---    ---
";
        var actual = Render(Parse(ExampleInput!));
        Output.WriteLine(actual);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day22(ExampleInput, Output).SolvePartOne();
        Assert.Equal(5, actual);
    }
}