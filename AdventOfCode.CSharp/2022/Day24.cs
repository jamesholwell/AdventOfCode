using System.Text;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day24 : Solver {
    public Day24(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private class Map {
        public int Height { get; set; }

        public int Width { get; set; }

        public (int x, int y, int t) StartPosition;

        public (int x, int y, int t) ExitPosition;

        private readonly List<(int x, int y)> upBlizzards = new();

        private readonly List<(int x, int y)> rightBlizzards = new();

        private readonly List<(int x, int y)> downBlizzards = new();

        private readonly List<(int x, int y)> leftBlizzards = new();
        
        public readonly bool[][,] Frames;

        public Map(string input, int numberOfFrames = 1000) {
            var lines = Shared.Split(input);

            Height = lines.Length;
            Width = lines[0].Trim().Length;

            StartPosition = (lines[0].IndexOf('.'), 0, 0);
            ExitPosition = (lines[^1].IndexOf('.'), Height - 1, 0);

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

            Frames = Enumerable.Range(0, numberOfFrames).Select(_ => new bool[Width, Height]).ToArray();

            for (var t = 0; t < numberOfFrames; ++t) {
                for (var x = 0; x < Width; ++x) {
                    Frames[t][x, 0] = x != StartPosition.x;
                    Frames[t][x, Height - 1] = x != ExitPosition.x;
                }

                for (var y = 1; y < Height - 1; ++y) {
                    Frames[t][0, y] = true;
                    Frames[t][Width - 1, y] = true;
                }
            }

            var xMod = Width - 2;
            var yMod = Height - 2;

            foreach (var b in upBlizzards) {
                for (var t = 0; t < numberOfFrames; ++t) {
                    Frames[t][1 + b.x, 1 + ((b.y - t) % yMod + yMod) % yMod] = true;
                }
            }

            foreach (var b in rightBlizzards) {
                for (var t = 0; t < numberOfFrames; ++t) {
                    Frames[t][1 + (b.x + t) % xMod, 1 + b.y] = true;
                }
            }

            foreach (var b in downBlizzards) {
                for (var t = 0; t < numberOfFrames; ++t) {
                    Frames[t][1 + b.x, 1 + (b.y + t) % yMod] = true;
                }
            }

            foreach (var b in leftBlizzards) {
                for (var t = 0; t < numberOfFrames; ++t) {
                    Frames[t][1 + ((b.x - t) % xMod + xMod) % xMod, 1 + b.y] = true;
                }
            }
        }

        public string RenderFrame(int t) {
            var buffer = new StringBuilder(Height * (Width + 2) + 50);

            buffer.Append(Environment.NewLine);
            for (var y = 0; y < Height; y++) {
                for (var x = 0; x < Width; x++) {
                    buffer.Append(Frames[t][x, y] ? '#' : '.');
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

                    buffer.Append(blizzards.TryGetValue((x - 1, y - 1), out var blizzard) ? blizzard : (Frames[t][x, y] ? '#' : '.'));
                }

                buffer.Append(Environment.NewLine);
            }

            return buffer.ToString();
        }
    }
    
    protected override long SolvePartOne() {
        var map = new Map(Input);
        return Solve(map, map.StartPosition, map.ExitPosition);
    }

    protected override long SolvePartTwo() {
        var map = new Map(Input);

        var trip1 = Solve(map, map.StartPosition, map.ExitPosition);
        var trip2 = Solve(map, map.ExitPosition with { t = trip1 }, map.StartPosition);
        var trip3 = Solve(map, map.StartPosition with { t = trip2 }, map.ExitPosition);

        return trip3;
    }

    private static int Solve(Map map, (int x, int y, int t) startPosition, (int x, int y, int _) targetPosition) {
        var queue = new PriorityQueue<(int x, int y, int t), int>();
        var set = new HashSet<(int x, int y, int t)>();
        queue.Enqueue(startPosition, 0);
        set.Add(startPosition);

        // ReSharper disable once InconsistentNaming - mathematical naming convention
        int d((int x, int y, int t) p) => map.ExitPosition.x - p.x + map.ExitPosition.y - p.y;

        var minimumDistance = new Dictionary<(int x, int y, int t), int> {
            {startPosition, 0}
        };

        while (queue.TryDequeue(out var position, out _)) {
            set.Remove(position);
            var t = position.t + 1;
            var currentG = minimumDistance[position];

            if (position.x == targetPosition.x && position.y == targetPosition.y)
                return position.t;

            // right
            if (!map.Frames[t][position.x + 1, position.y]) {
                var positionRight = position with {x = position.x + 1, t = t};
                var tentativeRight = currentG + 1;

                if (!minimumDistance.TryGetValue(positionRight, out var gScoreRight) || tentativeRight < gScoreRight) {
                    minimumDistance[positionRight] = tentativeRight;

                    if (!set.Contains(positionRight)) {
                        set.Add(positionRight);
                        queue.Enqueue(positionRight, tentativeRight + d(positionRight));
                    }
                }
            }

            // down
            if (position.y < map.Height - 1 && !map.Frames[t][position.x, position.y + 1]) {
                var positionDown = position with {y = position.y + 1, t = t};
                var tentativeDown = currentG + 1;

                if (!minimumDistance.TryGetValue(positionDown, out var gScoreDown) || tentativeDown < gScoreDown) {
                    minimumDistance[positionDown] = tentativeDown;

                    if (!set.Contains(positionDown)) {
                        set.Add(positionDown);
                        queue.Enqueue(positionDown, tentativeDown + d(positionDown));
                    }
                }
            }

            // left
            if (!map.Frames[t][position.x - 1, position.y]) {
                var positionLeft = position with {x = position.x - 1, t = t};
                var tentativeLeft = currentG + 1;

                if (!minimumDistance.TryGetValue(positionLeft, out var gScoreLeft) || tentativeLeft < gScoreLeft) {
                    minimumDistance[positionLeft] = tentativeLeft;

                    if (!set.Contains(positionLeft)) {
                        set.Add(positionLeft);
                        queue.Enqueue(positionLeft, tentativeLeft + d(positionLeft));
                    }
                }
            }

            // up
            if (position.y > 0 && !map.Frames[t][position.x, position.y - 1]) {
                var positionUp = position with {y = position.y - 1, t = t};
                var tentativeUp = currentG + 1;

                if (!minimumDistance.TryGetValue(positionUp, out var gScoreUp) || tentativeUp < gScoreUp) {
                    minimumDistance[positionUp] = tentativeUp;

                    if (!set.Contains(positionUp)) {
                        set.Add(positionUp);
                        queue.Enqueue(positionUp, tentativeUp + d(positionUp));
                    }
                }
            }

            // wait
            if (!map.Frames[t][position.x, position.y]) {
                var positionWait = position with {t = t};
                var tentativeWait = currentG + 1;

                if (!minimumDistance.TryGetValue(positionWait, out var gScoreWait) || tentativeWait < gScoreWait) {
                    minimumDistance[positionWait] = tentativeWait;

                    if (!set.Contains(positionWait)) {
                        set.Add(positionWait);
                        queue.Enqueue(positionWait, tentativeWait + d(positionWait));
                    }
                }
            }
        }

        throw new InvalidOperationException("No route found");
    }

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

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day24(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(54, actual);
    }
}
