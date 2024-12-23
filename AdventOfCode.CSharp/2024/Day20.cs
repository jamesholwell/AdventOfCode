using AdventOfCode.Core;
using AdventOfCode.Core.Algorithms;
using AdventOfCode.Core.Grid;
using AdventOfCode.Core.Points;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day20 : Solver {
    private readonly HashSet<(int x, int y)> spaces;

    private readonly Dictionary<(int x, int y), int> forward;
    private readonly Dictionary<(int x, int y), int> backward;
    
    private int minimumCheatSavingsForResult = 100;
    
    private readonly int fastestTime;

    public Day20(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) {
        if (input == null) {
            spaces = new HashSet<(int, int)>();
            forward = new Dictionary<(int, int), int>();
            backward = new Dictionary<(int, int), int>();
            return;
        }
        
        var grid = input.SplitGrid();
        var start = grid.Find('S');
        var end = grid.Find('E');
        var coordinates = grid.Where(c => c == '.').Union([start, end]).ToArray();
        
        spaces = [..coordinates];
        forward = Algorithm.Dijkstra(start, coordinates, p => p, ConnectionFunc);
        backward = Algorithm.Dijkstra(end, coordinates, p => p, ConnectionFunc);
        fastestTime = forward[end];
        
        return;

        IEnumerable<((int x, int y), int cost)> ConnectionFunc((int x, int y) node) {
            if (spaces.Contains(node.Up())) yield return (node.Up(), 1);
            if (spaces.Contains(node.Right())) yield return (node.Right(), 1);
            if (spaces.Contains(node.Down())) yield return (node.Down(), 1);
            if (spaces.Contains(node.Left())) yield return (node.Left(), 1);
        }
    }

    protected override long SolvePartOne() {
        Trace.WriteLine($"Fastest time without cheating: {fastestTime}");

        var possibleCheats = spaces.SelectMany(CheatOptions).ToArray();
        Trace.WriteLine($"Discovered {possibleCheats.Length} possible cheats");

        var cheatSavings = possibleCheats
            .Select(pc => (cheat: pc, savings: fastestTime - forward[pc.start] - 2 - backward[pc.end]))
            .Where(cs => cs.savings > 0)
            .ToArray();
        
        foreach (var group in cheatSavings.GroupBy(ch => ch.savings).OrderBy(g => g.Key))
            Trace.WriteLine(group.Count() == 1 
                ? $" - There is one cheat that saves {group.Key} picoseconds." 
                : $" - There are {group.Count()} cheats that save {group.Key} picoseconds.");

        return cheatSavings.Count(cs => cs.savings >= minimumCheatSavingsForResult);
        
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
         */
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

    protected override long SolvePartTwo() {
        var possibleCheats = spaces.SelectMany(CheatOptions).ToArray();
        Trace.WriteLine($"Discovered {possibleCheats.Length} possible cheats");

        var cheatSavings = possibleCheats
            .Select(pc => (cheat: pc, savings: fastestTime - backward[pc.end] - forward[pc.start] - pc.distance))
            .Where(cs => cs.savings > 0)
            .ToArray();
        
        foreach (var group in cheatSavings.Where(cs => cs.savings >= minimumCheatSavingsForResult).GroupBy(ch => ch.savings).OrderBy(g => g.Key))
            Trace.WriteLine(group.Count() == 1 
                ? $" - There is one cheat that saves {group.Key} picoseconds." 
                : $" - There are {group.Count()} cheats that save {group.Key} picoseconds.");

        return cheatSavings.Count(cs => cs.savings >= minimumCheatSavingsForResult);
        
        IEnumerable<((int x, int y) start, (int x, int y) end, int distance)> CheatOptions((int x, int y) start) {
            return spaces
                .Where(s => Math.Abs(start.x - s.x) + Math.Abs(start.y - s.y) <= 20)
                .Select(s => (end: s, distance: Math.Abs(start.x - s.x) + Math.Abs(start.y - s.y)))
                .Select(s => (start, s.end, s.distance))
                .ToArray();
        }
    }

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
        var solver = new Day20(ExampleInput, Output) {
            minimumCheatSavingsForResult = 50
        };

        var actual = solver.SolvePartOne();
        Assert.Equal(1, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var solver = new Day20(ExampleInput, Output) {
            minimumCheatSavingsForResult = 50
        };

        var actual = solver.SolvePartTwo();
        Assert.Equal(285, actual);
    }
}