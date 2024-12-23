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
            .ToDictionary(p => p.Key, p => p.Value.ToFrozenSet())
            .ToFrozenDictionary();
    }

    protected override string SolvePartOne() {
        var connections = GetConnections();
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

        return triplets.Count.ToString();
    }

    protected override string SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

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
}