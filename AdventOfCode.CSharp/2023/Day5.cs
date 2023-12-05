﻿using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day5 : Solver {
    public Day5(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var sections = Input.SplitBy("\n\n");
        
        var seeds = sections[0].Split(':', 2)[1]
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

        var lookup = new Dictionary<string, List<(long sourceOffset, long destinationOffset, long length)>>();

        foreach (var section in sections.Skip(1)) {
            var parts = section.Split(" map:");

            var mapParts = parts[0].Split("-to-");
            var key = mapParts[0];
            // var source = mapParts[0];
            // var destination = mapParts[1];
            if (!lookup.ContainsKey(key))
                lookup[key] = new List<(long, long, long)>(); 
            
            foreach (var range in Shared.Split(parts[1])) {
                var rangeParts = range.Split(' ', StringSplitOptions.TrimEntries).Select(long.Parse).ToArray();
                var sourceOffset = rangeParts[1];
                var destinationOffset = rangeParts[0];
                var length = rangeParts[2];

                lookup[key].Add((sourceOffset, destinationOffset, length));
            }
        }

        var locations = new List<long>();
        
        foreach (var seed in seeds) {
            var category = "seed";
            var index = seed;

            while (category != "location") {
                Output.WriteLine($"{category} {index} -> ");
                var match = lookup[category].FirstOrDefault(l =>
                    l.sourceOffset <= index && index < l.sourceOffset + l.length);

                if (match != default((long, long, long))) {
                    Output.WriteLine($"Matched {match.sourceOffset} {match.destinationOffset} {match.length}");
                    index = index - match.sourceOffset + match.destinationOffset;
                }

                switch (category) {
                    case "seed":
                        category = "soil";
                        break;
                    case "soil":
                        category = "fertilizer";
                        break;
                    case "fertilizer":
                        category = "water";
                        break;
                    case "water":
                        category = "light";
                        break;
                    case "light":
                        category = "temperature";
                        break;
                    case "temperature":
                        category = "humidity";
                        break;
                    case "humidity":
                        category = "location";
                        break;
                }
            }
            
            Output.WriteLine($"{category} {index}");
            Output.WriteLine();
            locations.Add(index);
            Output.WriteLine($"{category} = {index}");
        }
        
        return locations.Min();
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day5(ExampleInput, Output).SolvePartOne();
        Assert.Equal(35, actual);
    }
}