using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day17 : Solver {
    private int[,] grid = null!;
    private int width;
    private int height;
    
    public Day17(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }
    
    public override long SolvePartOne() {
        // initialize the grid
        InitializeGrid();

        // run A* over the grid
        (int x, int y, Heading h, int l) startNode = (0, 0, Heading.None, 0);
        var (minCost, path) = Algorithms.AStar(startNode, Connections, Heuristic, IsGoal);

        DrawPath(path);

        // return the result
        return minCost;
        
        IEnumerable<((int x, int y, Heading h, int l), int c)> Connections((int x, int y, Heading h, int l) node) {
            // up
            if (node.h != Heading.Down && node.y > 0 && (node.h != Heading.Up || node.l < 3))
                yield return (node with { y = node.y - 1, h = Heading.Up, l = node.h == Heading.Up ? node.l + 1 : 1 }, grid[node.y - 1, node.x]);
            
            // right
            if (node.h != Heading.Left && node.x < width - 1 && (node.h != Heading.Right || node.l < 3))
                yield return (node with { x = node.x + 1, h = Heading.Right, l = node.h == Heading.Right ? node.l + 1 : 1 }, grid[node.y, node.x + 1]);
            
            // down
            if (node.h != Heading.Up && node.y < height - 1 && (node.h != Heading.Down || node.l < 3))
                yield return (node with { y = node.y + 1, h = Heading.Down, l = node.h == Heading.Down ? node.l + 1 : 1 }, grid[node.y + 1, node.x]);
            
            // left
            if (node.h != Heading.Right && node.x > 0 && (node.h != Heading.Left || node.l < 3))
                yield return (node with { x = node.x - 1, h = Heading.Left, l = node.h == Heading.Left ? node.l + 1 : 1 }, grid[node.y, node.x - 1]);
        }

        int Heuristic((int x, int y, Heading h, int l) p) => width - 1 - p.x + height - 1 - p.y;
        
        bool IsGoal((int x, int y, Heading h, int l) node) => node.x == width - 1 && node.y == height - 1;
    }

    public override long SolvePartTwo() {
        // initialize the grid
        InitializeGrid();

        // run A* over the grid
        (int x, int y, Heading h, int l) startNode = (0, 0, Heading.None, 0);
        var (minCost, path) = Algorithms.AStar(startNode, Connections, Heuristic, IsGoal);

        DrawPath(path);

        // return the result
        return minCost;
        
        IEnumerable<((int x, int y, Heading h, int l), int c)> Connections((int x, int y, Heading h, int l) node) {
            // up
            if (node.h != Heading.Down)
                if (node.h != Heading.Up && node.y > 3)
                    yield return (node with { y = node.y - 4, h = Heading.Up, l = 4 }, grid[node.y - 1, node.x] + grid[node.y - 2, node.x] + grid[node.y - 3, node.x] + grid[node.y - 4, node.x]);
                else if (node is { h: Heading.Up, l: < 10, y: > 0 })
                    yield return (node with { y = node.y - 1, h = Heading.Up, l = node.l + 1 }, grid[node.y - 1, node.x]);

            // right
            if (node.h != Heading.Left)
                if (node.h != Heading.Right && node.x < width - 4)
                    yield return (node with { x = node.x + 4, h = Heading.Right, l = 4 }, grid[node.y, node.x + 1] + grid[node.y, node.x + 2] + grid[node.y, node.x + 3] + grid[node.y, node.x + 4]);
                else if (node is { h: Heading.Right, l: < 10 } && node.x < width - 1)
                    yield return (node with { x = node.x + 1, h = Heading.Right, l = node.l + 1 }, grid[node.y, node.x + 1]);
            
            // down
            if (node.h != Heading.Up)
                if (node.h != Heading.Down && node.y < height - 4)
                    yield return (node with { y = node.y + 4, h = Heading.Down, l = 4 }, grid[node.y + 1, node.x] + grid[node.y + 2, node.x] + grid[node.y + 3, node.x] + grid[node.y + 4, node.x]);
                else if (node is { h: Heading.Down, l: < 10 } && node.y < height - 1)
                    yield return (node with { y = node.y + 1, h = Heading.Down, l = node.l + 1 }, grid[node.y + 1, node.x]);
            
            // left
            if (node.h != Heading.Right)
                if (node.h != Heading.Left && node.x > 3)
                    yield return (node with { x = node.x - 4, h = Heading.Left, l = 4 }, grid[node.y, node.x - 1] + grid[node.y, node.x - 2] + grid[node.y, node.x - 3] + grid[node.y, node.x - 4]);
                else if (node is { h: Heading.Left, l: < 10, x: > 0 })
                    yield return (node with { x = node.x - 1, h = Heading.Left, l = node.l + 1 }, grid[node.y, node.x - 1]);
        }

        int Heuristic((int x, int y, Heading h, int l) p) => width - 1 - p.x + height - 1 - p.y;
        
        bool IsGoal((int x, int y, Heading h, int l) node) => node.x == width - 1 && node.y == height - 1;
    }

    private void DrawPath((int x, int y, Heading h, int l)[] path) {
        var pathMap = grid.Map((x, y, _) => (char)('0' + grid[y, x]));

        var lastHeading = Heading.None;
        foreach (var step in path) {
            if (step.h == Heading.None) continue;
            
            var symbol = step.h switch {
                Heading.Up => '^',
                Heading.Right => '>',
                Heading.Down => 'v',
                Heading.Left => '<',
                _ => throw new ArgumentOutOfRangeException()
            };

            pathMap[step.y, step.x] = symbol;

            if (lastHeading != step.h) {
                lastHeading = step.h;

                if (step.l == 4) {
                    // it's the first step of part two
                    var dx = step.h switch {
                        Heading.Right => -1,
                        Heading.Left => 1,
                        _ => 0
                    };

                    var dy = step.h switch {
                        Heading.Up => 1,
                        Heading.Down => -1,
                        _ => 0
                    };

                    for (var i = 1; i < 4; i++) {
                        pathMap[step.y + i * dy, step.x + i * dx] = symbol;
                    } 
                } 
            }
        }
        
        Trace.WriteLine(pathMap.Render());
    }

    private void InitializeGrid() {

        grid = Input.SplitGrid(c => c - '0');
        width = grid.Width();
        height = grid.Height();
    }

    private const string? ExampleInput1 = @"
2413432311323
3215453535623
3255245654254
3446585845452
4546657867536
1438598798454
4457876987766
3637877979653
4654967986887
4564679986453
1224686865563
2546548887735
4322674655533
";
    
    private const string? ExampleInput2 = @"
111111111111
999999999991
999999999991
999999999991
999999999991
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day17(ExampleInput1, Output).SolvePartOne();
        Assert.Equal(102, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample1() {
        var actual = new Day17(ExampleInput1, Output).SolvePartTwo();
        Assert.Equal(94, actual);
    }
    
    
    [Fact]
    public void SolvesPartTwoExample2() {
        var actual = new Day17(ExampleInput2, Output).SolvePartTwo();
        Assert.Equal(71, actual);
    }

    private enum Heading {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8
    }
    
    private static class Algorithms {
        /// <summary>
        ///     A* (pronounced "A-star") is a graph traversal and path search algorithm
        /// </summary>
        /// <see href="https://en.wikipedia.org/wiki/A*_search_algorithm"/>
        /// <param name="startNode">start position on the graph</param>
        /// <param name="connections">neighbour function (returns connected node and edge cost)</param>
        /// <param name="heuristic">estimated cost to the goal: must be a lower bound for the actual minimum cost</param>
        /// <param name="isGoal">goal detection predicate (supports generative networks where a class of nodes are the goal)</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static (int, T[] path) AStar<T>(
            T startNode,
            Func<T, IEnumerable<(T, int)>> connections,
            Func<T, int> heuristic,
            Func<T, bool> isGoal) where T : struct { 
            // the prioritised queue of where to explore next
            var queue = new PriorityQueue<T, int>();
            queue.Enqueue(startNode, heuristic(startNode));
        
            // the same queue as a set, so we can avoid duplicate entries
            var set = new HashSet<T> { startNode };

            // the set of all predecessors (for reconstruction)
            var predecessors = new Dictionary<T, T>();
            
            // the working set of minimum costs to the node
            var workingCosts = new Dictionary<T, int> {
                {startNode, 0}
            };

            // consume the queue in priority order until we find a goal
            while (queue.TryDequeue(out var currentNode, out _)) {
                if (isGoal(currentNode)) {
                    var reconstructionStack = new Stack<T>();
                    reconstructionStack.Push(currentNode);

                    while (predecessors.TryGetValue(reconstructionStack.Peek(), out var precedingNode))
                        reconstructionStack.Push(precedingNode);
                    
                    return (workingCosts[currentNode], reconstructionStack.ToArray());
                }
                
                set.Remove(currentNode);
                var currentCost = workingCosts[currentNode];

                foreach (var (node, edgeCost) in connections(currentNode)) {
                    var tentativeCost = currentCost + edgeCost;

                    // check if this path is better than any we've seen before
                    if (workingCosts.TryGetValue(node, out var workingCost) 
                        && tentativeCost >= workingCost) 
                        continue;
                    
                    // update our working costs and predecessor map
                    workingCosts[node] = tentativeCost;
                    predecessors[node] = currentNode;
                    
                    // NOTE: this is technically wrong, as we might have found a faster path
                    // and hence could go up the queue, but let's hope not ¯\_(ツ)_/¯
                    if (set.Contains(node)) 
                        continue;
                        
                    // this is the 'magic' part, where we prioritise the next nodes
                    // based on the heuristic estimate of the cost to the goal
                    queue.Enqueue(node, tentativeCost + heuristic(node));
                    set.Add(node);
                }
            }

            throw new InvalidOperationException("No route found");
        }
    }
}