using AdventOfCode.Core;
using Xunit;

namespace AdventOfCode.CSharp._2022;

public class Day18 : Solver {
    public Day18(string? input = null) : base(input) { }

    public override long SolvePartOne() {
        var origins = Parse();
        var faces = Faces(origins);
        
        return faces.GroupBy(f => f).Count(g => g.Count() == 1);
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

    private IEnumerable<(int, int, int, Face)> Faces(IEnumerable<(int, int, int)> positions) {
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

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

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
        var actual = new Day18(ExampleInput).SolvePartOne();
        Assert.Equal(64, actual);
    }
}