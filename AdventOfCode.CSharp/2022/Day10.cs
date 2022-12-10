using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day10 : Solver {
    private readonly ITestOutputHelper? io;

    public Day10(string? input = null) : base(input) { }

    //public Day10(string? input = null, ITestOutputHelper? io = null) : base(input) {
    //    this.io = io;
    //}

    public IEnumerable<int> ExecuteProgram() {
        var instructions = Shared.Split(Input.Replace("addx", "noop\naddx")).Select(s => s.Split(' '));

        int x = 1;
        yield return x;

        foreach (var instruction in instructions) {
            switch (instruction[0]) {
                case "addx":
                    x += int.Parse(instruction[1]);
                    break;
            }

            yield return x;
        }
    }

    public override long SolvePartOne() {
        var p = ExecuteProgram();
        var cyclesOfInterest = Enumerable.Range(0, 6).Select(i => 20 + 40 * i);

        var sum = 0;
        foreach (var cycle in cyclesOfInterest) {
            var value = p.ElementAt(cycle - 1);
            io?.WriteLine($"@{cycle,-4}\t{value}");
            sum += cycle * value;
        }

        return sum;
    }
        
    public override long SolvePartTwo() {
        var pixelBuffer = new char[240];
        using var programEnumerator = ExecuteProgram().GetEnumerator();

        for (var cycle = 0; cycle < 240; ++cycle) {
            programEnumerator.MoveNext();
            pixelBuffer[cycle] = Math.Abs(programEnumerator.Current - cycle % 40) < 2 ? '#' : '.';
        }

        Action<string> wl = io == null ? Console.WriteLine : io.WriteLine;
        foreach (var line in pixelBuffer.Chunk(40))
            wl(new string(line));

        return 0;
    }

    private const string? SmallExampleInput = @"
noop
addx 3
addx -5
";

    private const string? ExampleInput = @"
addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop
";

    [Fact]
    public void RegisterHistoryIsCorrectForSmallExample() {
        var actual = new Day10(SmallExampleInput).ExecuteProgram().ToArray();
        Assert.Equal( 1, actual[0]); // noop
        Assert.Equal( 1, actual[1]); // addx 3 ..
        Assert.Equal( 1, actual[2]); // ..ends
        Assert.Equal( 4, actual[3]); // addx -5
        Assert.Equal( 4, actual[4]); // ..ends
        Assert.Equal(-1, actual[5]); // *
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day10(ExampleInput).SolvePartOne();
        Assert.Equal(13140, actual);
    }

    [Fact]
    public void PartTwoTestExecutor() {
        var actual = new Day10(ExampleInput).SolvePartTwo();
        Assert.Equal(0, actual);
    }
}