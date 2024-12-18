namespace AdventOfCode.Core.Algorithms;

public static partial class Algorithm {
    public static Dictionary<T, int> Dijkstra<T>(
        T startNode,
        IEnumerable<T> nodes,
        Func<T, string> labelFunc,
        Func<T, IEnumerable<(string, int)>> connectionFunc) where T : notnull {
        var startLabel = labelFunc(startNode);
        var nodeLookup = nodes.ToDictionary(labelFunc, n => n);

        var visited = new HashSet<string>(new[] { startLabel });
        var unvisitedQueue = new PriorityQueue<string, int>();
        unvisitedQueue.Enqueue(startLabel, 0);

        var tentative = nodeLookup.Keys.ToDictionary(k => k, _ => int.MaxValue);
        tentative[startLabel] = 0;

        while (unvisitedQueue.TryDequeue(out var current, out _)) {
            foreach (var (neighbor, edgeDistance) in connectionFunc(nodeLookup[current])) {
                if (visited.Contains(neighbor)) continue;

                var newDistance = tentative[current] + edgeDistance;
                if (newDistance < tentative[neighbor]) {
                    tentative[neighbor] = newDistance;
                    unvisitedQueue.Enqueue(neighbor, newDistance);
                }
            }

            visited.Add(current);
        }

        return tentative.ToDictionary(p => nodeLookup[p.Key], p => p.Value);
    }
    
    public static Dictionary<T, int> Dijkstra<T, TLabel> (
        T startNode,
        IEnumerable<T> nodes,
        Func<T, TLabel> labelFunc,
        Func<T, IEnumerable<(TLabel, int)>> connectionFunc) where T : notnull where TLabel : notnull {
        var startLabel = labelFunc(startNode);
        var nodeLookup = nodes.ToDictionary(labelFunc, n => n);

        var visited = new HashSet<TLabel>(new[] { startLabel });
        var unvisitedQueue = new PriorityQueue<TLabel, int>();
        unvisitedQueue.Enqueue(startLabel, 0);

        var tentative = nodeLookup.Keys.ToDictionary(k => k, _ => int.MaxValue);
        tentative[startLabel] = 0;

        while (unvisitedQueue.TryDequeue(out var current, out _)) {
            foreach (var (neighbor, edgeDistance) in connectionFunc(nodeLookup[current])) {
                if (visited.Contains(neighbor)) continue;

                var newDistance = tentative[current] + edgeDistance;
                if (newDistance < tentative[neighbor]) {
                    tentative[neighbor] = newDistance;
                    unvisitedQueue.Enqueue(neighbor, newDistance);
                }
            }

            visited.Add(current);
        }

        return tentative.ToDictionary(p => nodeLookup[p.Key], p => p.Value);
    }
}