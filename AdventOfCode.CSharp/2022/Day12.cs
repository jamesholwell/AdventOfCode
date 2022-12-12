using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day12(string? input = null, ITestOutputHelper? outputHelper = null) : Solver(input, outputHelper) {
    private long Solve(bool isPartTwo = false) {
        var lines = Shared.Split(Input);

        var map = lines.SelectMany((l, y) => l.Trim().ToCharArray().Select((c, x) =>
            new KeyValuePair<Tuple<int, int>, char>(Tuple.Create(x, y), c))).ToArray();

        var elevations = map.Select(p =>
                new KeyValuePair<Tuple<int, int>, int>(p.Key,
                    p.Value == 'S' ? 0 : p.Value == 'E' ? 'z' - 'a' : p.Value - 'a'))
            .ToDictionary(p => p.Key, p => p.Value);

        var height = lines.Length;
        var width = lines[0].Length;

        var unvisited = elevations.ToDictionary(p => p.Key, _ => int.MaxValue);
        var tentative = elevations.ToDictionary(p => p.Key, _ => int.MaxValue);
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

        return tentative[end];
        
        void Visit(Tuple<int, int> current, Tuple<int, int> prospect) {
            var isConnected = elevations[prospect] - elevations[current] < 2;
            var isUnvisited = unvisited.ContainsKey(prospect);

            if (isConnected && isUnvisited && tentative[current] < tentative[prospect]) {
                var i = !isPartTwo ? 1 : elevations[prospect] == 0 ? 0 : 1;
                unvisited[prospect] = tentative[current] + i;
                tentative[prospect] = tentative[current] + i;
            }
        }
    }

    protected override long SolvePartOne() => Solve();
    
    protected override long SolvePartTwo() => Solve(true);

    private const string? ExampleInput = @"
Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day12(ExampleInput, Output).SolvePartOne();
        Assert.Equal(31, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day12(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(29, actual);
    }
}