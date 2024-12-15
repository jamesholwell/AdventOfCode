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

    private static string Expand(string? input) => input == null ? string.Empty : 
        input.Replace("#", "##")
            .Replace("O", "[]")
            .Replace(".", "..")
            .Replace("@", "@.");
    
    private static (int x, int y) AttemptMove((int x, int y) position, Directions direction, char[,] grid, HashSet<(int x, int y)> updates) {
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
            updates.Add((shunt.x, shunt.y));
            
            shunt = nextShunt;
        }

        grid[position.y, position.x] = '.';
        updates.Add(position);
        
        return newPosition;
    }
    
    private static (int x, int y) AttemptDoubleWidthMove((int x, int y) position, Directions direction, char[,] grid, HashSet<(int x, int y)> updates) {
        // left and right moves behave like part 1
        if (direction is Directions.East or Directions.West)
            return AttemptMove(position, direction, grid, updates);
        
        var dy = direction switch {
            Directions.North => -1,
            Directions.South => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        var shunts = new List<(int x, int y)>();
        var shuntFront = new HashSet<(int x, int y)> { position };

        while (true) {
            // move the shunt front forwards
            var nextPositions = shuntFront.Select(p => p with { y = p.y + dy }).ToArray();
            
            // look for obstructions
            if (nextPositions.Any(np => grid.At(np) == '#')) 
                return position;

            // path is not obstructed, advance
            shunts.AddRange(shuntFront);
            shuntFront.Clear();
            
            // if path is completely clear, we are done
            if (nextPositions.All(np => grid.At(np) == '.'))
                break;
            
            // otherwise we need to build the next shunt front
            foreach (var nextPosition in nextPositions) {
                if (grid.At(nextPosition) == '.')
                    continue;
                
                shuntFront.Add(nextPosition);
                
                // check for half-boxes
                if (grid.At(nextPosition) == ']')
                    shuntFront.Add(nextPosition with { x = nextPosition.x - 1 });
                else if (grid.At(nextPosition) == '[')
                    shuntFront.Add(nextPosition with { x = nextPosition.x + 1 });
            }
        }

        // now backtrack
        foreach (var shunt in shunts.OrderByDescending(s => s.y * dy)) {
            grid[shunt.y + dy, shunt.x] = grid.At(shunt);
            updates.Add((shunt.x, shunt.y + dy));
            
            grid[shunt.y, shunt.x] = '.';
            updates.Add((shunt.x, shunt.y));
        }
        
        return position with { y = position.y + dy };
    }

    private static int SumGps(char[,] grid) => grid.Flatten((x, y, c) => c != 'O' && c != '[' ? 0 : 100 * y + x).Sum();

    protected override long SolvePartOne() {
        var (grid, moves) = Parse(Input);
        var robotPosition = grid.Find('@');
        var updates = new HashSet<(int x, int y)>();
        
        foreach (var move in moves) 
            robotPosition = AttemptMove(robotPosition, move, grid, updates);
            
        Trace.WriteLine(grid.Render());
        return SumGps(grid);
    }

    protected override long SolvePartTwo() {
        var (grid, moves) = Parse(Expand(Input));
        var robotPosition = grid.Find('@');
        var updates = new HashSet<(int x, int y)>();
        var i = 0;
        
        try {
            Console.CursorVisible = false;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Move X:");
            Console.WriteLine(grid.Render());

            foreach (var move in moves) {
                updates.Clear();
                robotPosition = AttemptDoubleWidthMove(robotPosition, move, grid, updates);

                if (i++ % 10 == 0) {
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine(grid.Render());
                }
                else {
                    foreach (var coordinate in updates) {
                        Console.SetCursorPosition(coordinate.x, 1 + coordinate.y);
                        Console.Write(grid.At(coordinate));
                    }
                }

                if (updates.Count > 2)
                    Thread.Sleep(1);
            }
        }
        finally {
            Console.SetCursorPosition(0, grid.Height());
            Console.WriteLine();
            Console.WriteLine();
            Console.CursorVisible = true;
        }

        return SumGps(grid);
    }
    
    private readonly char[] directionChars = ['^', '>', 'v', '<'];
    
    private enum Directions {
        North,
        East,
        South,
        West
    }
    
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
    public void CalculatesDoubleWidthSumGpsAsExpected1() {
        const string gpsExample =
            """
            ##########
            ##...[]...
            ##........
            """;

        var grid = gpsExample.SplitGrid();
        Assert.Equal(105, SumGps(grid));
    }
    
    [Fact]
    public void CalculatesDoubleWidthSumGpsAsExpected2() {
        const string gpsExample =
            """
            ####################
            ##[].......[].[][]##
            ##[]...........[].##
            ##[]........[][][]##
            ##[]......[]....[]##
            ##..##......[]....##
            ##..[]............##
            ##..@......[].[][]##
            ##......[][]..[]..##
            ####################
            """;

        var grid = gpsExample.SplitGrid();
        Assert.Equal(9021, SumGps(grid));
    }
    
    [Fact]
    public void RobotMovesAsExpectedForSimpleExample() {
        const string simpleExampleInput =
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
        
        var (grid, moves) = Parse(simpleExampleInput);
        var robotPosition = grid.Find('@');
        
        Output.WriteLine("Initial state:");
        Output.WriteLine(grid.Render());
        var updates = new HashSet<(int x, int y)>();

        foreach (var move in moves) {
            robotPosition = AttemptMove(robotPosition, move, grid, updates);
            
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
    
    [Fact]
    public void ExpandsGridAsExpected() {
        var expandedInput = Expand(ExampleInput);
        var (grid, _) = Parse(expandedInput);
        
        const string expected =
            """
            ####################
            ##....[]....[]..[]##
            ##............[]..##
            ##..[][]....[]..[]##
            ##....[]@.....[]..##
            ##[]##....[]......##
            ##[]....[]....[]..##
            ##..[][]..[]..[][]##
            ##........[]......##
            ####################
            """;

        Assert.Equal(expected, grid.Render());
    }
    
    [Fact]
    public void RobotMovesAsExpectedForPartTwoExample() {
        const string simpleExampleInput =
            """
            #######
            #...#.#
            #.....#
            #..OO@#
            #..O..#
            #.....#
            #######

            <vv<<^^<<^^
            """;
        
        var (grid, moves) = Parse(Expand(simpleExampleInput));
        var robotPosition = grid.Find('@');
        
        Output.WriteLine("Initial state:");
        Output.WriteLine(grid.Render());
        var updates = new HashSet<(int x, int y)>();

        foreach (var move in moves) {
            robotPosition = AttemptDoubleWidthMove(robotPosition, move, grid, updates);
            
            Output.WriteLine(string.Empty);
            Output.WriteLine($"Move {directionChars[(int)move]}:");
            Output.WriteLine(grid.Render());
        }
        
        const string expected =
            """
            ##############
            ##...[].##..##
            ##...@.[]...##
            ##....[]....##
            ##..........##
            ##..........##
            ##############
            """;

        Assert.Equal(expected, grid.Render());
    }

    [Fact]
    public void RobotMovesTrickyConfigurationAsExpected() {
        const string trickyInput =
            """
            ####################
            ##[]..[]......[][]##
            ##[]...........[].##
            ##...........@[][]##
            ##..........[].[].##
            ##..##[]..[].[]...##
            ##...[]...[]..[]..##
            ##.....[]..[].[][]##
            ##........[]......##
            ####################
            """;
        
        var grid = trickyInput.SplitGrid();
        var robotPosition = grid.Find('@');
        var updates = new HashSet<(int x, int y)>();
        
        robotPosition = AttemptDoubleWidthMove(robotPosition, Directions.South, grid, updates);
        
        Output.WriteLine(grid.Render());
        
        const string expected =
            """
            ####################
            ##[]..[]......[][]##
            ##[]...........[].##
            ##............[][]##
            ##...........@.[].##
            ##..##[]..[][]....##
            ##...[]...[].[]...##
            ##.....[]..[].[][]##
            ##........[]..[]..##
            ####################
            """;
        Assert.Equal(expected, grid.Render());
        
        Assert.Equal(4, robotPosition.y);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day15(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(9021, actual);
    }
}