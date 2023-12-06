using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day6 : Solver {
    public Day6(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var lines = Shared.Split(Input);
        var times = lines[0].Substring(10).Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var distances = lines[1].Substring(10).Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        IEnumerable<(int time, int distanceRecord)> pairs = times.Zip(distances, (t, d) => (time: int.Parse(t), distance: int.Parse(d)));

        var accumulator = new List<int>();
        
        foreach (var pair in pairs) {
            Output.WriteLine($"Time: {pair.time}, Distance record: {pair.distanceRecord}, ");

            var waysToWin = 0;
            for (var i = 0; i < pair.time; ++i) {
                var distanceTravelled = i * (pair.time - i);
                if (distanceTravelled > pair.distanceRecord)
                    waysToWin++;
            }
            
            accumulator.Add(waysToWin);
        }
        
        return accumulator.Aggregate(1, (acc, item) => acc * item);
    }

    public override long SolvePartTwo() {
        var lines = Shared.Split(Input);
        var time = int.Parse(lines[0].Substring(10).Replace(" ", string.Empty));
        var distanceRecord = long.Parse(lines[1].Substring(10).Replace(" ", string.Empty));

        Output.WriteLine($"Time: {time}, Distance record: {distanceRecord}");

        var waysToWin = 0;
        for (var i = 0L; i < time; ++i) {
            var distanceTravelled = i * (time - i);
            if (distanceTravelled > distanceRecord)
                waysToWin++;
        }
        
        return waysToWin;
    }
    
    private const string? ExampleInput = @"
Time:      7  15   30
Distance:  9  40  200
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day6(ExampleInput, Output).SolvePartOne();
        Assert.Equal(288, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day6(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(71503, actual);
    }
}