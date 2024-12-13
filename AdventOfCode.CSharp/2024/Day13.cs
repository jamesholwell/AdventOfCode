using AdventOfCode.Core;
using Microsoft.Z3;
using Xunit;
using Xunit.Abstractions;
using Solver = AdventOfCode.Core.Solver;

namespace AdventOfCode.CSharp._2024;

public class Day13 : Solver {
    public Day13(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private static IEnumerable<(int ax, int ay, int bx, int by, long px, long py)> Parse(string s) {
        var blocks = s.SplitBy("\n\n").Select(Shared.Split);
        foreach (var block in blocks) {
            var a = block[0]["Button A: ".Length..].Split(", ");
            var b = block[1]["Button B: ".Length..].Split(", ");
            var p = block[2]["Prize: ".Length..].Split(", ");
            yield return (
                int.Parse(a[0]["X+".Length..]),
                int.Parse(a[1]["Y+".Length..]),
                int.Parse(b[0]["X+".Length..]),
                int.Parse(b[1]["Y+".Length..]),
                int.Parse(p[0]["X=".Length..]),
                int.Parse(p[1]["Y=".Length..])
            );
        }
    }

    private static long NumberOfPushesToWin((int ax, int ay, int bx, int by, long px, long py) m) {
        for (var b = 100; b >= 0; --b) {
            if ((m.px - b * m.bx) % m.ax == 0 && (m.py - b * m.by) % m.ay == 0) {
                var ax = (m.px - b * m.bx) / m.ax;
                var ay = (m.py - b * m.by) / m.ay;
                if (ax == ay && ax is >= 0 and <= 100) {
                    return b + 3 * ax;
                }
            }
        }

        return 0;
    }

    private long Z3NumberOfPushesToWin((int ax, int ay, int bx, int by, long px, long py) m) {
        var ctx = new Context();
        var solver = ctx.MkSolver();
        
        // set up the integer-valued variables 
        var a = ctx.MkIntConst("a");
        var b = ctx.MkIntConst("b");
        
        // set up the equations in the constants
        var ax = ctx.MkInt(m.ax);
        var ay = ctx.MkInt(m.ay);
        var bx = ctx.MkInt(m.bx);
        var by = ctx.MkInt(m.by);
        var positionX = ctx.MkAdd(ctx.MkMul(a, ax), ctx.MkMul(b, bx)); // a * ax + b * bx
        var positionY = ctx.MkAdd(ctx.MkMul(a, ay), ctx.MkMul(b, by)); // a * ay + b * by
        
        // set up the constraint that it hits the target
        var px = ctx.MkInt(m.px);
        var py = ctx.MkInt(m.py);
        solver.Add(ctx.MkEq(px, positionX)); // x = a * ax + b * bx
        solver.Add(ctx.MkEq(py, positionY)); // y = a * ay + b * by

        // ask Z3 to solve the problem
        var status = solver.Check();
        
        // maybe it can't be done
        if (status == Status.UNSATISFIABLE)
            return 0;
        
        // return the price if it can
        var model = solver.Model;
        var za = Convert.ToInt64(model.Eval(a).ToString());
        var zb = Convert.ToInt64(model.Eval(b).ToString());
        return zb + 3 * za;
    }

    protected override long SolvePartOne() => Parse(Input).Sum(NumberOfPushesToWin);

    protected override long SolvePartTwo() => Parse(Input)
        .Select(m => m with { px = 10000000000000 + m.px, py = 10000000000000 + m.py })
        .Sum(Z3NumberOfPushesToWin);

    [Fact]
    public void ParsesInputCorrectly() {
        const string input =
            """
            Button A: X+94, Y+34
            Button B: X+22, Y+67
            Prize: X=8400, Y=5400
            """;
        var actual = Parse(input).Single();
        
        Assert.Equal(94, actual.ax);
        Assert.Equal(34, actual.ay);
        Assert.Equal(22, actual.bx);
        Assert.Equal(67, actual.by);
        Assert.Equal(8400, actual.px);
        Assert.Equal(5400, actual.py);
    }
    
    [Fact]
    public void SolvesEdgeCase() {
        const string input =
            """
            Button A: X+36, Y+12
            Button B: X+39, Y+91
            Prize: X=2412, Y=1740
            """;
        var actual = new Day13(input, Output).SolvePartOne();
        Assert.Equal(174, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample1() {
        const string input =
            """
            Button A: X+94, Y+34
            Button B: X+22, Y+67
            Prize: X=8400, Y=5400
            """;
        var actual = new Day13(input, Output).SolvePartTwo();
        Assert.Equal(0, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample2() {
        const string input =
            """
            Button A: X+26, Y+66
            Button B: X+67, Y+21
            Prize: X=12748, Y=12176
            """;
        var actual = new Day13(input, Output).SolvePartTwo();
        Assert.NotEqual(0, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample3() {
        const string input =
            """
            Button A: X+17, Y+86
            Button B: X+84, Y+37
            Prize: X=7870, Y=6450
            """;
        var actual = new Day13(input, Output).SolvePartTwo();
        Assert.Equal(0, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample4() {
        const string input =
            """
            Button A: X+69, Y+23
            Button B: X+27, Y+71
            Prize: X=18641, Y=10279
            """;
        var actual = new Day13(input, Output).SolvePartTwo();
        Assert.NotEqual(0, actual);
    }
    
    private const string? ExampleInput = @"
Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day13(ExampleInput, Output).SolvePartOne();
        Assert.Equal(480, actual);
    }
    
    [Fact]
    public void SolvesPartOneExampleWithFastAlgorithm() {
        var actual = Parse(ExampleInput!).Sum(Z3NumberOfPushesToWin);
        Assert.Equal(480, actual);
    }
}