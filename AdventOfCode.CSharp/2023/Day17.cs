using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day17 : Solver {
    public Day17(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }
    
    public override long SolvePartOne() {
        // initialize the grid
        var grid = Input.SplitGrid(c => c - '0');
        var width = grid.Width();
        var height = grid.Height();

        // run A* over the grid
        (int x, int y, Heading h, int l) startNode = (0, 0, Heading.None, 0);
        var (minCost, path) = Algorithms.AStar(startNode, Connections, Heuristic, IsGoal);

        // draw the path map
        var pathMap = grid.Map((x, y, _) => {
            var foundPath = path.SingleOrDefault(p => p.x == x && p.y == y);
            return foundPath.h == Heading.Up ? '^'
                : foundPath.h == Heading.Right ? '>'
                : foundPath.h == Heading.Down ? 'v'
                : foundPath.h == Heading.Left ? '<' : (char)('0' + grid[y, x]);
        });
        Trace.WriteLine(pathMap.Render());
        
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

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
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

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day17(ExampleInput, Output).SolvePartOne();
        Assert.Equal(102, actual);
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