using System.Text;

using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day11 : Solver {
    public Day11(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var universe = Expand(Parse(Input));
        return PairAndMeasureDistance(universe);
    }

    public override long SolvePartTwo() {
        var universe = Expand(Parse(Input), 1000000);
        return PairAndMeasureDistance(universe);
    }

    private record Universe {
        public int Height;

        public int Width;
        
        private int numberOfGalaxies;

        public IDictionary<(int x, int y), int> Galaxies = new Dictionary<(int x, int y), int>();

        public void AddGalaxy(int x, int y) => Galaxies.Add((x, y), ++numberOfGalaxies);
        
        public bool IsGalaxy(int x, int y) => Galaxies.ContainsKey((x, y));
        
        public char GetSymbol(int x, int y) => Galaxies.TryGetValue((x, y), out var number) ? (char)('0' + number % 10) : '.';
    }

    private static Universe Parse(string input) {
        var lines = Shared.Split(input);

        var universe = new Universe {
            Width = lines[0].Length,
            Height = lines.Length
        };

        for (var y = 0; y < universe.Height; ++y)
            for (var x = 0; x < universe.Width; ++x)
                if (lines[y][x] == '#')
                    universe.AddGalaxy(x, y);

        return universe;
    }

    private static Universe Expand(Universe universe, int expansion = 2) {
        var emptyX = new HashSet<int>(Enumerable.Range(0, universe.Width));
        var emptyY = new HashSet<int>(Enumerable.Range(0, universe.Height));

        foreach (var (x, y) in universe.Galaxies.Keys) {
            if (emptyX.Contains(x)) emptyX.Remove(x);
            if (emptyY.Contains(y)) emptyY.Remove(y);
        }

        var expandedUniverse = new Universe() {
            Width = universe.Width + (expansion - 1) * emptyX.Count,
            Height = universe.Height + (expansion - 1) * emptyY.Count
        };

        foreach (var pair in universe.Galaxies) {
            var x = pair.Key.x + (expansion - 1) * emptyX.Count(i => i < pair.Key.x);
            var y = pair.Key.y + (expansion - 1) * emptyY.Count(i => i < pair.Key.y);
            expandedUniverse.AddGalaxy(x, y);
        }
        
        return expandedUniverse;
    }

    private static long PairAndMeasureDistance(Universe universe) {
        var pairs =
            from left in universe.Galaxies
            from right in universe.Galaxies
            where left.Value < right.Value
            select (left, right);

        var distances = pairs
            .Select(
                pair => (long)Math.Abs(pair.left.Key.x - pair.right.Key.x)
                        + Math.Abs(pair.left.Key.y - pair.right.Key.y));

        return distances.Sum();
    }

    private static string Render(Universe universe, bool showLabels = false) {
        var sb = new StringBuilder(universe.Width * universe.Height);
        
        for (var y = 0; y < universe.Height; ++y) {
            sb.Append(Environment.NewLine);

            for (var x = 0; x < universe.Width; ++x) {
                sb.Append(
                    showLabels ? universe.GetSymbol(x, y) :
                    universe.IsGalaxy(x, y) ? '#' : '.');
            }
        }
        
        sb.Append(Environment.NewLine);
        return sb.ToString();
    }
    
    private const string? ExampleInput = @"
...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....
";

    private const string ExampleInputExpanded = @"
....#........
.........#...
#............
.............
.............
........#....
.#...........
............#
.............
.............
.........#...
#....#.......
";
    
    private const string ExampleInputExpandedLabelled = @"
....1........
.........2...
3............
.............
.............
........4....
.5...........
............6
.............
.............
.........7...
8....9.......
";

    [Fact]
    public void RendersExampleInput() {
        var actual = Render(Parse(ExampleInput!));
        Trace.WriteLine(actual);
        
        Assert.Equal(ExampleInput, actual);
    }
    
    [Fact]
    public void RendersExampleInputExpanded() {
        var actual = Render(Expand(Parse(ExampleInput!)));
        Assert.Equal(ExampleInputExpanded, actual);
    }
    
    [Fact]
    public void RendersExampleInputExpandedLabelled() {
        var actual = Render(Expand(Parse(ExampleInput!)), true);
        Trace.WriteLine(actual);
        
        Assert.Equal(ExampleInputExpandedLabelled, actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day11(ExampleInput, Output).SolvePartOne();
        Assert.Equal(374, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample1() {
        var universe = Expand(Parse(ExampleInput!), 10);
        Output.WriteLine(Render(universe));
        var actual = PairAndMeasureDistance(universe);
        Assert.Equal(1030, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample2() {
        var universe = Expand(Parse(ExampleInput!), 100);
        var actual = PairAndMeasureDistance(universe);
        Assert.Equal(8410, actual);
    }
}