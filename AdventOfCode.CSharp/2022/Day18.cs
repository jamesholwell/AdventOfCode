using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day18(string? input = null, ITestOutputHelper? outputHelper = null) : Solver(input, outputHelper) {
    protected override long SolvePartOne() {
        var origins = Parse();
        var faces = Faces(origins);

        return faces.GroupBy(f => f).Count(g => g.Count() == 1);
    }

    protected override long SolvePartTwo() {
        var origins = Parse().ToArray();

        var maxX = origins.Max(o => o.x);
        var maxY = origins.Max(o => o.y);
        var maxZ = origins.Max(o => o.z);
        var filled = new bool[maxX + 1, maxY + 1, maxZ + 1];

        foreach (var origin in origins)
            filled[origin.x, origin.y, origin.z] = true;

        var seen = new HashSet<(int x, int y, int z)>();
        var fills = new Queue<(int x, int y, int z)>();
        fills.Enqueue((0, 0, 0));

        Trace.WriteLine($"Search space is {maxX} * {maxY} * {maxZ} = {maxX * maxY * maxZ}");

        while (fills.TryDequeue(out var c)) {
            if (!seen.Add(c))
                continue;

            filled[c.x, c.y, c.z] = true;

            // towards
            if (c.z > 0 && !filled[c.x, c.y, c.z - 1]) fills.Enqueue((c.x, c.y, c.z - 1));

            // right
            if (c.x < maxX && !filled[c.x + 1, c.y, c.z]) fills.Enqueue((c.x + 1, c.y, c.z));

            // up
            if (c.y < maxY && !filled[c.x, c.y + 1, c.z]) fills.Enqueue((c.x, c.y + 1, c.z));

            // away 
            if (c.z < maxZ && !filled[c.x, c.y, c.z + 1]) fills.Enqueue((c.x, c.y, c.z + 1));

            // left
            if (c.x > 0 && !filled[c.x - 1, c.y, c.z]) fills.Enqueue((c.x - 1, c.y, c.z));

            // down
            if (c.y > 0 && !filled[c.x, c.y - 1, c.z]) fills.Enqueue((c.x, c.y - 1, c.z));
        }

        var allCoordinates = Enumerable.Range(0, maxX)
            .SelectMany(x => Enumerable.Range(0, maxY).Select(y => (x, y)))
            .SelectMany(p => Enumerable.Range(0, maxZ).Select(z => (x: p.Item1, y: p.Item2, z)))
            .ToArray();

        var internalCubes = allCoordinates.Where(p => !filled[p.x, p.y, p.z]);

        var exteriorFaces = Faces(origins.Concat(internalCubes));

        return exteriorFaces.GroupBy(f => f).Count(g => g.Count() == 1);
    }

    private IEnumerable<(int x, int y, int z)> Parse() {
        foreach (var cube in Shared.Split(Input)) {
            var coords = cube.Split(',');
            var x = int.Parse(coords[0]);
            var y = int.Parse(coords[1]);
            var z = int.Parse(coords[2]);

            yield return (x, y, z);
        }
    }

    private enum Face {
        Front,
        Right,
        Up
    }

    private IEnumerable<(int x, int y, int z, Face f)> Faces(IEnumerable<(int, int, int)> positions) {
        /*
         * Decompose the cube into faces, there are three characteristic planes from the origin
         *
         *          U (y + 1)
         *         ____
         *       /    /|
         *      /____/ | R (x + 1)
         *      |    | /
         *      |____|/
         *     *   F
         *
         *  L = R (x)
         *  B = F (z + 1)
         *  D = U (y)
         *
         */

        foreach (var (x, y, z) in positions) {
            yield return (x, y, z, Face.Front); // F

            yield return (x + 1, y, z, Face.Right); // R

            yield return (x, y + 1, z, Face.Up); // U

            yield return (x, y, z + 1, Face.Front); // B

            yield return (x, y, z, Face.Right); // L

            yield return (x, y, z, Face.Up); // D
        }
    }

    private const string? ExampleInput = @"
2,2,2
1,2,2
3,2,2
2,1,2
2,3,2
2,2,1
2,2,3
2,2,4
2,2,6
1,2,5
3,2,5
2,1,5
2,3,5
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day18(ExampleInput, Output).SolvePartOne();
        Assert.Equal(64, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day18(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(58, actual);
    }
}