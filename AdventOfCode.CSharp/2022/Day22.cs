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
            for (var row = 0; row < 4; ++row) {
                for (var column = 0; column < 4; ++column) {
                    _faces[row, column] = new EmptyFace(faceSize);
                }
            }

            Faces = new List<Face>();
            CurrentFace = _faces[0, 0];
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

        public int UpRotation { get; set; }

        public Face? Right { get; set; }

        public int RightRotation { get; set; }
        
        public Face? Down { get; set; }

        public int DownRotation { get; set; }
        
        public Face? Left { get; set; }

        public int LeftRotation { get; set; }

        public string this[int newY] => Map[newY];
    }

    private sealed class MapFace : Face {
        private readonly string identity;

        public MapFace(int row, int column, string[] map) : base(map) {
            identity = "ABCDEFGHIJKLMNOP".Substring(4 * row + column, 1);
            OffsetY = map[0].Length * row;
            OffsetX = map[0].Length * column;
        }

        public int OffsetY { get; }

        public int OffsetX { get; }

        public override string ToString() => identity + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, Map);

        public long Password((Face face, int x, int y, int f) finalPosition) {
            return 1000 * (OffsetY + finalPosition.y + 1) + 4 * (OffsetX + finalPosition.x + 1) + finalPosition.f;
        }
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
                    cube[row, column] = new MapFace(row, column, slice);
                }
            }
        }

        // make the direct connections
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
                        if (positionHistory?[face][x, y] != null) {
                            buffer.Append(Facings[positionHistory[face][x, y]!.Value]);
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
        var (cube, instructions) = Parse(Input);
        Link(cube);
        
        var position = (cube.CurrentFace, 0, 0, 0);
        Trace.WriteLine(Render(cube));

        var (finalPosition, positionHistory) = Walk(cube, position, instructions);

        Output.WriteLine(Render(cube, positionHistory));

        return ((MapFace) finalPosition.face).Password(finalPosition);
    }

    private static void Link(Cube cube) {
        for (var row = 0; row < cube.NumberOfRows; ++row) {
            for (var column = 0; column < cube.NumberOfColumns; ++column) {
                var face = cube[row, column];
                if (face is EmptyFace) continue;

                if (face.Up == null) {
                    var newRow = row;
                    while (cube[--newRow, column] is EmptyFace) { }

                    face.Up = cube[newRow, column];
                }

                if (face.Right == null) {
                    var newColumn = column;
                    while (cube[row, ++newColumn] is EmptyFace) { }

                    face.Right = cube[row, newColumn];
                }

                if (face.Down == null) {
                    var newRow = row;
                    while (cube[++newRow, column] is EmptyFace) { }

                    face.Down = cube[newRow, column];
                }

                if (face.Left == null) {
                    var newColumn = column;
                    while (cube[row, --newColumn] is EmptyFace) { }

                    face.Left = cube[row, newColumn];
                }
            }
        }
    }

    private ((Face face, int x, int y, int f), IDictionary<Face, int?[,]>) Walk(Cube cube, (Face face, int x, int y, int f) initialPosition, (int forward, int turn)[] instructions) {
        var positionHistory = cube.Faces.ToDictionary(f => f, _ => new int?[cube.FaceSize, cube.FaceSize]);
        var position = initialPosition;
        positionHistory[cube.CurrentFace][position.x, position.y] = position.f;

        foreach (var instruction in instructions) {
            for (var i = 0; i < instruction.forward; ++i) {
                var newFace = position.face;
                var newX = position.x + (position.f == 0 ? 1 : position.f == 2 ? -1 : 0);
                var newY = position.y + (position.f == 1 ? 1 : position.f == 3 ? -1 : 0);

                if (newX < 0) {
                    newFace = newFace.Left!;
                    newX = cube.FaceSize - 1;
                }

                if (newY < 0) {
                    newFace = newFace.Up!;
                    newY = cube.FaceSize - 1;
                }

                if (newX >= cube.FaceSize) {
                    newFace = newFace.Right!;
                    newX = 0;
                }

                if (newY >= cube.FaceSize) {
                    newFace = newFace.Down!;
                    newY = 0;
                }

                if (newFace[newY][newX] == '#') {
                    break;
                }

                position = (newFace, newX, newY, position.f);
                positionHistory[newFace][position.x, position.y] = position.f;
            }

            position.f = ((position.f + instruction.turn) % 4 + 4) % 4;
            positionHistory[position.face][position.x, position.y] = position.f;
        }

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
    public void WalksLinkedMapCorrectly() {
        var (cube, instructions) = Parse(ExampleInput);
        Link(cube);

        var position = (cube.CurrentFace, 0, 0, 0);
        var (finalPosition, positionHistory) = Walk(cube, position, instructions);

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
        var actual = Render(cube, positionHistory);
        Output.WriteLine(actual);

        Assert.Equal(6, ((MapFace)finalPosition.face).OffsetY + finalPosition.y + 1);
        Assert.Equal(8, ((MapFace)finalPosition.face).OffsetX + finalPosition.x + 1);
        Assert.Equal(0, finalPosition.f);
        Assert.Equal(expected.ReplaceLineEndings(), actual);
    }

    [Fact]
    public void WalksLinkedMapAndFinishesInCorrectPlace() {
        var (cube, instructions) = Parse(ExampleInput);
        Link(cube);

        var position = (cube.CurrentFace, 0, 0, 0);
        var (finalPosition, _) = Walk(cube, position, instructions);

        Assert.Equal(6, ((MapFace)finalPosition.face).OffsetY + finalPosition.y + 1);
        Assert.Equal(8, ((MapFace)finalPosition.face).OffsetX + finalPosition.x + 1);
        Assert.Equal(0, finalPosition.f);
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