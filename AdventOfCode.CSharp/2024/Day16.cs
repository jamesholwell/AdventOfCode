using AdventOfCode.Core;
using AdventOfCode.Core.Algorithms;
using AdventOfCode.Core.Grid;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day16 : Solver {
    private readonly char[,] grid;
    
    private readonly (int x, int y) endTile;

    private readonly (int x, int y, Directions heading) initialState;

    public Day16(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) {
        // test-runner initializes without values: suppress exceptions
        if (input == null) {
            grid = new char[0,0];
            endTile = (0, 0);
            initialState = (0, 0, Directions.East);
            return;
        }
            
        grid = input.SplitGrid();
        var startTile = grid.Find('S');
        initialState = (startTile.x, startTile.y, heading: Directions.East);
        endTile = grid.Find('E');
    }

    protected override long SolvePartOne() {
        var (minCost, path) = Algorithm.AStar(initialState, Connections, Heuristic, IsGoal);

        foreach (var step in path)
            grid[step.y, step.x] = step.heading switch {
                Directions.North => '^',
                Directions.East => '>',
                Directions.South => 'v',
                Directions.West => '<',
                _ => throw new ArgumentOutOfRangeException()
            };
        
        Trace.WriteLine(grid.Render());
        
        return minCost;
    }

    protected override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private IEnumerable<((int x, int y, Directions heading), int)> Connections(
        (int x, int y, Directions heading) state) {

        var forward = state.heading switch {
            Directions.North => state with { y = state.y - 1},
            Directions.East => state with { x = state.x + 1 },
            Directions.South => state with { y = state.y + 1 },
            Directions.West => state with { x = state.x - 1 },
            _ => throw new ArgumentOutOfRangeException()
        };

        if (grid.At(forward.x, forward.y) != '#')
            yield return (forward, 1);
            
        yield return (state with { heading = Clockwise(state.heading)}, 1000);
        yield return (state with { heading = CounterClockwise(state.heading)}, 1000);
    }

    private static Directions Clockwise(Directions direction) =>
        direction switch {
            Directions.North => Directions.East,
            Directions.East => Directions.South,
            Directions.South => Directions.West,
            Directions.West => Directions.North,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    private static Directions CounterClockwise(Directions direction) =>
        direction switch {
            Directions.North => Directions.West,
            Directions.West => Directions.South,
            Directions.South => Directions.East,
            Directions.East => Directions.North,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    private int Heuristic((int x, int y, Directions heading) state) => 
        Math.Abs(endTile.x - state.x) + Math.Abs(endTile.y - state.y);

    private bool IsGoal((int x, int y, Directions heading) state) =>
        state.x == endTile.x && state.y == endTile.y;

    private enum Directions {
        North,
        East,
        South,
        West
    }

    private const string? ExampleInput1 = @"
###############
#.......#....E#
#.#.###.#.###.#
#.....#.#...#.#
#.###.#####.#.#
#.#.#.......#.#
#.#.#####.###.#
#...........#.#
###.#.#####.#.#
#...#.....#.#.#
#.#.#.###.#.#.#
#.....#...#.#.#
#.###.#.#.#.#.#
#S..#.....#...#
###############
";

    private const string? ExampleInput2 = @"
#################
#...#...#...#..E#
#.#.#.#.#.#.#.#.#
#.#.#.#...#...#.#
#.#.#.#.###.#.#.#
#...#.#.#.....#.#
#.#.#.#.#.#####.#
#.#...#.#.#.....#
#.#.#####.#.###.#
#.#.#.......#...#
#.#.###.#####.###
#.#.#...#.....#.#
#.#.#.#####.###.#
#.#.#.........#.#
#.#.#.#########.#
#S#.............#
#################
";

    [Theory]
    [InlineData(ExampleInput1, 7036)]
    [InlineData(ExampleInput2, 11048)]
    public void SolvesPartOneExample(string input, int expected) {
        var actual = new Day16(input, Output).SolvePartOne();
        Assert.Equal(expected, actual);
    }
}