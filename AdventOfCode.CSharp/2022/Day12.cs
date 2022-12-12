using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day12 : Solver {
    private readonly ITestOutputHelper? io;

    private Dictionary<Tuple<int, int>, int> unvisited;

    private Dictionary<Tuple<int, int>, int> tentative;

    private Dictionary<Tuple<int, int>, int> elevations;

    //public Day12(string? input = null, ITestOutputHelper? io = null) : base(input) {
    //    this.io = io;
    //}

    public Day12(string? input = null) : base(input) { }

    public override long SolvePartOne() {
        var lines = Shared.Split(Input);

        var map = lines.SelectMany((l, y) => l.Trim().ToCharArray().Select((c, x) =>
            new KeyValuePair<Tuple<int, int>, char>(Tuple.Create(x, y), c))).ToArray();

        elevations = map.Select(p => 
            new KeyValuePair<Tuple<int, int>, int>(p.Key,
                p.Value == 'S' ? 0 : p.Value == 'E' ? 'z' - 'a' : (int) p.Value - (int) 'a')).ToDictionary(p => p.Key, p => p.Value);

        var height = lines.Length;
        var width = lines[0].Length;

        unvisited = elevations.ToDictionary(p => p.Key, p => int.MaxValue);
        tentative = elevations.ToDictionary(p => p.Key, p => int.MaxValue);
        var start = map.Single(m => m.Value == 'S').Key;
        var end = map.Single(m => m.Value == 'E').Key;
        unvisited[start] = 0;
        tentative[start] = 0;

        while (unvisited.Any()) {
            var current = unvisited.MinBy(p => p.Value).Key;

            // up
            if (current.Item2 > 0) {
                Visit(current, Tuple.Create(current.Item1, current.Item2 - 1));
            }

            // down
            if (current.Item2 < height - 1) {
                Visit(current, Tuple.Create(current.Item1, current.Item2 + 1));
            }

            // left
            if (current.Item1 > 0) {
                Visit(current, Tuple.Create(current.Item1 - 1, current.Item2));
            }

            // down
            if (current.Item1 < width - 1) {
                Visit(current, Tuple.Create(current.Item1 + 1, current.Item2));
            }

            unvisited.Remove(current);
        }

        return tentative[end]        ;
    }

    private void Visit(Tuple<int, int> current, Tuple<int, int> prospect) {
        var isConnected = elevations[prospect] - elevations[current] < 2;
        var isUnvisited = unvisited.ContainsKey(prospect);

        if (isConnected && isUnvisited && tentative[current] < tentative[prospect]) {
            unvisited[prospect] = tentative[current] + 1;
            tentative[prospect] = tentative[current] + 1;
        }
    }

    private const string? ExampleInput = @"
Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day12(ExampleInput).SolvePartOne();
        Assert.Equal(31, actual);
    }
}