namespace AdventOfCode.Core.Algorithms;

public static class Algorithm {
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

        // the set of all predecessors (for reconstruction)
        var predecessors = new Dictionary<T, T>();

        // the working set of minimum costs to the node
        var workingCosts = new Dictionary<T, int> {
            { startNode, 0 }
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

                // this is the 'magic' part, where we prioritise the next nodes
                // based on the heuristic estimate of the cost to the goal
                queue.Enqueue(node, tentativeCost + heuristic(node));
            }
        }

        throw new InvalidOperationException("No route found");
    }
    
    public static IEnumerable<T[]> AllStars<T>(
        T startNode,
        Func<T, IEnumerable<(T, int)>> connections,
        Func<T, int> heuristic,
        Func<T, bool> isGoal) where T : struct {
        // the prioritised queue of where to explore next
        var queue = new PriorityQueue<T, int>();
        queue.Enqueue(startNode, heuristic(startNode));

        // the set of all predecessors (for reconstruction)
        var predecessors = new Dictionary<T, HashSet<T>>();

        // the working set of minimum costs to the node
        var workingCosts = new Dictionary<T, int> {
            { startNode, 0 }
        };
        
        // the minimum path length and goal set
        var minimumCost = default(int?);
        var goalNodes = new HashSet<T>();

        // consume the queue in priority order until we find a goal
        while (queue.TryDequeue(out var currentNode, out _)) {
            var currentCost = workingCosts[currentNode];

            // if we are already above the minimum length, give up
            if (minimumCost < currentCost)
                continue;
            
            // record a new goal reached
            if (isGoal(currentNode)) {
                minimumCost = currentCost;
                goalNodes.Add(currentNode);
                continue;
            }

            // consider the possible edge traversals
            foreach (var (node, edgeCost) in connections(currentNode)) {
                var tentativeCost = currentCost + edgeCost;

                // check if this path is not worse than any we've seen before
                if (workingCosts.TryGetValue(node, out var workingCost)
                    && tentativeCost > workingCost)
                    continue;

                // update our working costs and predecessor map
                workingCosts[node] = tentativeCost;
                if (!predecessors.ContainsKey(node))
                    predecessors[node] = [];    
                predecessors[node].Add(currentNode);

                // this is the 'magic' part, where we prioritise the next nodes
                // based on the heuristic estimate of the cost to the goal
                queue.Enqueue(node, tentativeCost + heuristic(node));
            }
        }

        return goalNodes.SelectMany(Reconstruct);

        IEnumerable<T[]> Reconstruct(T node) {
            foreach (var predecessor in predecessors[node]) {
                if (!predecessors.ContainsKey(predecessor)) 
                    yield return [predecessor]; 
                else  
                    foreach (var reconstruction in Reconstruct(predecessor))
                        yield return reconstruction.Concat([node]).ToArray();
            }
        }
    }
}