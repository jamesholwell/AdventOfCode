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

        var start = (x: grid.Max(p => p.x) - 1, y: grid.Max(p => p.y));
        var nextStep = start with { y = start.y - 1 };
        var paths = new List<HashSet<(int, int)>>();
        
        var queue = new Queue<((int x, int y) position, HashSet<(int x, int y)> set)>();
        queue.Enqueue((nextStep, new HashSet<(int, int)>() { start }));

        while (queue.TryDequeue(out var pair)) {
            pair.set.Add(pair.position);

            if (pair.position.y == 0) {
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
    
    public override long SolvePartTwo() {
        var grid = Input.SplitPoints();
        var pathTiles = grid.CoordinatesWhere(p => p != '#').ToHashSet();

        var start = (x: 1, y: 0);
        var nextStep = start with { y = 1 };
        var end = (x: grid.Max(p => p.x) - 1, y: grid.Max(p => p.y));

        var nodes = new Dictionary<(int x, int y), Dictionary<(int x, int y), int>> {
            { start, new Dictionary<(int x, int y), int>() },
            { end, new Dictionary<(int x, int y), int>() }
        };

        var queue = new Queue<((int x, int y) position, (int x, int y) origin, (int x, int y) predecessor, int distance)>();
        queue.Enqueue((nextStep, start, start, 1));
        
        while (queue.TryDequeue(out var pair)) {
            if (nodes.TryGetValue(pair.position, out var node)) {
                node[pair.origin] = pair.distance;
                nodes[pair.origin][pair.position] = pair.distance;
                continue;
            }
            
            var up    = pair.position with { y = pair.position.y - 1 };
            var right = pair.position with { x = pair.position.x + 1 };
            var down  = pair.position with { y = pair.position.y + 1 };
            var left  = pair.position with { x = pair.position.x - 1 };
            
            var canUp    = pair.predecessor != up && pathTiles.Contains(up);
            var canRight = pair.predecessor != right && pathTiles.Contains(right);
            var canDown  = pair.predecessor != down && pathTiles.Contains(down);
            var canLeft  = pair.predecessor != left && pathTiles.Contains(left);

            // exactly one option => continue
            if ((canUp ? 1 : 0) + (canRight ? 1 : 0) + (canDown ? 1 : 0) + (canLeft ? 1 : 0) == 1) {
                var next = canUp ? up : canRight ? right : canDown ? down : left;
                queue.Enqueue((next, pair.origin, pair.position, pair.distance + 1));
                continue;
            }
            
            // more than one option => place node and split
            nodes[pair.position] = new Dictionary<(int x, int y), int> { { pair.origin, pair.distance } };
            nodes[pair.origin].Add(pair.position, pair.distance);
            
            if (canUp) 
                queue.Enqueue((up, pair.position, pair.position, 1));
            
            if (canRight)
                queue.Enqueue((right, pair.position, pair.position, 1));

            if (canDown) 
                queue.Enqueue((down, pair.position, pair.position, 1));
            
            if (canLeft) 
                queue.Enqueue((left, pair.position, pair.position, 1));
        }
        
        /*
        // output a mermaid visualization of the network
        var mermaid = new List<(int, string)>();
        foreach (var node in nodes)
            foreach (var neighbour in node.Value)
                mermaid.Add((node.Key.x + node.Key.y, $"  x{node.Key.x}y{node.Key.y} --{neighbour.Value}--> x{neighbour.Key.x}y{neighbour.Key.y}"));
        
        Output.WriteLine("flowchart TD");
        foreach (var mermaidLine in mermaid.OrderBy(m => m.Item1))
            Output.WriteLine(mermaidLine.Item2);
        */

        var exitDistance = 0;
        if (nodes[end].Count == 1) {
            Trace.WriteLine("Detected exit node: applying speedup by moving goal one node closer");
            exitDistance = nodes[end].Values.Single();
            end = nodes[end].Keys.Single();
        }
        
        Trace.WriteLine($"Beginning brute-force search of {nodes.Count} nodes");
        var pathQueue = new Queue<((int x, int y) position, int distance, HashSet<(int x, int y)> set)>();
        pathQueue.Enqueue((start, 0, new HashSet<(int, int)> { start }));

        int i = 0, maxDistance = 0;
        while (pathQueue.TryDequeue(out var pair)) {
            if (++i % 1e6 == 0) 
                Trace.WriteLine($"Status at {i / 1e6}M dequeues: {pathQueue.Count / 1000:N0}k in queue; longest path detected has length {maxDistance}; current path has visited {pair.set.Count} nodes");
            
            pair.set.Add(pair.position);
            
            if (pair.position == end) {
                if (pair.distance > maxDistance)
                    maxDistance = pair.distance;

                continue;
            }
            
            var n = 0; // reuse the hash set for the first fork
            foreach (var neighbour in nodes[pair.position].Where(neighbour => !pair.set.Contains(neighbour.Key)))
                pathQueue.Enqueue((neighbour.Key, pair.distance + neighbour.Value, n++ == 0 ? pair.set : new HashSet<(int x, int y)>(pair.set)));
        }

        return maxDistance + exitDistance;
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
    
    private const string SmallExample = @"
#.###
#...#
#.#.#
#...#
#.#.#
#...#
###.#
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartOne();
        Assert.Equal(94, actual);
    }
    
    [Fact]
    public void SolvesSmallExample() {
        var actual = new Day23(SmallExample, Output).SolvePartTwo();
        Assert.Equal(12, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(154, actual);
    }
}