using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day9 : Solver {
    private readonly ITestOutputHelper? io;

    private readonly HashSet<Tuple<int, int>> tailHistory;

    private int mapX = 0;

    private int mapY = 0;

    private int mapWidth = 20;

    private int mapHeight = 5;

    //public Day9(string? input = null, ITestOutputHelper? io = null) : base(input) {
    //    this.tailHistory = new List<Tuple<int, int>>();
    //    this.io = io;
    //}

    public Day9(string? input = null) : base(input) {
        this.tailHistory = new HashSet<Tuple<int, int>>();
    }

    private class IntPoint {
        public int X { get; set; }

        public int Y { get; set; }

        public Tuple<int, int> AsTuple() => Tuple.Create(X, Y);
    }

    public override long SolvePartOne() {
        var h = new IntPoint();
        var t = new IntPoint();

        //DrawMap(h, t);
        RememberTheTail(t);

        foreach (var instruction in Shared.Split(Input)) {
            var direction = instruction[0];
            var steps = int.Parse(instruction.Substring(1).Trim());

            for (var i = 0; i < steps; ++i) {                
                // take head step
                switch (direction) {
                    case 'U': h.Y++; break;
                    case 'D': h.Y--; break;
                    case 'L': h.X--; break;
                    case 'R': h.X++; break;
                }

                // take tail step
                var z = Math.Max(Math.Abs(h.X - t.X), Math.Abs(h.Y - t.Y)) > 1 ? 0 : 1;
                if (h.X - t.X > z) t.X++;
                if (h.Y - t.Y > z) t.Y++;
                if (t.X - h.X > z) t.X--;
                if (t.Y - h.Y > z) t.Y--;

                //DrawMap(h, t, direction, steps, i);
                RememberTheTail(t);
            }
        }

        //DrawMap(h, t);

        var minX = tailHistory.Min(t => t.Item1);
        var maxX = tailHistory.Max(t => t.Item1);
        var minY = tailHistory.Min(t => t.Item2);
        var maxY = tailHistory.Max(t => t.Item2);
        Console.WriteLine($"Grid {minX}-{maxX} wide by {minY}-{maxY} high");

        mapWidth = (maxX - minX) / 2;
        mapHeight = (maxY - minY) / 2;
        mapX = minX + mapWidth;
        mapY = minY + mapHeight;
        DrawMap(h, t);

        return tailHistory.Distinct().Count();
    }

    private void RememberTheTail(IntPoint t) {
        tailHistory.Add(t.AsTuple());
    }

    private void DrawMap(IntPoint h, IntPoint t, char direction, int steps, int step) {
        Action<string> writeLine = io == null ? Console.WriteLine : io.WriteLine;
        writeLine($"{direction} {steps}...{step + 1}/{steps}");

        DrawMap(h, t);
    }

    private void DrawMap(IntPoint h, IntPoint t) {
        Action<string> writeLine = io == null ? Console.WriteLine : io.WriteLine;
        var buffer = new char[2 * mapWidth];

        for (var y = mapY + mapHeight; y >= mapY - mapHeight; y--) {
            var i = 0;
            for (var x = mapX - mapWidth; x < mapX + mapWidth; x++) {
                buffer[i++] =
                    h.X == x && h.Y == y ? 'H' :
                    t.X == x && t.Y == y ? 'T' :
                    tailHistory.Contains(Tuple.Create(x, y)) ? '#' : '.';
            }

            writeLine(new string(buffer));
        }

        if (h.Y > mapY + mapHeight - 3) mapY++;
        if (h.Y < mapY - mapHeight + 3) mapY--;
        if (h.X > mapX + mapWidth - 3) mapX++;
        if (h.X < mapX - mapWidth + 3) mapX--;

        writeLine(string.Empty);
        //Thread.Sleep(10);
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
        var actual = new Day9(ExampleInput).SolvePartOne();
        Assert.Equal(13, actual);
    }
}