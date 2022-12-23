using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day23 : Solver {
    public Day23(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private class Elf {
        public (int x, int y) Position;

        public Elf(int row, int column) {
            this.Position = (column, row);
        }
    }

    private Elf[] Parse(string input) =>
        Shared.Split(input)
            .SelectMany((line, row) => line.Select((cell, column) => cell == '#' ? new Elf(row, column) : null))
            .Where(maybeElf => maybeElf != null)
            .Select(probablyElf => probablyElf!)
            .ToArray();

    private string Render(Elf[] elves, out int emptySpaces) {
        var positions = new HashSet<(int x, int y)>(elves.Select(e => e.Position));

        var minX = positions.Min(t => t.x);
        var maxX = positions.Max(t => t.x);
        var minY = positions.Min(t => t.y);
        var maxY = positions.Max(t => t.y);
        var buffer = new StringBuilder((maxX - minX + 2) * (maxY - minY + 1));
        emptySpaces = 0;
        
        for (var y = minY; y <= maxY; y++) {
            for (var x = minX; x <= maxX; x++) {
                var elfIsHere = positions.Contains((x, y));
                buffer.Append(elfIsHere ? '#' : '.');
                if (!elfIsHere) emptySpaces++;
            }

            buffer.Append(Environment.NewLine);
        }

        return buffer.ToString();
    }

    public override long SolvePartOne() => 0;

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string ExampleInput = @"
....#..
..###.#
#...#.#
.#...##
#.###..
##.#.##
.#..#..
";

    [Fact] public void ParsesInputCorrectly() {
        var elves = Parse(ExampleInput);
        var actual = Render(elves, out _);
        Output.WriteLine(actual);

        Assert.Equal(ExampleInput.ReplaceLineEndings(), Environment.NewLine + actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartOne();
        Assert.Equal(110, actual);
    }
}