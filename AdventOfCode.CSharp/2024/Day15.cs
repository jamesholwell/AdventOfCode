using AdventOfCode.Core;
using AdventOfCode.Core.Grid;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day15(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    private static (char[,] grid, Directions[] moves) Parse(string input) {
        var parts = input.SplitBy("\n\n");
        
        var grid = parts[0].SplitGrid();
        
        var moves = parts[1].Replace("\n", string.Empty).Select(s => s switch {
            '^' => Directions.North,
            '>' => Directions.East,
            'v' => Directions.South,
            '<' => Directions.West,
            _ => throw new FormatException($"Invalid direction: {s}")
        }).ToArray();
        
        return (grid, moves);
    }

    private static (int x, int y) AttemptMove((int x, int y) position, Directions direction, char[,] grid) {
        (int x, int y) vector = direction switch {
            Directions.North => (0, -1),
            Directions.East => (1, 0),
            Directions.South => (0, 1),
            Directions.West => (-1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        var newPosition = (x: position.x + vector.x, y: position.y + vector.y);
        
        // facing a wall, can't move
        if (grid.At(newPosition) == '#')
            return position;
        
        // move the shunt pointer to the end of the stack of boxes
        var shunt = newPosition;
        while (grid.At(shunt) != '.') {
            shunt = (x: shunt.x + vector.x, y: shunt.y + vector.y);
            
            // can't shunt into a wall
            if (grid.At(shunt) == '#')
                return position;
        }

        // now backtrack
        while (shunt != position) {
            var nextShunt = (x: shunt.x - vector.x, y: shunt.y - vector.y);
            grid[shunt.y, shunt.x] = grid.At(nextShunt);
            shunt = nextShunt;
        }

        grid[position.y, position.x] = '.';
        return newPosition;
    }
    
    private static int SumGps(char[,] grid) => grid.Flatten((x, y, c) => c != 'O' ? 0 : 100 * y + x).Sum();

    protected override long SolvePartOne() {
        var (grid, moves) = Parse(Input);
        var robotPosition = grid.Find('@');
        
        foreach (var move in moves) 
            robotPosition = AttemptMove(robotPosition, move, grid);
            
        Trace.WriteLine(grid.Render());
        return SumGps(grid);
    }

    protected override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private readonly char[] directionChars = ['^', '>', 'v', '<'];
    
    private enum Directions {
        North,
        East,
        South,
        West
    }

    private const string SimpleExampleInput =
        """
        ########
        #..O.O.#
        ##@.O..#
        #...O..#
        #.#.O..#
        #...O..#
        #......#
        ########

        <^^>>>vv<v>>v<<
        """;

    private const string? ExampleInput = 
        """
        ##########
        #..O..O.O#
        #......O.#
        #.OO..O.O#
        #..O@..O.#
        #O#..O...#
        #O..O..O.#
        #.OO.O.OO#
        #....O...#
        ##########

        <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
        vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
        ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
        <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
        ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
        ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
        >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
        <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
        ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
        v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
        """;

    [Fact]
    public void CalculatesSumGpsAsExpected() {
        const string gpsExample =
            """
            #######
            #...O..
            #......
            """;

        var grid = gpsExample.SplitGrid();
        Assert.Equal(104, SumGps(grid));
    }
    
    [Fact]
    public void RobotMovesAsExpectedForSimpleExample() {
        var (grid, moves) = Parse(SimpleExampleInput);
        var robotPosition = grid.Find('@');
        
        Output.WriteLine("Initial state:");
        Output.WriteLine(grid.Render());

        foreach (var move in moves) {
            robotPosition = AttemptMove(robotPosition, move, grid);
            
            Output.WriteLine(string.Empty);
            Output.WriteLine($"Move {directionChars[(int)move]}:");
            Output.WriteLine(grid.Render());
        }
        
        const string expected =
            """
            ########
            #....OO#
            ##.....#
            #.....O#
            #.#O@..#
            #...O..#
            #...O..#
            ########
            """;

        Assert.Equal(expected, grid.Render());
        Assert.Equal(2028, SumGps(grid));
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day15(ExampleInput, Output).SolvePartOne();
        Assert.Equal(10092, actual);
    }
}