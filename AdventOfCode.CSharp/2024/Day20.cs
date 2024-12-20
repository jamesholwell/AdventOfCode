using AdventOfCode.Core;
using AdventOfCode.Core.Algorithms;
using AdventOfCode.Core.Grid;
using AdventOfCode.Core.Points;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day20(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    protected override long SolvePartOne() {
        var grid = Input.SplitGrid();
        var start = grid.Find('S');
        var end = grid.Find('E');
        var coordinates = grid.Where(c => c == '.').Union([start, end]).ToArray();
        var spaces = new HashSet<(int,int)>(coordinates);
        
        var dijkstra = Algorithm.Dijkstra(start, coordinates, p => p, ConnectionFunc);
        
        var fastestTimeWithoutCheating = dijkstra[end];
        Trace.WriteLine($"Fastest time without cheating: {fastestTimeWithoutCheating}");
        
        /*
         * Cheats look like:
         *
         * 	    	2
         *      2	#	2
         *  2	#	0	#	2
         *      2	#	2
         *          2
         *
         *  Where 0 is the 'start position', '#' is the first pico-second through a wall
         *  and 2 is the second pico-second re-entering the track.
         *
         *  If you don't glitch through a wall, it's not a very good cheat
         *
         */
        var possibleCheats = coordinates.SelectMany(CheatOptions).ToArray();
        Trace.WriteLine($"Discovered {possibleCheats.Length} possible cheats");

        var cheatSavings = possibleCheats
            .Select(pc => (cheat: pc, savings: dijkstra[pc.end] - dijkstra[pc.start] - 2))
            .Where(cs => cs.savings > 0)
            .ToArray();
        
        foreach (var group in cheatSavings.GroupBy(ch => ch.savings).OrderBy(g => g.Key))
            Trace.WriteLine($" - There are {group.Count()} cheats that save {group.Key} picoseconds.");

        return cheatSavings.Count(cs => cs.savings >= 100);
        
        IEnumerable<((int x, int y), int cost)> ConnectionFunc((int x, int y) node) {
            if (spaces.Contains(node.Up())) yield return (node.Up(), 1);
            if (spaces.Contains(node.Right())) yield return (node.Right(), 1);
            if (spaces.Contains(node.Down())) yield return (node.Down(), 1);
            if (spaces.Contains(node.Left())) yield return (node.Left(), 1);
        }
        
        IEnumerable<((int x, int y) start, (int x, int y) end)> CheatOptions((int x, int y) node) {
            var upIsWall = !spaces.Contains(node.Up());
            var rightIsWall = !spaces.Contains(node.Right());
            var downIsWall = !spaces.Contains(node.Down());
            var leftIsWall = !spaces.Contains(node.Left());
            
            // clockwise from north
            if (upIsWall && spaces.Contains(node with {y = node.y - 2})) yield return (node, node with {y = node.y - 2});
            if ((upIsWall || rightIsWall) && spaces.Contains((node.x + 1, node.y - 1))) yield return (node, (node.x + 1, node.y - 1));
            if (rightIsWall && spaces.Contains(node with { x = node.x + 2})) yield return (node, node with {x = node.x + 2});
            if ((rightIsWall || downIsWall) && spaces.Contains((node.x + 1, node.y + 1))) yield return (node, (node.x + 1, node.y + 1));
            if (downIsWall && spaces.Contains(node with {y = node.y + 2})) yield return (node, node with { y = node.y + 2 });
            if ((downIsWall || leftIsWall) &&  spaces.Contains((node.x - 1, node.y + 1))) yield return (node, (node.x - 1, node.y + 1));
            if (leftIsWall && spaces.Contains(node with { x = node.x - 2})) yield return (node, node with {x = node.x - 2});
            if ((leftIsWall || upIsWall) && spaces.Contains((node.x - 1, node.y - 1))) yield return (node, (node.x - 1, node.y - 1));
        }
    }

    protected override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = 
        """
        ###############
        #...#...#.....#
        #.#.#.#.#.###.#
        #S#...#.#.#...#
        #######.#.#.###
        #######.#.#...#
        #######.#.###.#
        ###..E#...#...#
        ###.#######.###
        #...###...#...#
        #.#####.#.###.#
        #.#...#.#.#...#
        #.#.#.#.#.#.###
        #...#...#...###
        ###############
        """;

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day20(ExampleInput, Output).SolvePartOne();
        Assert.Equal(0, actual);
    }
}