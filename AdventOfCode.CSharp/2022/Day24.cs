using System.Data.Common;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day24 : Solver {
    public Day24(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private class Map {
        public int Height { get; set; }

        public int Width { get; set; }

        public (int x, int y) StartPosition;

        public (int x, int y) ExitPosition;

        private readonly List<(int x, int y)> upBlizzards = new();

        private readonly List<(int x, int y)> rightBlizzards = new();

        private readonly List<(int x, int y)> downBlizzards = new();

        private readonly List<(int x, int y)> leftBlizzards = new();

        private readonly (int x, int y)[,,][] blizzardCache;

        private readonly bool[][,] frames;

        public Map(string input, int numberOfFrames = 500) {
            var lines = Shared.Split(input);

            Height = lines.Length;
            Width = lines[0].Trim().Length;

            StartPosition = (lines[0].IndexOf('.'), 0);
            ExitPosition = (lines[^1].IndexOf('.'), Height - 1);

            for (var row = 1; row < Height - 1; row++) {
                for (var column = 1; column < Width - 1; column++) {
                    var cellPosition = (column - 1, row - 1);

                    switch (lines[row][column]) {
                        case '^':
                            upBlizzards.Add(cellPosition);
                            break;

                        case '>':
                            rightBlizzards.Add(cellPosition);
                            break;

                        case 'v':
                            downBlizzards.Add(cellPosition);
                            break;

                        case '<':
                            leftBlizzards.Add(cellPosition);
                            break;
                    }
                }
            }

            frames = Enumerable.Range(0, numberOfFrames).Select(_ => new bool[Height, Width]).ToArray();

            for (var t = 0; t < numberOfFrames; ++t) {
                for (var x = 0; x < Width; ++x) {
                    frames[t][x, 0] = x != StartPosition.x;
                    frames[t][x, Height - 1] = x != ExitPosition.x;
                }

                for (var y = 1; y < Height - 1; ++y) {
                    frames[t][0, y] = true;
                    frames[t][Width - 1, y] = true;
                }
            }

            var xMod = Width - 2;
            var yMod = Height - 2;

            foreach (var b in upBlizzards) {
                for (var t = 0; t < numberOfFrames; ++t) {
                    frames[t][1 + b.x, 1 + ((b.y - t) % yMod + yMod) % yMod] = true;
                }
            }

            foreach (var b in rightBlizzards) {
                for (var t = 0; t < numberOfFrames; ++t) {
                    frames[t][1 + (b.x + t) % xMod, 1 + b.y] = true;
                }
            }

            foreach (var b in downBlizzards) {
                for (var t = 0; t < numberOfFrames; ++t) {
                    frames[t][1 + b.x, 1 + (b.y + t) % yMod] = true;
                }
            }

            foreach (var b in leftBlizzards) {
                for (var t = 0; t < numberOfFrames; ++t) {
                    frames[t][1 + ((b.x - t) % xMod + xMod) % xMod, 1 + b.y] = true;
                }
            }
        }

        public string RenderFrame(int t) {
            var buffer = new StringBuilder(Height * (Width + 2) + 50);

            buffer.Append(Environment.NewLine);
            for (var y = 0; y < Height; y++) {
                for (var x = 0; x < Width; x++) {
                    buffer.Append(frames[t][x, y] ? '#' : '.');
                }

                buffer.Append(Environment.NewLine);
            }

            return buffer.ToString();
        }

        public string Render(int t, (int x, int y)? position = null) {
            var xMod = Width - 2;
            var yMod = Height - 2;

            var blizzards =
                upBlizzards.Select(b => (b.x, y: ((b.y - t) % yMod + yMod) % yMod, d: '^')).Union(
                        rightBlizzards.Select(b => (x: (b.x + t) % xMod, b.y, d: '>')).Union(
                            downBlizzards.Select(b => (b.x, y: (b.y + t) % yMod, d: 'v')).Union(
                                leftBlizzards.Select(b => (x: ((b.x - t) % xMod + xMod) % xMod, b.y, d: '<')))))
                    .GroupBy(b => (b.x, b.y))
                    .ToDictionary(g => g.Key, g => g.Count() == 1 ? g.Single().d : g.Count().ToString()[0]);

            var buffer = new StringBuilder(Height * (Width + 2) + 50);
            
            buffer.Append(Environment.NewLine);
            
            for (var y = 0; y < Height; y++) {
                for (var x = 0; x < Width; x++) {
                    if (position?.x == x && position.Value.y == y) {
                        buffer.Append('E');
                        continue;
                    }

                    buffer.Append(blizzards.TryGetValue((x - 1, y - 1), out var blizzard) ? blizzard : (frames[t][x, y] ? '#' : '.'));
                }

                buffer.Append(Environment.NewLine);
            }

            return buffer.ToString();
        }
    }
    
    public override long SolvePartOne() {
        var map = new Map(Input);
        var maxX = map.Width - 1;
        var maxY = map.Height - 1;
        var lowerBound = int.MaxValue;
        var i = 0;
        var nextToExit = map.ExitPosition with {y = map.ExitPosition.y - 1};
        
        var queue = new PriorityQueue<((int x, int y) position, int time, string direction), int>();
        queue.Enqueue((map.StartPosition, 0, "start"), 0);

        while (queue.TryDequeue(out var scenario, out var priority)) {
            Output.WriteLine($"Minute {scenario.time}, {scenario.direction}:");
            Output.WriteLine(map.Render(scenario.time, scenario.position));
            Trace.WriteLine(map.Render(scenario.time + 1));

            var newTime = scenario.time + 1;
            var position = scenario.position;

            var blizzards = new (int, int)[0];//map.GetBlizzards(position, newTime);

            var right = (position.x + 1, position.y);
            var canMoveRight = position.y != -1 && position.x < maxX && !blizzards.Contains(right);

            var down = (position.x, position.y + 1);
            var canMoveDown = position.y < maxY && !blizzards.Contains(down);

            var left = (position.x - 1, position.y);
            var canMoveLeft = position.y != -1 && position.x > 0 && !blizzards.Contains(left);

            var up = (position.x, position.y - 1);
            var canMoveUp = position.y > 0 && !blizzards.Contains(up);

            var canWait = !blizzards.Contains(position);

            var trace = "Can move: ";
            if (canMoveRight) trace += "Right";
            if (canMoveDown) trace += " Down";
            if (canMoveLeft) trace += " Left";
            if (canMoveUp) trace += " Up";
            if (canWait) trace += " Wait";
            Trace.WriteLine(trace);
            Trace.WriteLine();

            var distance = (nextToExit.x - position.x) + (nextToExit.y - position.y);
            
            if (newTime + distance >= lowerBound) {
                continue;
            }

            if (position == nextToExit) {
                if (newTime < lowerBound)
                    lowerBound = newTime;

                Output.WriteLine($"Found exit! {newTime}");

                continue;
            }
            
            if (canMoveRight && scenario.direction != "move left")
                queue.Enqueue((right, newTime, "move right"), priority - 1);

            if (canMoveDown && scenario.direction != "move up")
                queue.Enqueue((down, newTime, "move down"), priority - 1);

            if (canMoveLeft && newTime + distance + 1 < lowerBound && scenario.direction != "move right")
                queue.Enqueue((left, newTime, "move left"), priority + 1);

            if (canMoveUp && newTime + distance + 1 < lowerBound && scenario.direction != "move down") 
                queue.Enqueue((up, newTime, "move up"), priority + 1);

            if (canWait && scenario.direction != "wait")
                queue.Enqueue(scenario with { time = newTime, direction = "wait" }, priority + 1);
        }

        return lowerBound;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string SimpleExampleInput = @"
#.#####
#.....#
#>....#
#.....#
#...v.#
#.....#
#####.#
";

    private const string ExampleInput = @"
#.######
#>>.<^<#
#.<..<<#
#>v.><>#
#<^v^^>#
######.#
";

    [Fact]
    public void ParsesSimpleExampleCorrectly() {
        var actual = new Map(SimpleExampleInput).Render(0);
        Trace.WriteLine(actual);
        Assert.Equal(SimpleExampleInput.ReplaceLineEndings(), actual);
    }

    [Fact]
    public void SimpleExampleBlizzardsBlowCorrectly() {
        var map = new Map(SimpleExampleInput);

        const string expected1 = @"
#.#####
#.....#
#.#...#
#.....#
#.....#
#...#.#
#####.#
";
        var actual1 = map.RenderFrame(1);
        Trace.WriteLine(actual1);
        Assert.Equal(expected1.ReplaceLineEndings(), actual1);

        const string expected2 = @"
#.#####
#...#.#
#..#..#
#.....#
#.....#
#.....#
#####.#
";
        var actual2 = map.RenderFrame(2);
        Trace.WriteLine(actual2);
        Assert.Equal(expected2.ReplaceLineEndings(), actual2);

        const string expected3 = @"
#.#####
#.....#
#...#.#
#.....#
#.....#
#.....#
#####.#
";
        var actual3 = map.RenderFrame(3);
        Trace.WriteLine(actual3);
        Assert.Equal(expected3.ReplaceLineEndings(), actual3);


        const string expected4 = @"
#.#####
#.....#
#....##
#...#.#
#.....#
#.....#
#####.#
";
        var actual4 = map.RenderFrame(4);
        Trace.WriteLine(actual4);
        Assert.Equal(expected4.ReplaceLineEndings(), actual4);

        const string expected5 = @"
#.#####
#.....#
##....#
#.....#
#...#.#
#.....#
#####.#
";
        var actual5 = map.RenderFrame(5);
        Trace.WriteLine(actual5);
        Assert.Equal(expected5.ReplaceLineEndings(), actual5);
    }

    [Fact]
    public void SimpleExampleBlizzardsRenderCorrectly() {
        var map = new Map(SimpleExampleInput);

        const string expected1 = @"
#.#####
#.....#
#.>...#
#.....#
#.....#
#...v.#
#####.#
";
        var actual1 = map.Render(1);
        Trace.WriteLine(actual1);
        Assert.Equal(expected1.ReplaceLineEndings(), actual1);

        const string expected2 = @"
#.#####
#...v.#
#..>..#
#.....#
#.....#
#.....#
#####.#
";
        var actual2 = map.Render(2);
        Trace.WriteLine(actual2);
        Assert.Equal(expected2.ReplaceLineEndings(), actual2);

        const string expected3 = @"
#.#####
#.....#
#...2.#
#.....#
#.....#
#.....#
#####.#
";
        var actual3 = map.Render(3);
        Trace.WriteLine(actual3);
        Assert.Equal(expected3.ReplaceLineEndings(), actual3);


        const string expected4 = @"
#.#####
#.....#
#....>#
#...v.#
#.....#
#.....#
#####.#
";
        var actual4 = map.Render(4);
        Trace.WriteLine(actual4);
        Assert.Equal(expected4.ReplaceLineEndings(), actual4);

        const string expected5 = @"
#.#####
#.....#
#>....#
#.....#
#...v.#
#.....#
#####.#
";
        var actual5 = map.Render(5);
        Trace.WriteLine(actual5);
        Assert.Equal(expected5.ReplaceLineEndings(), actual5);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day24(ExampleInput, Output).SolvePartOne();
        Assert.Equal(18, actual);
    }
}
