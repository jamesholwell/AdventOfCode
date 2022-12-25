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

        public Map(string input) {
            var lines = Shared.Split(input);

            Height = lines.Length - 2;
            Width = lines[0].Trim().Length - 2;
            blizzardCache = new (int x, int y)[Width, Height + 1, Width * Height][];

            StartPosition = (lines[0].IndexOf('.') - 1, -1);
            ExitPosition = (lines[^1].IndexOf('.') - 1, Height);

            for (var row = 0; row < Height; row++) {
                for (var column = 0; column < Width; column++) {
                    var cellPosition = (column, row);

                    switch (lines[row + 1][column + 1]) {
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
        }

        public string Render(int t, (int x, int y)? position = null) {
            var buffer = new StringBuilder((Height + 4) * (Width + 4));
            
            buffer.Append(Environment.NewLine);
            buffer.Append(new string('#', 1 + StartPosition.x));
            buffer.Append(position?.y == -1 ? 'E' : '.');
            buffer.Append(new string('#', Width - StartPosition.x));
            buffer.Append(Environment.NewLine);

            var blizzards =
                upBlizzards.Select(b => (b.x, y: ((b.y - t) % Height + Height) % Height, d: '^')).Union(
                        rightBlizzards.Select(b => (x: (b.x + t) % Width, b.y, d: '>')).Union(
                            downBlizzards.Select(b => (b.x, y: (b.y + t) % Height, d: 'v')).Union(
                                leftBlizzards.Select(b => (x: ((b.x - t) % Width + Width) % Width, b.y, d: '<')))))
                    .GroupBy(b => (b.x, b.y))
                    .ToDictionary(g => g.Key, g => g.Count() == 1 ? g.Single().d : g.Count().ToString()[0]);

            for (var row = 0; row < Height; row++) {
                buffer.Append('#');

                for (var column = 0; column < Width; column++) {
                    buffer.Append((position.HasValue && position.Value.x == column && position.Value.y == row) ? 'E' : 
                    blizzards.TryGetValue((column, row), out var blizzard) ? blizzard : '.');
                }
                
                buffer.Append('#');
                buffer.Append(Environment.NewLine);
            }

            buffer.Append(new string('#', 1 + ExitPosition.x));
            buffer.Append('.');
            buffer.Append(new string('#', Width - ExitPosition.x));
            buffer.Append(Environment.NewLine);

            return buffer.ToString();
        }

        public (int x, int y)[] GetBlizzards((int x, int y) position, int t) {
            t %= (Height * Width);

            blizzardCache[position.x, position.y + 1, t] ??= 
                upBlizzards.Select(b => b with {y = ((b.y - t) % Height + Height) % Height}).Union(
                        rightBlizzards.Select(b => b with {x = (b.x + t) % Width}).Union(
                            downBlizzards.Select(b => b with {y = (b.y + t) % Height}).Union(
                                leftBlizzards.Select(b => b with {x = ((b.x - t) % Width + Width) % Width}))))
                    .Where(b => Math.Abs(b.x - position.x) < 2 && Math.Abs(b.y - position.y) < 2)
                    .ToArray();

            return blizzardCache[position.x, position.y + 1, t];
        }
    }
    
    public override long SolvePartOne() {
        var map = new Map(Input);
        var maxX = map.Width - 1;
        var maxY = map.Height - 1;
        var i = 0;
        var nextToExit = map.ExitPosition with {y = map.ExitPosition.y - 1};
        
        var queue = new PriorityQueue<((int x, int y) position, int time), int>();
        queue.Enqueue((map.StartPosition, 0), map.Height + 1 + map.Width);
        var set = new HashSet<((int x, int y) position, int time)>();
        set.Add(queue.Peek());

        var shortest = new Dictionary<((int x, int y) position, int time), int>();
        shortest.Add(queue.Peek(), 0);

        while (queue.TryDequeue(out var scenario, out var priority)) {
            set.Remove(scenario);
            //Trace.WriteLine($"Minute {scenario.time}, {scenario.direction}:");
            //Trace.WriteLine(map.Render(scenario.time, scenario.position));
            //Trace.WriteLine(map.Render(scenario.time + 1));

            var newTime = scenario.time + 1;
            var position = scenario.position;
            var score = shortest[scenario];

            var blizzards = map.GetBlizzards(position, newTime);

            var right = position with { x = position.x + 1 };
            var canMoveRight = position.y != -1 && position.x < maxX && !blizzards.Contains(right);

            var down = position with { y = position.y + 1 };
            var canMoveDown = position.y < maxY && !blizzards.Contains(down);

            var left = position with { x = position.x - 1 };
            var canMoveLeft = position.y != -1 && position.x > 0 && !blizzards.Contains(left);

            var up = position with { y = position.y - 1 };
            var canMoveUp = position.y > 0 && !blizzards.Contains(up);

            var canWait = !blizzards.Contains(position);

            //var trace = "Can move: ";
            //if (canMoveRight) trace += "Right";
            //if (canMoveDown) trace += " Down";
            //if (canMoveLeft) trace += " Left";
            //if (canMoveUp) trace += " Up";
            //if (canWait) trace += " Wait";
            //Trace.WriteLine(trace);
            //Trace.WriteLine();
            
            if (position == nextToExit) {
                return newTime;
            }

            if (canMoveRight) {
                var rp = (right, newTime);

                if (!shortest.TryGetValue(rp, out var rs) || score + 1 < rs) {
                    shortest[rp] = score + 1;
                    if (!set.Contains(rp)) {
                        queue.Enqueue(rp, score + 1 + map.ExitPosition.x - right.x + map.ExitPosition.y - right.y);
                        set.Add(rp);
                    }
                }
            }

            if (canMoveDown) {
                var dp = (down, newTime);

                if (!shortest.TryGetValue(dp, out var ds) || score + 1 < ds) {
                    shortest[dp] = score + 1;
                    if (!set.Contains(dp)) {
                        queue.Enqueue(dp, score + 1 + map.ExitPosition.x - down.x + map.ExitPosition.y - down.y);
                        set.Add(dp);
                    }
                }
            }

            if (canMoveLeft) {
                var lp = (left, newTime);

                if (!shortest.TryGetValue(lp, out var ls) || score + 1 < ls) {
                    shortest[lp] = score + 1;
                    if (!set.Contains(lp)) {
                        queue.Enqueue(lp, score + 1 + map.ExitPosition.x - left.x + map.ExitPosition.y - left.y);
                        set.Add(lp);
                    }
                }
            }

            if (canMoveUp) {
                var upp = (up, newTime);

                if (!shortest.TryGetValue(upp, out var us) || score + 1 < us) {
                    shortest[upp] = score + 1;
                    if (!set.Contains(upp)) {
                        queue.Enqueue(upp, score + 1 + map.ExitPosition.x - up.x + map.ExitPosition.y - up.y);
                        set.Add(upp);
                    }
                }
            }

            if (canWait) {
                var wp = scenario with { time = newTime };

                if (!shortest.TryGetValue(wp, out var ws) || score + 1 < ws) {
                    shortest[wp] = score + 1;
                    if (!set.Contains(wp)) {
                        queue.Enqueue(wp,
                            score + 1 + map.ExitPosition.x - position.x + map.ExitPosition.y - position.y);
                        set.Add(wp);
                    }
                }

            }
        }

        throw new InvalidOperationException();
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
