using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day2 : Solver {
    public Day2(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var availableRed = 12;
        var availableGreen = 13;
        var availableBlue = 14;
        var accumulator = 0;

        foreach (var line in Shared.Split(Input)) {
            var game = ParseGame(line);
            var fail = false;

            foreach (var set in game.Sets) {
                if (set.Reds > availableRed) fail = true;
                if (set.Greens > availableGreen) fail = true;
                if (set.Blues > availableBlue) fail = true;
            }

            if (!fail) accumulator += game.Id;
        }

        return accumulator;
    }
    
    public override long SolvePartTwo() {
        var accumulator = 0;

        foreach (var line in Shared.Split(Input)) {
            var maxRed = 0;
            var maxGreen = 0;
            var maxBlue = 0;
            var game = ParseGame(line);

            foreach (var set in game.Sets) {
                if (set.Reds > maxRed) maxRed = set.Reds;
                if (set.Greens > maxGreen) maxGreen = set.Greens;
                if (set.Blues > maxBlue) maxBlue = set.Blues;
            }

            accumulator += maxRed * maxGreen * maxBlue;
        }

        return accumulator;
    }

    class Game {
        public int Id;

        public readonly List<GameSet> Sets = new();
    }

    class GameSet {
        public int Reds;
        public int Blues;
        public int Greens;
    }

    Game ParseGame(string line) {
        var outerparts = line.Split(':');
        var gameparts = outerparts[0].Split(' ');
        var game = new Game { Id = int.Parse(gameparts[1]) };

        foreach (var outerset in outerparts[1].Split(';', StringSplitOptions.TrimEntries)) {
            var set = new GameSet();
            foreach (var pair in outerset.Split(',', StringSplitOptions.TrimEntries)) {
                var pairparts = pair.Split(' ');
                var number = int.Parse(pairparts[0]);
                switch (pairparts[1]) {
                    case "red":
                        set.Reds = number;
                        break;
                    case "green":
                        set.Greens = number;
                        break;
                    case "blue":
                        set.Blues = number;
                        break;
                    default:
                        throw new InvalidOperationException($"Failed to parse set '{outerset}'");
                }
            }

            game.Sets.Add(set);
        }

        return game;
    }

    private const string? ExampleInput = @"
Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day2(ExampleInput, Output).SolvePartOne();
        Assert.Equal(8, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day2(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(2286, actual);
    }
}