using AdventOfCode.Core;
using AdventOfCode.Core.Grid;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day25(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    private (List<int[]> keys, List<int[]> locks) Parse(string input) {
        var keys = new List<int[]>();
        var locks = new List<int[]>();

        foreach (var block in input.SplitBy("\n\n")) {
            var rows = block.Split('\n', 7);
            
            var buffer = new int[5];
            for (var pin = 0; pin < buffer.Length; pin++) 
                for (var i = 0; i < 7; i++)
                    if (rows[i][pin] == '#')
                        ++buffer[pin];
                
            if (rows[0] == "#####")
                keys.Add(buffer);
            else
                locks.Add(buffer);
        }
        
        return (keys, locks);
    }

    protected override long SolvePartOne() {
        var (keys, locks) = Parse(Input);

        return keys.Sum(k => locks.Count(l => k.Zip(l).All(p => p.First + p.Second < 8)));
    } 

    protected override long SolvePartTwo() => throw new InvalidOperationException("Merry Christmas!");

    private const string? ExampleInput = 
        """
        #####
        .####
        .####
        .####
        .#.#.
        .#...
        .....
        
        #####
        ##.##
        .#.##
        ...##
        ...#.
        ...#.
        .....
        
        .....
        #....
        #....
        #...#
        #.#.#
        #.###
        #####
        
        .....
        .....
        #.#..
        ###..
        ###.#
        ###.#
        #####
        
        .....
        .....
        .....
        #....
        #.#..
        #.#.#
        #####
        """;

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day25(ExampleInput, Output).SolvePartOne();
        Assert.Equal(3, actual);
    }
}