using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day19 : Solver {
    public Day19(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private readonly struct Blueprint {
        public readonly int Id;

        public readonly int OreRobotOreCost;

        public readonly int ClayRobotOreCost;

        public readonly int ObsidianRobotOreCost;

        public readonly int ObsidianRobotClayCost;

        public readonly int GeodeRobotOreCost;

        public readonly int GeodeRobotObsidianCost;

        public readonly int OreRobotLimit;

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

            OreRobotLimit = Math.Max(Math.Max(Math.Max(OreRobotOreCost, ClayRobotOreCost), ObsidianRobotOreCost), GeodeRobotOreCost);
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

        public readonly int OreRobots;

        public readonly int ClayRobots;

        public readonly int ObsidianRobots;

        public readonly int TimeLeft;

        public readonly int LifetimeGeodesCracked;

        public Scenario(Blueprint blueprint, int ores, int clays, int obsidians, int oreRobots, int clayRobots, int obsidianRobots, int timeLeft, int lifetimeGeodesCracked) {
            Blueprint = blueprint;
            Ores = ores;
            Clays = clays;
            Obsidians = obsidians;
            OreRobots = oreRobots;
            ClayRobots = clayRobots;
            ObsidianRobots = obsidianRobots;
            TimeLeft = timeLeft;
            LifetimeGeodesCracked = lifetimeGeodesCracked;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario BuildGeodeRobot(int timeTaken) {
            return new Scenario(Blueprint, 
                Ores + timeTaken * OreRobots - Blueprint.GeodeRobotOreCost, 
                Clays + timeTaken * ClayRobots,
                Obsidians + timeTaken * ObsidianRobots - Blueprint.GeodeRobotObsidianCost,
                OreRobots, 
                ClayRobots, 
                ObsidianRobots,
                TimeLeft - timeTaken, 
                LifetimeGeodesCracked + (TimeLeft - timeTaken));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario BuildObsidianRobot(int timeTaken) {
            return new Scenario(Blueprint,
                Ores + timeTaken * OreRobots - Blueprint.ObsidianRobotOreCost,
                Clays + timeTaken * ClayRobots - Blueprint.ObsidianRobotClayCost,
                Obsidians + timeTaken * ObsidianRobots,
                OreRobots,
                ClayRobots,
                ObsidianRobots + 1,
                TimeLeft - timeTaken,
                LifetimeGeodesCracked);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario BuildClayRobot(int timeTaken) {
            return new Scenario(Blueprint,
                Ores + timeTaken * OreRobots - Blueprint.ClayRobotOreCost,
                Clays + timeTaken * ClayRobots,
                Obsidians + timeTaken * ObsidianRobots,
                OreRobots,
                ClayRobots + 1,
                ObsidianRobots,
                TimeLeft - timeTaken,
                LifetimeGeodesCracked);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scenario BuildOreRobot(int timeTaken) {
            return new Scenario(Blueprint,
                Ores + timeTaken * OreRobots - Blueprint.OreRobotOreCost,
                Clays + timeTaken * ClayRobots,
                Obsidians + timeTaken * ObsidianRobots,
                OreRobots + 1,
                ClayRobots,
                ObsidianRobots,
                TimeLeft - timeTaken,
                LifetimeGeodesCracked);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Ores, Clays, Obsidians, OreRobots, ClayRobots, ObsidianRobots, TimeLeft, LifetimeGeodesCracked);
        }
    }

    public override long SolvePartOne() {
        var blueprints = ParseBlueprints(Input);
        var maxima = Solve(blueprints, 24);
        return maxima.Sum(b => b.Key.Id * b.Value);
    }

    public override long SolvePartTwo() {
        var blueprints = ParseBlueprints(Input).Take(3).ToArray();
        var maxima = Solve(blueprints, 32);
        return maxima.Aggregate(1, (acc, charge) => acc * charge.Value);
    }
    
    private IDictionary<Blueprint, int> Solve(Blueprint[] blueprints, int timeLeft) {
        var currentScenarios = blueprints.Select(b => new Scenario(b, 0, 0, 0, 1, 0, 0, timeLeft, 0)).ToArray();
        var i = 0;
        var ops = 0;
        var timer = new Stopwatch();
        var lowerBounds = blueprints.ToDictionary(b => b, _ => 0);

        while (currentScenarios.Any()) {
            timer.Reset();
            timer.Start();
            var passOperations = currentScenarios.Length;
            Output.WriteLine($"Pass {++i} with {passOperations:N0} scenarios to evaluate");

            var nextScenarios = new ConcurrentBag<Scenario>();

            currentScenarios.AsParallel().ForAll(scenario =>
            {
                Interlocked.Increment(ref ops);
                var blueprint = scenario.Blueprint;

                // no time left to build and produce any more geodes
                if (scenario.TimeLeft <= 1) return;

                // if we have obsidian production, we could try to build another geode cracking robot
                if (scenario.ObsidianRobots > 0) {
                    var timeUntilGeodeRobotReady = 1 + Math.Max(
                        Math.Max(0, (blueprint.GeodeRobotOreCost - scenario.Ores + scenario.OreRobots - 1) / scenario.OreRobots),
                        Math.Max(0, (blueprint.GeodeRobotObsidianCost - scenario.Obsidians + scenario.ObsidianRobots - 1) / scenario.ObsidianRobots)
                    );

                    if (timeUntilGeodeRobotReady < scenario.TimeLeft) {
                        nextScenarios.Add(scenario.BuildGeodeRobot(timeUntilGeodeRobotReady));

                        /*

                         Always build a geode robot if you can immediately
                         
                         Imagine that not building Robot X instead of a Geode Robot was advantageous,
                         that advantage could only arise from being able to build another Geode Robot faster,
                         which would only occur if the additional resource Robot X mined were essential
                         
                         t = t0  -> Build X
                                 -> Build G  Score (t0 - 2)  X++
                                 -> Build G  Score (t0 - 3)
                         
                         Now consider the alternative (build now)

                         t = t0  -> Build G  Score (t0 - 1)
                                 -> Build X  
                                 -> (tick)   X++
                                 -> Build G  Score (t0 - 4)

                        Since (t0 - 2) + (t0 - 3) == (2*t0 - 5) == (t0 - 1) + (t0 - 4) there is no advantage

                         */

                        if (timeUntilGeodeRobotReady == 1) return;
                    }
                }

                // if we have clay production, we could try to build another obsidian robot (unless we have enough)
                if (scenario.ClayRobots > 0 && scenario.ObsidianRobots < blueprint.GeodeRobotObsidianCost) {
                    var timeUntilObsidianRobotReady = 1 + Math.Max(
                        Math.Max(0, (blueprint.ObsidianRobotOreCost - scenario.Ores + scenario.OreRobots - 1) / scenario.OreRobots),
                        Math.Max(0, (blueprint.ObsidianRobotClayCost - scenario.Clays + scenario.ClayRobots - 1) / scenario.ClayRobots)
                    );

                    if (timeUntilObsidianRobotReady < scenario.TimeLeft) {
                        nextScenarios.Add(scenario.BuildObsidianRobot(timeUntilObsidianRobotReady));
                    }
                }

                // we can always try to build a clay robot (but we might not need any more)
                if (scenario.ClayRobots < blueprint.ObsidianRobotClayCost) {
                    var timeUntilClayRobotReady = 1 + Math.Max(0, (blueprint.ClayRobotOreCost - scenario.Ores + scenario.OreRobots - 1) / scenario.OreRobots);
                    if (timeUntilClayRobotReady < scenario.TimeLeft) {
                        nextScenarios.Add(scenario.BuildClayRobot(timeUntilClayRobotReady));
                    }
                }
                // we can always try to build an ore robot (but we might not need any more)
                if (scenario.OreRobots < blueprint.OreRobotLimit) {
                    var timeUntilOreRobotReady = 1 + Math.Max(0, (blueprint.OreRobotOreCost - scenario.Ores + scenario.OreRobots - 1) / scenario.OreRobots);
                    if (timeUntilOreRobotReady < scenario.TimeLeft) {
                        nextScenarios.Add(scenario.BuildOreRobot(timeUntilOreRobotReady));
                    }
                }
            });
            
            Output.WriteLine($"... done exploring, building next pass");
            
            // set up the next pass
            currentScenarios = nextScenarios.Distinct().ToArray();

            // write out lower bounds
            foreach (var scenariosForBlueprint in currentScenarios.GroupBy(b => b.Blueprint)) {
                var max = scenariosForBlueprint.Max(s => s.LifetimeGeodesCracked);

                if (max > lowerBounds[scenariosForBlueprint.Key])
                    lowerBounds[scenariosForBlueprint.Key] = max;
            }

            timer.Stop();
            Output.WriteLine(timer.ElapsedMilliseconds < 1
                ? $"... finished pass {i} after {timer.ElapsedTicks} ticks."
                : $"... finished pass {i} after {1e-3 * timer.ElapsedMilliseconds:N1}s. {passOperations / (1.0 * timer.ElapsedMilliseconds):N1} kops/sec");
        }

        Output.WriteLine();
        Output.WriteLine($"Total of {ops:N0} scenarios inspected");
        Output.WriteLine();

        foreach (var blueprint in lowerBounds) {
            Output.WriteLine($"Blueprint {blueprint.Key.Id} produces at most {blueprint.Value} geodes");
        }

        return lowerBounds;
    }
    
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

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day19(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(56 * 62, actual);
    }
}