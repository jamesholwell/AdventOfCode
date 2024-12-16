using System.Text;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day22(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    /*
     * map is oriented [y][x] and is 0-indexed to create a 1-space buffer of "off map" spaces
     *
     * facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^)
     *
     * turn is right = +1, left = -1, no turn = 0 (last instruction)
     */

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
            set
            {
                _faces[row, column] = value;
                Faces.Add(value);
                if (CurrentFace is EmptyFace) CurrentFace = value;
            }
        }
    }

    private enum Facing {
        Up = 3,
        Right = 0,
        Down = 1,
        Left = 2
    }

    private abstract class Face(string[] map) {
        public string[] Map { get; protected set; } = map;

        public Face? Up { get; set; }

        public int UpRotation { get; set; }

        public Face? Right { get; set; }

        public int RightRotation { get; set; }

        public Face? Down { get; set; }

        public int DownRotation { get; set; }

        public Face? Left { get; set; }

        public int LeftRotation { get; set; }

        public string this[int newY] => Map[newY];

        public Face? GetDiagonal(Facing neighbor, Facing neighborOfNeighbor) {
            var neighboringFace = neighbor switch {
                Facing.Up => Up,
                Facing.Right => Right,
                Facing.Down => Down,
                Facing.Left => Left,
                _ => throw new ArgumentOutOfRangeException(nameof(neighbor), neighbor, null)
            };

            if (neighboringFace == null) return null;

            var neighborsRotation = neighbor switch {
                Facing.Up => UpRotation,
                Facing.Right => RightRotation,
                Facing.Down => DownRotation,
                Facing.Left => LeftRotation,
                _ => throw new ArgumentOutOfRangeException(nameof(neighbor), neighbor, null)
            };

            var orientedNeighborOfNeighbor = (Facing)((((int)neighborOfNeighbor - neighborsRotation) % 4 + 4) % 4);
            return orientedNeighborOfNeighbor switch {
                Facing.Up => neighboringFace.Up,
                Facing.Right => neighboringFace.Right,
                Facing.Down => neighboringFace.Down,
                Facing.Left => neighboringFace.Left,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public int GetDiagonalRotation(Facing neighbor, Facing neighborOfNeighbor) {
            var neighboringFace = neighbor switch {
                Facing.Up => Up,
                Facing.Right => Right,
                Facing.Down => Down,
                Facing.Left => Left,
                _ => throw new ArgumentOutOfRangeException(nameof(neighbor), neighbor, null)
            };

            if (neighboringFace == null) throw new InvalidOperationException();

            var neighborsRotation = neighbor switch {
                Facing.Up => UpRotation,
                Facing.Right => RightRotation,
                Facing.Down => DownRotation,
                Facing.Left => LeftRotation,
                _ => throw new ArgumentOutOfRangeException(nameof(neighbor), neighbor, null)
            };

            var orientedNeighborOfNeighbor = (Facing)((((int)neighborOfNeighbor - neighborsRotation) % 4 + 4) % 4);
            return neighborsRotation + orientedNeighborOfNeighbor switch {
                Facing.Up => neighboringFace.UpRotation,
                Facing.Right => neighboringFace.RightRotation,
                Facing.Down => neighboringFace.DownRotation,
                Facing.Left => neighboringFace.LeftRotation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private sealed class MapFace(int row, int column, string[] map) : Face(map) {
        // ReSharper disable once StringLiteralTypo
        private readonly string identity = "ABCDEFGHIJKLMNOP".Substring(4 * row + column, 1);

        public int OffsetY { get; } = map[0].Length * row;

        public int OffsetX { get; } = map[0].Length * column;

        public override string ToString() => identity + Environment.NewLine + Environment.NewLine +
                                             string.Join(Environment.NewLine, Map);

        public long Password((Face face, int x, int y, int f) finalPosition) {
            return 1000 * (OffsetY + finalPosition.y + 1) + 4 * (OffsetX + finalPosition.x + 1) + finalPosition.f;
        }
    }

    private sealed class EmptyFace(int size)
        : Face(Enumerable.Range(0, size).Select(_ => new string(' ', size)).ToArray());

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
                var slice = normalizedLines.Skip(row * size).Take(size).Select(l => l.Substring(column * size, size))
                    .ToArray();

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
                        var history = positionHistory?[face][x, y];
                        if (history != null) {
                            buffer.Append(Facings[history.Value]);
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

    private static void Fold(Cube cube) {
        // make the inferred connections
        var notBored = true;

        while (notBored) {
            notBored = false;
            for (var row = 0; row < cube.NumberOfRows; ++row) {
                for (var column = 0; column < cube.NumberOfColumns; ++column) {
                    var face = cube[row, column];
                    if (face is EmptyFace) continue;

                    if (face.Up == null) {
                        if (face.GetDiagonal(Facing.Right, Facing.Up) != null) {
                            face.Up = face.GetDiagonal(Facing.Right, Facing.Up);
                            face.UpRotation = face.GetDiagonalRotation(Facing.Right, Facing.Up) - 1;
                            notBored = true;
                        }
                        else if (face.GetDiagonal(Facing.Left, Facing.Up) != null) {
                            face.Up = face.GetDiagonal(Facing.Left, Facing.Up);
                            face.UpRotation = face.GetDiagonalRotation(Facing.Left, Facing.Up) + 1;
                            notBored = true;
                        }
                    }

                    if (face.Right == null) {
                        if (face.GetDiagonal(Facing.Down, Facing.Right) != null) {
                            face.Right = face.GetDiagonal(Facing.Down, Facing.Right);
                            face.RightRotation = face.GetDiagonalRotation(Facing.Down, Facing.Right) - 1;
                            notBored = true;
                        }
                        else if (face.GetDiagonal(Facing.Up, Facing.Right) != null) {
                            face.Right = face.GetDiagonal(Facing.Up, Facing.Right);
                            face.RightRotation = face.GetDiagonalRotation(Facing.Up, Facing.Right) + 1;
                            notBored = true;
                        }
                    }

                    if (face.Down == null) {
                        if (face.GetDiagonal(Facing.Left, Facing.Down) != null) {
                            face.Down = face.GetDiagonal(Facing.Left, Facing.Down);
                            face.DownRotation = face.GetDiagonalRotation(Facing.Left, Facing.Down) - 1;
                            notBored = true;
                        }
                        else if (face.GetDiagonal(Facing.Right, Facing.Down) != null) {
                            face.Down = face.GetDiagonal(Facing.Right, Facing.Down);
                            face.DownRotation = face.GetDiagonalRotation(Facing.Right, Facing.Down) + 1;
                            notBored = true;
                        }
                    }

                    if (face.Left == null) {
                        if (face.GetDiagonal(Facing.Up, Facing.Left) != null) {
                            face.Left = face.GetDiagonal(Facing.Up, Facing.Left);
                            face.LeftRotation = face.GetDiagonalRotation(Facing.Up, Facing.Left) - 1;
                            notBored = true;
                        }
                        else if (face.GetDiagonal(Facing.Down, Facing.Left) != null) {
                            face.Left = face.GetDiagonal(Facing.Down, Facing.Left);
                            face.LeftRotation = face.GetDiagonalRotation(Facing.Down, Facing.Left) + 1;
                            notBored = true;
                        }
                    }
                }
            }
        }
    }

    private ((Face face, int x, int y, int f), IDictionary<Face, int?[,]>) Walk(Cube cube,
        (Face face, int x, int y, int f) initialPosition, (int forward, int turn)[] instructions) {
        var positionHistory = cube.Faces.ToDictionary(f => f, _ => new int?[cube.FaceSize, cube.FaceSize]);
        var position = initialPosition;
        positionHistory[cube.CurrentFace][position.x, position.y] = position.f;

        foreach (var instruction in instructions) {
            for (var i = 0; i < instruction.forward; ++i) {
                var newFace = position.face;
                var newX = position.x + (position.f == 0 ? 1 : position.f == 2 ? -1 : 0);
                var newY = position.y + (position.f == 1 ? 1 : position.f == 3 ? -1 : 0);
                var newFacing = position.f;

                (int, int, int) Rotate(int x, int y, int f, int rotation) {
                    if (rotation == 0) return (x, y, f);

                    var requiredRightTurns = (rotation % 4 + 4) % 4;

                    while (requiredRightTurns-- > 0) {
                        var tempX = x;
                        x = y;
                        y = cube.FaceSize - 1 - tempX;
                        f -= 1;
                    }

                    f = (f % 4 + 4) % 4;
                    return (x, y, f);
                }

                if (newX < 0) {
                    var rotation = newFace.LeftRotation;
                    newFace = newFace.Left ?? throw new InvalidOperationException();
                    (newX, newY, newFacing) = Rotate(cube.FaceSize - 1, newY, newFacing, rotation);
                }

                if (newY < 0) {
                    var rotation = newFace.UpRotation;
                    newFace = newFace.Up ?? throw new InvalidOperationException();
                    (newX, newY, newFacing) = Rotate(newX, cube.FaceSize - 1, newFacing, rotation);
                }

                if (newX >= cube.FaceSize) {
                    var rotation = newFace.RightRotation;
                    newFace = newFace.Right ?? throw new InvalidOperationException();
                    (newX, newY, newFacing) = Rotate(0, newY, newFacing, rotation);
                }

                if (newY >= cube.FaceSize) {
                    var rotation = newFace.DownRotation;
                    newFace = newFace.Down ?? throw new InvalidOperationException();
                    (newX, newY, newFacing) = Rotate(newX, 0, newFacing, rotation);
                }

                if (newFace[newY][newX] == '#') {
                    break;
                }

                position = (newFace, newX, newY, newFacing);
                positionHistory[newFace][position.x, position.y] = position.f;
            }

            position.f = ((position.f + instruction.turn) % 4 + 4) % 4;
            positionHistory[position.face][position.x, position.y] = position.f;
        }

        return (position, positionHistory);
    }

    protected override long SolvePartOne() {
        var (cube, instructions) = Parse(Input);
        Link(cube);

        var position = (cube.CurrentFace, 0, 0, 0);
        //Trace.WriteLine(Render(cube));

        var (finalPosition, positionHistory) = Walk(cube, position, instructions);

        Trace.WriteLine(Render(cube, positionHistory));

        return ((MapFace)finalPosition.face).Password(finalPosition);
    }

    protected override long SolvePartTwo() {
        var (cube, instructions) = Parse(Input);
        Fold(cube);

        var position = (cube.CurrentFace, 0, 0, 0);
        //Trace.WriteLine(Render(cube));

        var (finalPosition, positionHistory) = Walk(cube, position, instructions);

        Trace.WriteLine(Render(cube, positionHistory));

        return ((MapFace)finalPosition.face).Password(finalPosition);
    }

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

        var (cube, instructions) = Parse(ExampleInput);
        Link(cube);
        var (_, positionHistory) = Walk(cube, (cube.CurrentFace, 0, 0, 0), instructions);

        var actual = Render(cube, positionHistory);
        Output.WriteLine(actual);

        Assert.Equal(expected.ReplaceLineEndings(), actual);
    }

    [Fact]
    public void WalksFoldedMapCorrectly() {
        const string expected = @"        >>v#    
        .#v.    
        #.v.    
        ..v.    
...#..^...v#    
.>>>>>^.#.>>    
.^#....#....    
.^........#.    
        ...#..v.
        .....#v.
        .#v<<<<.
        ..v...#.
";

        var (cube, instructions) = Parse(ExampleInput);
        Fold(cube);

        var (_, positionHistory) = Walk(cube, (cube.CurrentFace, 0, 0, 0), instructions);

        var actual = Render(cube, positionHistory);
        Output.WriteLine(actual);

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