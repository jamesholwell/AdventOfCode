using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day9(string? input = null, ITestOutputHelper? outputHelper = null) : Solver(input, outputHelper) {
    private readonly HashSet<Tuple<int, int>> tailHistory = new();

    private int mapX;

    private int mapY;

    private int mapWidth = 20;

    private int mapHeight = 5;

    private class IntPoint {
        public int X { get; set; }

        public int Y { get; set; }

        public Tuple<int, int> AsTuple() => Tuple.Create(X, Y);
    }

    protected override long SolvePartOne() {
        var h = new IntPoint();
        var t = new IntPoint();
        RememberTheTail(t);

        foreach (var instruction in Shared.Split(Input)) {
            var direction = instruction[0];
            var steps = int.Parse(instruction.Substring(1).Trim());

            for (var i = 0; i < steps; ++i) {
                // take head step
                switch (direction) {
                    case 'U':
                        h.Y++;
                        break;
                    case 'D':
                        h.Y--;
                        break;
                    case 'L':
                        h.X--;
                        break;
                    case 'R':
                        h.X++;
                        break;
                }

                // take tail step
                var z = Math.Max(Math.Abs(h.X - t.X), Math.Abs(h.Y - t.Y)) > 1 ? 0 : 1;
                if (h.X - t.X > z) t.X++;
                if (h.Y - t.Y > z) t.Y++;
                if (t.X - h.X > z) t.X--;
                if (t.Y - h.Y > z) t.Y--;

                RememberTheTail(t);
            }
        }

        var minX = tailHistory.Min(p => p.Item1);
        var maxX = tailHistory.Max(p => p.Item1);
        var minY = tailHistory.Min(p => p.Item2);
        var maxY = tailHistory.Max(p => p.Item2);
        Trace.WriteLine($"Grid {minX}-{maxX} wide by {minY}-{maxY} high");

        mapWidth = (maxX - minX) / 2;
        mapHeight = (maxY - minY) / 2;
        mapX = minX + mapWidth;
        mapY = minY + mapHeight;
        DrawMap(h, t);

        return tailHistory.Distinct().Count();
    }

    protected override long SolvePartTwo() {
        var rope = Enumerable.Range(0, 10).Select(_ => new IntPoint()).ToArray();

        RememberTheTail(rope[9]);

        foreach (var instruction in Shared.Split(Input)) {
            var direction = instruction[0];
            var steps = int.Parse(instruction.Substring(1).Trim());

            for (var i = 0; i < steps; ++i) {
                // take head step
                switch (direction) {
                    case 'U':
                        rope[0].Y++;
                        break;
                    case 'D':
                        rope[0].Y--;
                        break;
                    case 'L':
                        rope[0].X--;
                        break;
                    case 'R':
                        rope[0].X++;
                        break;
                }

                for (var k = 0; k < 9; k++) {
                    // take tail step
                    var h = rope[k];
                    var t = rope[k + 1];
                    var z = Math.Max(Math.Abs(h.X - t.X), Math.Abs(h.Y - t.Y)) > 1 ? 0 : 1;
                    if (h.X - t.X > z) rope[k + 1].X++;
                    if (h.Y - t.Y > z) rope[k + 1].Y++;
                    if (t.X - h.X > z) rope[k + 1].X--;
                    if (t.Y - h.Y > z) rope[k + 1].Y--;
                }

                RememberTheTail(rope[9]);
            }
        }

        return tailHistory.Distinct().Count();
    }

    private void RememberTheTail(IntPoint t) {
        tailHistory.Add(t.AsTuple());
    }

    private void DrawMap(IntPoint h, IntPoint t) {
        var buffer = new char[2 * mapWidth];

        for (var y = mapY + mapHeight; y >= mapY - mapHeight; y--) {
            var i = 0;
            for (var x = mapX - mapWidth; x < mapX + mapWidth; x++) {
                buffer[i++] =
                    h.X == x && h.Y == y ? 'H' :
                    t.X == x && t.Y == y ? 'T' :
                    tailHistory.Contains(Tuple.Create(x, y)) ? '#' : '.';
            }

            Trace.WriteLine(new string(buffer));
        }

        if (h.Y > mapY + mapHeight - 3) mapY++;
        if (h.Y < mapY - mapHeight + 3) mapY--;
        if (h.X > mapX + mapWidth - 3) mapX++;
        if (h.X < mapX - mapWidth + 3) mapX--;

        Trace.WriteLine(string.Empty);
    }

    private const string? ExampleInput = @"
R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day9(ExampleInput, Output).SolvePartOne();
        Assert.Equal(13, actual);
    }
}