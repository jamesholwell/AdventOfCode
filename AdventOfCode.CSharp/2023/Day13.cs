using System.Text;

using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day13 : Solver {
    public Day13(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var patterns = Input.SplitBy("\n\n");
        var accumulator = 0;

        foreach (var pattern in patterns) {
            // look for a horizontal mirror
            accumulator += 100 * FindHorizontalMirror(pattern);

            // rotate and look for a vertical mirror, horizontally
            accumulator += FindHorizontalMirror(Rotate(pattern));
        }

        return accumulator;
    }

    private string Rotate(string pattern) {
        var sb = new StringBuilder();

        var rows = Shared.Split(pattern);

        // thinking in the image domain i.e.
        for (var y = 0; y < rows[0].Length; ++y) {
            for (var x = 0; x < rows.Length; ++x)
                sb.Append(rows[x][y]);
            sb.Append("\n");
        }

        return sb.ToString();
    }

    private static int FindHorizontalMirror(string pattern, int excludeMirror = 0) {
        var rows = Shared.Split(pattern)
            .Select((r, i) => (r, i))
            .ToDictionary(p => p.i, p => p.r);

        var height = rows.Count;

        // create the set of identical rows
        var identicalRows = new HashSet<(int, int)>(
            rows
                .SelectMany(
                    r =>
                        rows.Where(p => r.Value == p.Value)
                            .Select(p => (r.Key, p.Key))).ToArray());


        var candidateMirrorsAndMatches =
            Enumerable
                .Range(0, height - 1)
                .ToDictionary(
                    i => i,
                    i =>
                        Enumerable
                            .Range(0, height - 1)
                            .Select(j => (i - j, 1 + i + j))
                            .Where(
                                p => p.Item1 >= 0 && p.Item2 >= 0
                                                  && p.Item1 < height && p.Item2 < height)
                            .ToArray());

        var mirrors = candidateMirrorsAndMatches
            .Where(p =>
                p.Key != excludeMirror - 1
                && p.Value.All(r => identicalRows.Contains(r)))
            .ToArray();

        return mirrors.Length switch {
            > 1 => throw new InvalidOperationException("Too many mirrors"),
            1 => 1 + mirrors.Single().Key,
            _ => 0
        };
    }

    public override long SolvePartTwo() {
        var patterns = Input.SplitBy("\n\n");
        var accumulator = 0;

        foreach (var pattern in patterns) {
            var existingHMirror = FindHorizontalMirror(pattern);
            var existingVMirror = FindHorizontalMirror(Rotate(pattern));
            var patternBuilder = new StringBuilder(pattern.Length);
            
            for (var i = 0; i < pattern.Length; ++i) {
                if (pattern[i] != '.' && pattern[i] != '#') continue;

                patternBuilder.Clear();
                patternBuilder.Append(pattern);
                patternBuilder.Remove(i, 1);
                patternBuilder.Insert(i, pattern[i] == '.' ? '#' : '.');
                var newPattern = patternBuilder.ToString();
                    
                var newHMirror = FindHorizontalMirror(newPattern, existingHMirror);
                if (newHMirror > 0) {
                    accumulator += 100 * newHMirror;
                    break;
                }

                var newVMirror = FindHorizontalMirror(Rotate(newPattern), existingVMirror);
                if (newVMirror > 0) {
                    accumulator += newVMirror;
                    break;
                }
            }
        }

        return accumulator;
    }

    private const string? ExampleInput = @"
#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day13(ExampleInput, Output).SolvePartOne();
        Assert.Equal(405, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day13(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(400, actual);
    }
}