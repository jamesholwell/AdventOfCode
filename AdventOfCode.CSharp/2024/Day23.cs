using System.Collections.Frozen;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day23(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver<string>(input, outputHelper) {
    private (string left, string right)[] Parse(string input) =>
        Shared.Split(input).Select(s => {
            var parts = s.Split("-");
            return (parts[0], parts[1]);
        }).ToArray();

    private FrozenDictionary<string, FrozenSet<string>> GetConnections() {
        var connections = new Dictionary<string, HashSet<string>>();

        foreach (var connection in Parse(Input)) {
            if (!connections.TryGetValue(connection.left, out var left))
                connections[connection.left] = [connection.right];
            else
                left.Add(connection.right);

            if (!connections.TryGetValue(connection.right, out var right))
                connections[connection.right] = [connection.left];
            else
                right.Add(connection.left);
        }

        return connections
            .ToDictionary(p => p.Key, p => p.Value.OrderBy(v => v).ToFrozenSet())
            .ToFrozenDictionary();
    }

    protected override string SolvePartOne() {
        var connections = GetConnections();

        return GetTriplets(connections).Count.ToString();
    }

    private static HashSet<string> GetTriplets(FrozenDictionary<string, FrozenSet<string>> connections) {
        var triplets = new HashSet<string>();

        foreach (var connection in connections.Where(p => p.Key.StartsWith('t'))) {
            var node = connection.Value.ToArray();

            for (var i = 0; i < node.Length; ++i) {
                for (var j = i + 1; j < node.Length; ++j) {
                    if (connections[node[i]].Contains(node[j]))
                        triplets.Add(string.Join(',', new[] { connection.Key, node[i], node[j] }.OrderBy(v => v)));
                }
            }
        }

        return triplets;
    }

    protected override string SolvePartTwo() {
        var connections = GetConnections();
        
        // hey, we already did some of the work in part 1!
        var seeds = GetTriplets(connections)
            .Select(t => (key: t, working: t.Split(',')))
            .Select(i => (
                i.key,
                i.working, 
                remaining: connections[i.working[0]].Except(i.working).ToArray()));

        var seen = new HashSet<string>();
        var queue = new Queue<(string key, string[] working, string[] remaining)>(seeds);
        var longestSet = string.Empty;

        while (queue.TryDequeue(out var item)) {
            // calculate the options for the next step
            var didQueue = false;

            foreach (var nextNode in item.remaining) {
                if (!connections[nextNode].IsSupersetOf(item.working))
                    continue;

                /*
                 * The following is equivalent to
                 *  
                 *      var nextWorking = item.working.Concat([nextNode]).ToArray();
                 *      Array.Sort(nextWorking);
                 *  
                 *      var nextKey = string.Join(',', nextWorking);
                 *
                 *  But leverages the already-ordered nature of the working set
                 * 
                 */
                
                // fast initialise next working set
                var nextWorking = new string[item.working.Length + 1];
                var place = 0;
                while (place < item.working.Length && string.Compare(item.working[place], nextNode, StringComparison.Ordinal) < 0)
                    ++place;
                Array.Copy(item.working, nextWorking, place);
                nextWorking[place] = nextNode;
                if (place <= item.working.Length)
                    Array.Copy(item.working, place, nextWorking, place + 1, item.working.Length - place);
                    
                // fast initialise next key
                var nextKey =
                    place == item.working.Length
                        ? $"{item.key},{nextNode}"
                        : $"{item.key[..(place * 3)]}{nextNode},{item.key[(place * 3)..]}";
                
                // test if we've already seen this set
                if (seen.Add(nextKey))
                    queue.Enqueue((nextKey, nextWorking, item.remaining[1..]));
                    
                didQueue = true;
            }

            // if we couldn't get any bigger, see if we were the biggest so far
            if (!didQueue && item.key.Length > longestSet.Length)
                longestSet = item.key;
        }

        return longestSet;
    }

    private const string? ExampleInput =
        """
        kh-tc
        qp-kh
        de-cg
        ka-co
        yn-aq
        qp-ub
        cg-tb
        vc-aq
        tb-ka
        wh-tc
        yn-cg
        kh-ub
        ta-co
        de-co
        tc-td
        tb-wq
        wh-td
        ta-ka
        td-qp
        aq-cg
        wq-ub
        ub-vc
        de-ta
        wq-aq
        wq-vc
        wh-yn
        ka-de
        kh-ta
        co-tc
        wh-qp
        tb-vc
        td-yn
        """;

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartOne();
        Assert.Equal("7", actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day23(ExampleInput, Output).SolvePartTwo();
        Assert.Equal("co,de,ka,ta", actual);
    }
}