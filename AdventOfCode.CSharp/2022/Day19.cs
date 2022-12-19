using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day19 : Solver {
    public Day19(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private struct Blueprint {
        public int Id;

        public int OreRobotOreCost;

        public int ClayRobotOreCost;

        public int ObsidianRobotOreCost;

        public int ObsidianRobotClayCost;

        public int GeodeRobotOreCost;

        public int GeodeRobotObsidianCost;

        public Blueprint(string s) {
            var outsideParts = s.Split(':');
            Id = int.Parse(outsideParts[0].Substring("Blueprint ".Length));

            // tell csharp compiler to be quiet
            OreRobotOreCost = 0;
            ClayRobotOreCost = 0;
            ObsidianRobotOreCost = 0;
            ObsidianRobotClayCost = 0;
            GeodeRobotOreCost = 0;
            GeodeRobotObsidianCost = 0;

            foreach (var clause in outsideParts[1].Split(". ")) {
                var parts = clause.Trim().Split(" ");

                switch (parts[1]) {
                    case "ore":
                        OreRobotOreCost = int.Parse(parts[4]);
                        break;

                    case "clay":
                        ClayRobotOreCost = int.Parse(parts[4]);
                        break;

                    case "obsidian":
                        ObsidianRobotOreCost = int.Parse(parts[4]);
                        ObsidianRobotClayCost = int.Parse(parts[7]);
                        break;

                    case "geode":
                        GeodeRobotOreCost = int.Parse(parts[4]);
                        GeodeRobotObsidianCost = int.Parse(parts[7]);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public override string ToString() => 
            $"Blueprint {Id}: " +
            $"Each ore robot costs {OreRobotOreCost} ore. " +
            $"Each clay robot costs {ClayRobotOreCost} ore. " +
            $"Each obsidian robot costs {ObsidianRobotOreCost} ore and {ObsidianRobotClayCost} clay. " +
            $"Each geode robot costs {GeodeRobotOreCost} ore and {GeodeRobotObsidianCost} obsidian.";


        public string ToPrettyString() =>
            $"Blueprint {Id}: " + Environment.NewLine +
            $"  Each ore robot costs {OreRobotOreCost} ore. " + Environment.NewLine +
            $"  Each clay robot costs {ClayRobotOreCost} ore. " + Environment.NewLine +
            $"  Each obsidian robot costs {ObsidianRobotOreCost} ore and {ObsidianRobotClayCost} clay. " + Environment.NewLine +
            $"  Each geode robot costs {GeodeRobotOreCost} ore and {GeodeRobotObsidianCost} obsidian.";
    }

    private readonly struct Scenario {
        public readonly Blueprint Blueprint;

        public readonly int Ores;

        public readonly int Clays;

        public readonly int Obsidians;

        public readonly int Geodes;

        public readonly int OreRobots;

        public readonly int ClayRobots;

        public readonly int ObsidianRobots;

        public readonly int GeodeRobots;

        public readonly int TimeLeft;

        public Scenario(Blueprint blueprint, int ores, int clays, int obsidians, int geodes, int oreRobots, int clayRobots, int obsidianRobots, int geodeRobots, int timeLeft) {
            Blueprint = blueprint;
            Ores = ores;
            Clays = clays;
            Obsidians = obsidians;
            Geodes = geodes;
            OreRobots = oreRobots;
            ClayRobots = clayRobots;
            ObsidianRobots = obsidianRobots;
            GeodeRobots = geodeRobots;
            TimeLeft = timeLeft;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario Tick() {
            return new Scenario(Blueprint, Ores + OreRobots, Clays + ClayRobots, Obsidians + ObsidianRobots,
                Geodes + GeodeRobots, OreRobots, ClayRobots, ObsidianRobots, GeodeRobots, TimeLeft - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario BuildGeodeRobot() {
            return new Scenario(Blueprint, Ores + OreRobots - Blueprint.GeodeRobotOreCost, Clays + ClayRobots, Obsidians + ObsidianRobots - Blueprint.GeodeRobotObsidianCost,
                Geodes + GeodeRobots, OreRobots, ClayRobots, ObsidianRobots, GeodeRobots + 1, TimeLeft - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario BuildObsidianRobot() {
            return new Scenario(Blueprint, Ores + OreRobots - Blueprint.ObsidianRobotOreCost, Clays + ClayRobots - Blueprint.ObsidianRobotClayCost, Obsidians + ObsidianRobots,
                Geodes + GeodeRobots, OreRobots, ClayRobots, ObsidianRobots + 1, GeodeRobots, TimeLeft - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario BuildClayRobot() {
            return new Scenario(Blueprint, Ores + OreRobots - Blueprint.ClayRobotOreCost, Clays + ClayRobots, Obsidians + ObsidianRobots,
                Geodes + GeodeRobots, OreRobots, ClayRobots + 1, ObsidianRobots, GeodeRobots, TimeLeft - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario BuildOreRobot() {
            return new Scenario(Blueprint, Ores + OreRobots - Blueprint.OreRobotOreCost, Clays + ClayRobots, Obsidians + ObsidianRobots,
                Geodes + GeodeRobots, OreRobots + 1, ClayRobots, ObsidianRobots, GeodeRobots, TimeLeft - 1);
        }
    }

    public override long SolvePartOne() {
        var blueprints = ParseBlueprints(Input);
        var scenarios = blueprints.Select(b => new Scenario(b, 0, 0, 0, 0, 1, 0, 0, 0, 24)).ToArray();
        var i = 0;
        var lowerBounds = blueprints.ToDictionary(b => b, _ => 0);

        while (scenarios.Any()) {
            Output.WriteLine($"Pass {++i} with {scenarios.Length} scenarios to evaluate");

            var newlySeenScenarios = new ConcurrentBag<Scenario>();

            scenarios.AsParallel().ForAll(scenario =>
            {
                var blueprint = scenario.Blueprint;
                if (scenario.TimeLeft <= 0) return;
                var builtARobot = false;

                // build geode robot
                if (scenario.Ores >= blueprint.GeodeRobotOreCost &&
                    scenario.Obsidians >= blueprint.GeodeRobotObsidianCost) {
                    newlySeenScenarios.Add(scenario.BuildGeodeRobot());

                    // assumption: this is always the best action (?)
                    return;
                }

                // build obsidian robot
                if (scenario.Ores >= blueprint.ObsidianRobotOreCost &&
                    scenario.Clays >= blueprint.ObsidianRobotClayCost) {
                    newlySeenScenarios.Add(scenario.BuildObsidianRobot());

                    // assumption: this is always the best action if you don't already have production (?)
                    if (scenario.Obsidians == 0)
                        return;
                }
                
                // build clay robot
                if (scenario.Ores >= blueprint.ClayRobotOreCost && scenario.ClayRobots < 8) {
                    newlySeenScenarios.Add(scenario.BuildClayRobot());

                    // assumption: this is always the best action if you don't already have production (?)
                    if (scenario.Clays == 0)
                        return;
                }

                // build ore robot
                if (scenario.Ores >= blueprint.OreRobotOreCost && scenario.OreRobots < 5) {
                    newlySeenScenarios.Add(scenario.BuildOreRobot());
                }

                newlySeenScenarios.Add(scenario.Tick());
            });
            
            Output.WriteLine($"... done exploring, with {newlySeenScenarios.Count} new scenarios seen");

            scenarios = newlySeenScenarios.ToArray();

            // write out lower bounds
            foreach (var scenariosForBlueprint in scenarios.GroupBy(b => b.Blueprint)) {
                var max = scenariosForBlueprint.Max(s => s.Geodes);

                if (max > lowerBounds[scenariosForBlueprint.Key])
                    lowerBounds[scenariosForBlueprint.Key] = max;
            }
        }

        foreach (var blueprint in lowerBounds) {
            Output.WriteLine($"Blueprnt {blueprint.Key.Id}, geodes {blueprint.Value}");
        }

        return lowerBounds.Sum(b => b.Key.Id * b.Value);
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.
"
    ;

    private static Blueprint[] ParseBlueprints(string input) {
        return Shared.Split(input).Select(s => new Blueprint(s)).ToArray();
    }

    [Fact]
    public void ParsesExampleBlueprintsCorrectly() {
        Assert.Equal(ExampleInput, "\r\n" + string.Join("\r\n", ParseBlueprints(ExampleInput!).Select(v => v.ToString())) + "\r\n");
    }


    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day19(ExampleInput, Output).SolvePartOne();
        Assert.Equal(33, actual);
    }
}