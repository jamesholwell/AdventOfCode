using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day16 : Solver {
    private readonly ITestOutputHelper io;

   // public Day16(string? input = null) : base(input) { }

    public Day16(ITestOutputHelper io, string? input = null) : base(input) {
        this.io = io;
    }

    private struct Valve {
        public string Label { get; }

        public int FlowRate { get; }

        public string[] Connections { get; }

        public Valve(string s) {
            var clauses = s.Split(';');
            Label = clauses[0].Substring("Valve ".Length, 2);
            FlowRate = int.Parse(clauses[0].Substring("Valve AA has flow rate=".Length));
            Connections = clauses[1].Substring(" tunnels lead to valve ".Length).Trim().Split(", ");
        }

        public override string ToString() => $"Valve {Label} has flow rate={FlowRate}; {(Connections.Length > 1 ? "tunnels lead to valves" : "tunnel leads to valve")} {string.Join(", ", Connections)}";
    }

    private static Valve[] ParseValves(string input) {
        return Shared.Split(input).Select(s => new Valve(s)).ToArray();
    }

    public override long SolvePartOne() {
        var valves = ParseValves(Input);

        foreach (var valve in valves) {
            io.WriteLine(valve.ToString());
        }

        return 0;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II
";

    [Fact]
    public void ParsesCorrectly() {
        Assert.Equal(ExampleInput, "\r\n" + string.Join("\r\n", ParseValves(ExampleInput!)) + "\r\n");
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day16(io, ExampleInput).SolvePartOne();
        Assert.Equal(0, actual);
    }
}