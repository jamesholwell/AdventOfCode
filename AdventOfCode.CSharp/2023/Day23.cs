using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day23 : Solver {
    public Day23(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var grid = Input.SplitPoints();
        
        // remember we are traversing backwards, so go against the slope
        var canUps = grid.CoordinatesWhere(c => c == '.' || c == 'v').ToHashSet();
        var canRights = grid.CoordinatesWhere(c => c == '.' || c == '<').ToHashSet();
        var canDowns = grid.CoordinatesWhere(c => c == '.' || c == '^').ToHashSet();
        var canLefts = grid.CoordinatesWhere(c => c == '.' || c == '>').ToHashSet();

        var target = (x: 0, y: 0);
        var start = (x: grid.Max(p => p.x) - 1, y: grid.Max(p => p.y));
        var nextStep = start with { y = start.y - 1 };
        var paths = new List<HashSet<(int, int)>>();
        
        var queue = new Queue<((int x, int y) position, HashSet<(int x, int y)> set)>();
        queue.Enqueue((nextStep, new HashSet<(int, int)>() { start }));

        while (queue.TryDequeue(out var pair)) {
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

            if (canUp) {
                queue.Enqueue((up, pair.set));
                
                if (canRight || canDown || canLeft)
                    pair.set = new HashSet<(int x, int y)>(pair.set);
            }
            
            if (canRight) {
                queue.Enqueue((right, pair.set));
                
                if (canDown || canLeft)
                    pair.set = new HashSet<(int x, int y)>(pair.set);
            }
            
            if (canDown) {
                queue.Enqueue((down, pair.set));
                
                if (canLeft)
                    pair.set = new HashSet<(int x, int y)>(pair.set);
            }
            
            if (canLeft) 
                queue.Enqueue((left, pair.set));
        }

        return paths.Max(p => p.Count) - 1;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

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
}