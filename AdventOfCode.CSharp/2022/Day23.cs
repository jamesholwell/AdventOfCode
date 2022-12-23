using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day23 : Solver {
    public Day23(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private class Elf {
        // x runs left to right; y runs from top to bottom
        public (int x, int y) Position;

        public Elf(int row, int column) {
            this.Position = (column, row);
        }

        public (Elf, (int x, int y))? ProposedNextPosition(HashSet<(int x, int y)> currentPositions, int round) {
            var x = Position.x;
            var y = Position.y;

            // grid from top left
            var isEmptyNorthWest = !currentPositions.Contains((x - 1, y - 1));
            var isEmptyNorth     = !currentPositions.Contains((x    , y - 1));
            var isEmptyNorthEast = !currentPositions.Contains((x + 1, y - 1));
            
            var isEmptyWest      = !currentPositions.Contains((x - 1, y    ));
            var isEmptyEast      = !currentPositions.Contains((x + 1, y    ));

            var isEmptySouthWest = !currentPositions.Contains((x - 1, y + 1));
            var isEmptySouth     = !currentPositions.Contains((x    , y + 1));
            var isEmptySouthEast = !currentPositions.Contains((x + 1, y + 1));

            // compass from north west
            var isEmptyNortherly = isEmptyNorthWest && isEmptyNorth && isEmptyNorthEast;
            var isEmptyEasterly = isEmptyNorthEast && isEmptyEast && isEmptySouthEast;
            var isEmptySoutherly = isEmptySouthEast && isEmptySouth && isEmptySouthWest;
            var isEmptyWesterly = isEmptySouthWest && isEmptyWest && isEmptyNorthWest;

            if (isEmptyNortherly && isEmptyEasterly && isEmptySoutherly && isEmptyWesterly)
                return null;

            for (var look = 0; look < 4; ++look) {
                var looking = (look + round) % 4;

                switch (looking) {
                    case 0:
                        if (isEmptyNortherly) return (this, (x, y - 1));
                        break;

                    case 1:
                        if (isEmptySoutherly) return (this, (x, y + 1));
                        break;

                    case 2:
                        if (isEmptyWesterly) return (this, (x - 1, y));
                        break;

                    case 3:
                        if (isEmptyEasterly) return (this, (x + 1, y));
                        break;
                }
            }

            return null;
        }
    }

    private Elf[] Parse(string input) =>
        Shared.Split(input)
            .SelectMany((line, row) => line.Select((cell, column) => cell == '#' ? new Elf(row, column) : null))
            .Where(maybeElf => maybeElf != null)
            .Select(probablyElf => probablyElf!)
            .ToArray();

    private string Render(IEnumerable<Elf> elves, out int emptySpaces) {
        var positions = new HashSet<(int x, int y)>(elves.Select(e => e.Position));

        var minX = positions.Min(t => t.x);
        var maxX = positions.Max(t => t.x);
        var minY = positions.Min(t => t.y);
        var maxY = positions.Max(t => t.y);

        return RenderInner(positions, minX, maxX, minY, maxY, out emptySpaces);
    }

    private string Render(IEnumerable<Elf> elves, int minX, int maxX, int minY, int maxY) {
        var positions = new HashSet<(int x, int y)>(elves.Select(e => e.Position));
        return RenderInner(positions, minX, maxX, minY, maxY, out _);
    }

    private string RenderInner(HashSet<(int x, int y)> positions, int minX, int maxX, int minY, int maxY, out int emptySpaces) {
        var buffer = new StringBuilder((maxX - minX + 2) * (maxY - minY + 1) + 50);
        emptySpaces = 0;

        buffer.Append(Environment.NewLine);

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

    private static bool ExecuteRound(Elf[] elves, int round) {
        var currentPositions = new HashSet<(int x, int y)>(elves.Select(e => e.Position));

        var movements = 
            elves
            .AsParallel()
            .Select(e => e.ProposedNextPosition(currentPositions, round))
            .Where(p => p != null)
            .GroupBy(p => p!.Value.Item2)
            .Where(g => g.Count() == 1)
            .Select(g => g.Single()!.Value)
            .ToArray();

        foreach (var movement in movements) {
            movement.Item1.Position = movement.Item2;
        }

        return movements.Any();
    }

    public override long SolvePartOne() {
        var elves = Parse(Input);
        var emptySpaces = 0;

        Trace.WriteLine(Render(elves, out _));

        // tick!
        for (var i = 0; i < 10; ++i) {
            ExecuteRound(elves, i);

            Trace.WriteLine();
            Trace.WriteLine(Render(elves, out emptySpaces));
            Trace.WriteLine();
        }

        return emptySpaces;
    }

    public override long SolvePartTwo() {
        var elves = Parse(Input);
        var moving = true;
        var round = 0;

        Trace.WriteLine(Render(elves, out _));

        // tick!
        while(moving) {
            moving = ExecuteRound(elves, round++);

            Trace.WriteLine();
            Trace.WriteLine(Render(elves, out _));
            Trace.WriteLine();
        }

        return round;
    }
    
    private const string SmallExampleInput = @"
.....
..##.
..#..
.....
..##.
.....
";

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
        var actual = Render(elves, 0, 6, 0, 6);
        Output.WriteLine(actual);

        Assert.Equal(ExampleInput.ReplaceLineEndings(), actual);
    }

    [Fact]
    public void ModelsPartOneSmallExampleCorrectly() {
        var elves = Parse(SmallExampleInput);

        const string expectedRound0 = @"
.....
..##.
..#..
.....
..##.
.....
";

        var actualRound0 = Render(elves, 0, 4, 0, 5);
        Trace.WriteLine(actualRound0);
        Trace.WriteLine();
        Assert.Equal(expectedRound0.ReplaceLineEndings(), actualRound0);

        ExecuteRound(elves, 0);

        const string expectedRound1 = @"
..##.
.....
..#..
...#.
..#..
.....
";

        var actualRound1 = Render(elves, 0, 4, 0, 5);
        Trace.WriteLine(actualRound1);
        Trace.WriteLine();
        Assert.Equal(expectedRound1.ReplaceLineEndings(), actualRound1);

        ExecuteRound(elves, 1);

        const string expectedRound2 = @"
.....
..##.
.#...
....#
.....
..#..
";

        var actualRound2 = Render(elves, 0, 4, 0, 5);
        Trace.WriteLine(actualRound2);
        Trace.WriteLine();
        Assert.Equal(expectedRound2.ReplaceLineEndings(), actualRound2);

        ExecuteRound(elves, 2);

        const string expectedRound3 = @"
..#..
....#
#....
....#
.....
..#..
";

        var actualRound3 = Render(elves, 0, 4, 0, 5);
        Trace.WriteLine(actualRound3);
        Trace.WriteLine();
        Assert.Equal(expectedRound3.ReplaceLineEndings(), actualRound3);


        ExecuteRound(elves, 3);
        
        var actualRound4 = Render(elves, 0, 4, 0, 5);
        Trace.WriteLine(actualRound4);
        Trace.WriteLine();
        Assert.Equal(expectedRound3.ReplaceLineEndings(), actualRound4);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartOne();
        Assert.Equal(110, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(20, actual);
    }
}