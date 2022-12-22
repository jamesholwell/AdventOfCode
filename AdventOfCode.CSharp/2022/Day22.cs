using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day22 : Solver {
    /*
     * map is oriented [y][x] and is 0-indexed to create a 1-space buffer of "off map" spaces
     *
     * facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^)
     *
     * turn is right = +1, left = -1, no turn = 0 (last instruction)
     */
    public Day22(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private class Cube {
        public Cube(int numberOfRows, int numberOfColumns, int faceSize) {
            _faces = new Face[4, 4];
            Faces = new List<Face>();
            CurrentFace = new EmptyFace(0);
            NumberOfRows = numberOfRows;
            NumberOfColumns = numberOfColumns;
            FaceSize = faceSize;
        }
        
        public int NumberOfRows { get; }

        public int NumberOfColumns { get; }

        public int FaceSize { get; }

        public List<Face> Faces { get; }

        // ReSharper disable once InconsistentNaming - oh do be quiet
        private readonly Face[,] _faces;

        public Face CurrentFace;

        public Face this[int row, int column]
        {
            get => _faces[(row % 4 + 4) % 4, (column % 4 + 4) % 4];
            set {
                _faces[row, column] = value;
                Faces.Add(value);
                if (CurrentFace is EmptyFace) CurrentFace = value;
            }
        }
    }

    private abstract class Face {
        protected Face(string[] map) {
            Map = map;
        }

        public string[] Map { get; protected set; }

        public Face? Up { get; set; }

        public Face? Right { get; set; }

        public Face? Down { get; set; }

        public Face? Left { get; set; }
    }

    private sealed class MapFace : Face {
        public MapFace(string[] map) : base(map) { }
    }

    private sealed class EmptyFace : Face {
        public EmptyFace(int size) : base(Enumerable.Range(0, size).Select(_ => new string(' ', size)).ToArray()) { }
    }
    
    private (Cube cube, (int forward, int turn)[] instructions) Parse(string input) {
        var parts = input.SplitBy("\n\n");
        return (cube: ParseCube(parts[0]), ParseInstructions(parts[1]).ToArray());
    }

    private static Cube ParseCube(string s) {
        var lines = s.Split("\n");
        var height = lines.Length;
        var width = lines.Max(l => l.Length);

        var normalizedLines = lines.Select(l => l + new string(' ', width - l.Length)).ToArray();

        var size = height > width ? height / 4 : height / 3;
        var columns = width / size;
        var rows = height / size;

        var cube = new Cube(rows, columns, size);

        for (var row = 0; row < rows; ++row) {
            for (var column = 0; column < columns; ++column) {
                var slice = normalizedLines.Skip(row * size).Take(size).Select(l => l.Substring(column * size, size)).ToArray();

                if (string.IsNullOrWhiteSpace(slice[0]))
                    cube[row, column] = new EmptyFace(size);
                else {
                    cube[row, column] = new MapFace(slice);
                }
            }
        }
        
        for (var row = 0; row < rows; ++row) {
            for (var column = 0; column < columns; ++column) {
                var face = cube[row, column];
                if (face is EmptyFace) continue;
                
                var upFace = cube[row - 1, column];
                if (upFace is MapFace) {
                    face.Up = upFace;
                    upFace.Down = face;
                }

                var rightFace = cube[row, column + 1];
                if (rightFace is MapFace) {
                    face.Right = rightFace;
                    rightFace.Left = face;
                }

                var downFace = cube[row + 1, column];
                if (downFace is MapFace) {
                    face.Down = downFace;
                    downFace.Up = face;
                }

                var leftFace = cube[row, column - 1];
                if (leftFace is MapFace) {
                    face.Left = leftFace;
                    leftFace.Right = face;
                }
            }
        }
        
        return cube;
    }

    private static List<(int forward, int turn)> ParseInstructions(string s) {
        var instructions = new List<(int forward, int turn)>();

        var rightRuns = s.Split('R');
        for (var rightIndex = 0; rightIndex < rightRuns.Length; rightIndex++) {
            var isLastRightRun = rightIndex == rightRuns.Length - 1;
            var leftRuns = rightRuns[rightIndex].Split('L');
            for (var leftIndex = 0; leftIndex < leftRuns.Length; leftIndex++) {
                var isLastLeftRun = leftIndex == leftRuns.Length - 1;
                var facing = isLastLeftRun && isLastRightRun ? 0 : isLastLeftRun ? 1 : -1;
                instructions.Add((int.Parse(leftRuns[leftIndex]), facing));
            }
        }

        return instructions;
    }

    private string Render(Cube cube, IDictionary<Face, int?[,]>? positionHistory = null) {
        var buffer = new StringBuilder(cube.NumberOfRows * cube.FaceSize * (cube.NumberOfColumns * cube.FaceSize + 2));

        for (var row = 0; row < cube.NumberOfRows; ++row) {
            for (var y = 0; y < cube.FaceSize; ++y) {

                for (var column = 0; column < cube.NumberOfColumns; ++column) {
                    var face = cube[row, column];

                    for (var x = 0; x < cube.FaceSize; ++x) {
                        if (positionHistory?[face][column, row] != null) {
                            buffer.Append(Facings[positionHistory[face][column, row]!.Value]);
                            continue;
                        }

                        buffer.Append(face.Map[y][x]);
                    }
                }

                buffer.Append(Environment.NewLine);
            }
        }

        return buffer.ToString();
    }

    public override long SolvePartOne() {
        var (map, instructions) = Parse(Input);
        var position = (0, 0, 0);
        Trace.WriteLine(Render(map));

        var (finalPosition, positionHistory) = Walk(map, position, instructions);

        Output.WriteLine(Render(map, positionHistory));
        
        return 1000 * finalPosition.y + 4 * finalPosition.x + finalPosition.f;
    }

    private ((int x, int y, int f), IDictionary<Face, int?[,]>) Walk(Cube map, (int x, int y, int f) initialPosition, (int forward, int turn)[] instructions) {
        var positionHistory = map.Faces.ToDictionary(f => f, _ => new int?[map.NumberOfColumns, map.NumberOfRows]);
        var position = initialPosition;
        positionHistory[map.CurrentFace][position.x, position.y] = position.f;

        //foreach (var instruction in instructions) {
        //    for (var i = 0; i < instruction.forward; ++i) {
        //        var newX = position.x + (position.f == 0 ? 1 : position.f == 2 ? -1 : 0);
        //        var newY = position.y + (position.f == 1 ? 1 : position.f == 3 ? -1 : 0);
                
        //        if (map[newY][newX] == ' ') {
        //            switch (position.f) {
        //                case 0:
        //                    newX = 1;
        //                    while (map[newY][newX] == ' ') newX++;
        //                    break;

        //                case 1:
        //                    newY = 1;
        //                    while (map[newY][newX] == ' ') newY++;
        //                    break;

        //                case 2:
        //                    newX = width - 1;
        //                    while (map[newY][newX] == ' ') newX--;
        //                    break;

        //                case 3:
        //                    newY = height - 1;
        //                    while (map[newY][newX] == ' ') newY--;
        //                    break;
        //            }
        //        }

        //        if (map[newY][newX] == '#') {
        //            continue;
        //        }

        //        position = (newX, newY, position.f);
        //        positionHistory[position.x, position.y] = position.f;
        //    }

        //    position.f = ((position.f + instruction.turn) % 4 + 4) % 4;
        //    positionHistory[position.x, position.y] = position.f;
        //}

        return (position, positionHistory);
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string ExampleInput = @"
        ...#
        .#..
        #...
        ....
...#.......#
........#...
..#....#....
..........#.
        ...#....
        .....#..
        .#......
        ......#.

10R5L5R10L4R5L5
";

    private const string Facings = ">v<^";

    [Fact]
    public void ParsesMapCorrectly() {
        const string expected = @"        ...#    
        .#..    
        #...    
        ....    
...#.......#    
........#...    
..#....#....    
..........#.    
        ...#....
        .....#..
        .#......
        ......#.
";

        var (map, _) = Parse(ExampleInput);
        var actual = Render(map);
        Trace.WriteLine(actual);

        Assert.Equal(expected.ReplaceLineEndings(), actual);
    }
    
    [Fact]
    public void ParsesInstructionsCorrectly() {
        var (_, instructions) = Parse(ExampleInput);
        Assert.Equal(7, instructions.Length);
        Assert.Equal((10, 1), instructions[0]);
        Assert.Equal((5, -1), instructions[1]);
        Assert.Equal((5, 1), instructions[2]);
        Assert.Equal((10, -1), instructions[3]);
        Assert.Equal((4, 1), instructions[4]);
        Assert.Equal((5, -1), instructions[5]);
        Assert.Equal((5, 0), instructions[6]);
    }

    [Fact]
    public void WalksMapCorrectly() {
        var (map, instructions) = Parse(ExampleInput);
        var position = (0, 0, 0);

        var (finalPosition, positionHistory) = Walk(map, position, instructions);

        const string expected = @"        >>v#    
        .#v.    
        #.v.    
        ..v.    
...#...v..v#    
>>>v...>#.>>    
..#v...#....    
...>>>>v..#.    
        ...#....
        .....#..
        .#......
        ......#.
";
        var actual = Render(map, positionHistory);
        Output.WriteLine(actual);

        Assert.Equal(6, finalPosition.y);
        Assert.Equal(8, finalPosition.x);
        Assert.Equal(0, finalPosition.f);
        Assert.Equal(expected.ReplaceLineEndings(), actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day22(ExampleInput, Output).SolvePartOne();
        Assert.Equal(6032, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day22(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(5031, actual);
    }
}