using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day23 : Solver {
    public Day23(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        return Solve(Input);
    }

    public override long SolvePartTwo() {
        var processedInput = Input.Replace('^', '.')
            .Replace('>', '.')
            .Replace('v', '.')
            .Replace('<', '.');

        Trace.WriteLine($"Input contains {processedInput.Count(c => c == '.')} spaces");
        
        return Solve(processedInput);
    }

    private long Solve(string input) {
        var grid = input.SplitPoints();
        var i = 0;
        
        // remember we are traversing backwards, so go against the slope
        var canUps = grid.CoordinatesWhere(c => c == '.' || c == 'v').ToHashSet();
        var canRights = grid.CoordinatesWhere(c => c == '.' || c == '<').ToHashSet();
        var canDowns = grid.CoordinatesWhere(c => c == '.' || c == '^').ToHashSet();
        var canLefts = grid.CoordinatesWhere(c => c == '.' || c == '>').ToHashSet();

        var target = (x: 0, y: 0);
        var start = (x: grid.Max(p => p.x) - 1, y: grid.Max(p => p.y));
        var nextStep = start with { y = start.y - 1 };
        var paths = new List<HashSet<(int, int)>>();
        
        var queue = new PriorityQueue<((int x, int y) position, HashSet<(int x, int y)> set), int>();
        queue.Enqueue((nextStep, new HashSet<(int, int)>() { start }), 1);
            
        var queueHash = new Dictionary<(int x, int y), int> { { (start.x, start.y), 0 }, { (nextStep.x, nextStep.y), 1 } };

        while (queue.TryDequeue(out var pair, out var t)) {
            if (++i % 1000000 == 0)
                Trace.WriteLine($"Queue contains {queue.Count} items, max walk is {paths.Max(p => p.Count)} at time {t}");
            
            pair.set.Add(pair.position);

            if (pair.position.y == target.y) {
                paths.Add(pair.set);
                continue;
            }
            
            var up = pair.position with { y = pair.position.y - 1 };
            var right = pair.position with { x = pair.position.x + 1 };
            var down = pair.position with { y = pair.position.y + 1 };
            var left = pair.position with { x = pair.position.x - 1 };
            
            var canUp    = !pair.set.Contains(up) && canUps.Contains(up);
            var canRight = !pair.set.Contains(right) && canRights.Contains(right);
            var canDown  = !pair.set.Contains(down) && canDowns.Contains(down);
            var canLeft  = !pair.set.Contains(left) && canLefts.Contains(left);

            var tNext = t + 1;

            if (canUp) {
                if (!queueHash.TryGetValue(up, out var ct) || tNext > ct) {
                    queueHash[up] = tNext;
                    queue.Enqueue((up, pair.set), tNext);

                    if (canRight || canDown || canLeft)
                        pair.set = new HashSet<(int x, int y)>(pair.set);
                }
            }
            
            if (canRight) {
                if (!queueHash.TryGetValue(right, out var ct) || tNext > ct) {
                    queueHash[right] = tNext;
                    queue.Enqueue((right, pair.set), tNext);

                    if (canDown || canLeft)
                        pair.set = new HashSet<(int x, int y)>(pair.set);
                }
            }
            
            if (canDown) {
                if (!queueHash.TryGetValue(down, out var ct) || tNext > ct) {
                    queueHash[down] = tNext;
                    queue.Enqueue((down, pair.set), tNext);

                    if (canLeft)
                        pair.set = new HashSet<(int x, int y)>(pair.set);
                }
            }

            if (canLeft) {
                if (!queueHash.TryGetValue(left, out var ct) || tNext > ct) {
                    queueHash[left] = tNext;
                    queue.Enqueue((left, pair.set), tNext);
                }
            }
        }

        return paths.Max(p => p.Count) - 1;
    }

    private const string? ExampleInput = @"
#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartOne();
        Assert.Equal(94, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(154, actual);
    }
}