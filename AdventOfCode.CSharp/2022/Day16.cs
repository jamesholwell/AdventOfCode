using System.Collections.Concurrent;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day16 : Solver {
    private readonly ITestOutputHelper io;

    private static Valve[] InterestingValves;

    public Day16(string? input = null) : base(input) { }

    //public Day16(ITestOutputHelper io, string? input = null) : base(input) {
    //    this.io = io;
    //}

    private class Valve {
        public string Label { get; }

        public int FlowRate { get; }

        public string[] Connections { get; }

        public Dictionary<Valve, int> Distances { get; set; }

        public Valve(string s) {
            var clauses = s.Split(';');
            Label = clauses[0].Substring("Valve ".Length, 2);
            FlowRate = int.Parse(clauses[0].Substring("Valve AA has flow rate=".Length));
            Connections = clauses[1].Substring(" tunnels lead to valve ".Length).Trim().Split(", ");
            Distances = new Dictionary<Valve, int>();
        }

        public override string ToString() => $"Valve {Label} has flow rate={FlowRate}; {(Connections.Length > 1 ? "tunnels lead to valves" : "tunnel leads to valve")} {string.Join(", ", Connections)}";
    }

    private static Valve[] ParseValves(string input) {
        return Shared.Split(input).Select(s => new Valve(s)).ToArray();
    }

    private struct Scenario {
        public Valve CurrentValve;

        public Valve[] VisitedValves;

        public int TimeLeft;

        public int FlowRate = 0;

        public int CurrentPressureReleased = 0;

        public Scenario(Valve currentValve, Valve[] valves, int timeLeft, int flowRate = 0, int currentPressureReleased = 0) {
            CurrentValve = currentValve;
            VisitedValves = valves;
            TimeLeft = timeLeft;
            FlowRate = flowRate;
            CurrentPressureReleased = currentPressureReleased;
        }

        public Scenario MoveToOpen(Valve nextValve, int distance) {
            var timeTaken = (1 + distance);
            return new Scenario(
                nextValve, 
                ArrayCons(nextValve, this.VisitedValves), 
                this.TimeLeft - timeTaken, 
                this.FlowRate + nextValve.FlowRate, 
                this.CurrentPressureReleased + this.FlowRate * timeTaken);
        }

        public int TotalPressureRelease => CurrentPressureReleased + FlowRate * TimeLeft;

        public IEnumerable<KeyValuePair<Valve, int>> Options {
            get
            {
                var visitedValves = VisitedValves;
                var timeLeft = TimeLeft - 1;
                return CurrentValve.Distances.Where(p => !visitedValves.Contains(p.Key) && p.Value < timeLeft);
            }
        }

        public override string ToString() => string.Join("->", VisitedValves.Select(v => v.Label));
    }


    private readonly struct TwoPlayerScenario {
        public readonly Valve CurrentValveOne;

        public readonly Valve CurrentValveTwo;

        public readonly Dictionary<Valve, int> OpenedValves;

        public readonly int TimeLeftOne;

        public readonly int TimeLeftTwo;

        public readonly int TotalPressureRelease;

        public readonly int UpperBoundTotalPressureRelease;

        public TwoPlayerScenario(
            Valve currentValveOne, 
            Valve currentValveTwo,
            Dictionary<Valve, int> openedValves,
            int timeLeftOne, 
            int timeLeftTwo) {
            this.CurrentValveOne = currentValveOne;
            this.CurrentValveTwo = currentValveTwo;
            this.OpenedValves = openedValves;
            this.TimeLeftOne = timeLeftOne;
            this.TimeLeftTwo = timeLeftTwo;

            this.TotalPressureRelease = OpenedValves.Select(p => p.Key.FlowRate * p.Value).Sum();

            var t1 = TimeLeftOne;
            var v1 = CurrentValveOne;
            var t2 = TimeLeftTwo;
            var v2 = CurrentValveTwo;
            this.UpperBoundTotalPressureRelease =
                TotalPressureRelease + 
                InterestingValves
                    .Where(valve => !openedValves.ContainsKey(valve))
                    .Sum(valve => Math.Max(0, Math.Max(t1 - v1.Distances[valve] - 1, t2 - v2.Distances[valve] - 1)) * valve.FlowRate);
        }

        public IEnumerable<(Valve nextOne, int distanceOne, int remainOne, Valve nextTwo, int distanceTwo, int remainTwo)> Options {
            get {
                var visitedValves = OpenedValves;
                var currentOne = CurrentValveOne;
                var currentTwo = CurrentValveTwo;
                var maxDistanceOne = TimeLeftOne;
                var maxDistanceTwo = TimeLeftTwo;

                var optionsOne = CurrentValveOne.Distances.Where(p => !visitedValves.ContainsKey(p.Key) && p.Value < maxDistanceOne).ToArray();
                var optionsTwo = CurrentValveTwo.Distances.Where(p => !visitedValves.ContainsKey(p.Key) && p.Value < maxDistanceTwo).ToArray();

                if (optionsOne.Length == 1 && optionsTwo.Length == 1) {
                    var left = optionsOne.Single();
                    var right = optionsTwo.Single();
                    if (left.Key == right.Key) {
                        if (left.Value < right.Value) {
                            optionsTwo = Array.Empty<KeyValuePair<Valve, int>>();
                        }
                        else {
                            optionsOne = Array.Empty<KeyValuePair<Valve, int>>();
                        }
                    }
                }

                if (optionsOne.Any() && optionsTwo.Any())
                    return 
                        from op1 in optionsOne
                        from op2 in optionsTwo
                        where op1.Key != op2.Key
                        select (op1.Key, op1.Value, optionsOne.Length - 1, op2.Key, op2.Value, optionsTwo.Length - 1);

                if (!optionsTwo.Any())
                    return optionsOne.Select(op1 => (op1.Key, op1.Value, optionsOne.Length - 1, currentTwo, 0, 0));

                if (!optionsOne.Any())
                    return optionsTwo.Select(op2 => (currentOne, 0, 0, op2.Key, op2.Value, optionsTwo.Length - 1));

                throw new InvalidOperationException("No options available");
            }
        }

    }

    public override long SolvePartOne() {
        var valves = InitializeValves();

        var completeScenarios = new List<Scenario>();
        var currentValve = valves.Single(v => v.Label == "AA");
        var scenarios = new List<Scenario> { new(currentValve, new[] { currentValve }, 30) };

        while (scenarios.Any()) {
            var newScenarios = new List<Scenario>();

            foreach (var scenario in scenarios) {
                var options = scenario.Options.ToArray();

                foreach (var (nextValve, distance) in options) {
                    //Console.WriteLine($"Trying {scenario}->{nextValve.Label} for value {(scenario.TimeLeft - distance - 1) * nextValve.FlowRate}");

                    var newScenario = scenario.MoveToOpen(nextValve, distance);

                    if (newScenario.TimeLeft > 0)
                        newScenarios.Add(newScenario);
                    else
                        completeScenarios.Add(newScenario);
                }

                if (!options.Any()) completeScenarios.Add(scenario);
            }

            scenarios = newScenarios;
        }

        var bestScenario = completeScenarios.MaxBy(s => s.TotalPressureRelease);

        Console.WriteLine(bestScenario.ToString());
        return bestScenario.TotalPressureRelease;
    }

    public override long SolvePartTwo() {
        var valves = InitializeValves();
        InterestingValves = valves.Where(v => v.FlowRate > 0).ToArray();

        var i = 0;
        
        var currentValve = valves.Single(v => v.Label == "AA");
        var initialScenario = new TwoPlayerScenario(currentValve, currentValve, new Dictionary<Valve, int> { { currentValve, 0 } }, 26, 26);
        var scenarios = new[] { initialScenario };
        var seenScenarios = new[] { initialScenario };

        while (scenarios.Any()) {
            Console.WriteLine($"Pass {++i} with {scenarios.Length} scenarios to evaluate");

            var nextScenarios = new ConcurrentBag<TwoPlayerScenario>();
            var newlySeenScenarios = new ConcurrentBag<TwoPlayerScenario>();

            scenarios.AsParallel()
                .WithDegreeOfParallelism(8)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .SelectMany(scenario => scenario.Options.Select(op => (scenario, op)))
                .ForAll(pair => {
                    var (scenario, op) = pair;
                    var visitedNodes = scenario.OpenedValves.ToDictionary(p => p.Key, p => p.Value);
                    var timeLeftOne = scenario.TimeLeftOne;
                    var timeLeftTwo = scenario.TimeLeftTwo;

                    if (op.nextOne != scenario.CurrentValveOne) {
                        timeLeftOne -= op.distanceOne + 1;
                        visitedNodes[op.nextOne] = timeLeftOne;
                    }

                    if (op.nextTwo != scenario.CurrentValveTwo) {
                        timeLeftTwo -= op.distanceTwo + 1;
                        visitedNodes[op.nextTwo] = timeLeftTwo;
                    }

                    var newScenario = new TwoPlayerScenario(
                        op.nextOne,
                        op.nextTwo,
                        visitedNodes,
                        timeLeftOne,
                        timeLeftTwo);

                    if (op.remainOne > 0 || op.remainTwo > 0)
                        nextScenarios.Add(newScenario);

                    newlySeenScenarios.Add(newScenario);
                });

            Console.WriteLine($"... done exploring, with {newlySeenScenarios.Count} new scenarios seen");

            if (!newlySeenScenarios.Any()) break;

            var lowerBound = newlySeenScenarios.Max(s => s.TotalPressureRelease);
            Console.WriteLine($"... solution lower bound is {lowerBound}");

            scenarios = nextScenarios.Where(ns => ns.UpperBoundTotalPressureRelease >= lowerBound).ToArray();
            seenScenarios = seenScenarios.Concat(newlySeenScenarios).Where(ns => ns.UpperBoundTotalPressureRelease >= lowerBound).ToArray();
        }
        
        var bestScenario = seenScenarios.MaxBy(s => s.TotalPressureRelease);

        //foreach (var l in bestScenario.OpenedValves.Select(p => $"{p.Key.Label}@{p.Value}"))io.WriteLine(l);
        return bestScenario.TotalPressureRelease;
    }

    private Valve[] InitializeValves() {
        var valves = ParseValves(Input).ToArray();
        for (var i = 0; i < valves.Length; i++) {
            valves[i].Distances =
                Algorithms.Dijkstra(valves[i], valves, v => v.Label, v => v.Connections.Select(c => (c, 1)))
                    .Where(d => d.Key.FlowRate > 0)
                    .ToDictionary(p => p.Key, p => p.Value);
        }

        return valves;
    }

    private static T[] ArrayCons<T>(T element, T[] arr) {
        var r = new T[arr.Length + 1];
        Array.Copy(arr, r, arr.Length);
        r[arr.Length] = element;
        return r;
    }

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
        Assert.Equal(ExampleInput, "\r\n" + string.Join("\r\n", ParseValves(ExampleInput!).Select(v => v.ToString())) + "\r\n");
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day16(ExampleInput).SolvePartOne();
        Assert.Equal(1651, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day16(ExampleInput).SolvePartTwo();
        Assert.Equal(1707, actual);
    }

    public static class Algorithms {
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
    }
}