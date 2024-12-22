using AdventOfCode.Core;
using AdventOfCode.Core.Points;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day21(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    private readonly ILookup<(char from, char to),string> numericPaths = CreateAllNumericPaths();

    /// <summary>
    ///     Gets the coordinate of the door button
    /// </summary>
    /// <remarks>
    ///
    ///   Origin is 'gap'
    ///   X is + for left to right
    ///   Y is + for bottom to top
    /// 
    ///     +---+---+---+
    ///     | 7 | 8 | 9 |
    ///     +---+---+---+
    ///     | 4 | 5 | 6 |
    ///     +---+---+---+
    ///     | 1 | 2 | 3 |
    ///     +---+---+---+
    ///         | 0 | A |
    ///         +---+---+
    /// 
    /// </remarks>
    private static readonly Dictionary<char,(int x, int y)> NumericCoordinates = new() {
        { 'A' ,  (2, 0) },
        { '0' ,  (1, 0) },
        { '1' ,  (0, 1) },
        { '2' ,  (1, 1) },
        { '3' ,  (2, 1) },
        { '4' ,  (0, 2) },
        { '5' ,  (1, 2) },
        { '6' ,  (2, 2) },
        { '7' ,  (0, 3) },
        { '8' ,  (1, 3) },
        { '9' ,  (2, 3) }
    };

    private static ILookup<(char from, char to), string> CreateAllNumericPaths() {
        var inverse = NumericCoordinates.ToDictionary(p => p.Value, p => p.Key);
        var particles = new Queue<(char start, (int x, int y) position, string path)>(NumericCoordinates.Select(p => (p.Key, (p.Value), "")));
        var seen = new HashSet<(char start, (int x, int y) position, string path)>();

        while (particles.TryDequeue(out var particle)) {
            if (!seen.Add(particle) || particle.path.Length > 5) continue;
            
            if (inverse.ContainsKey(particle.position.Up())) // inverted y-axis
                particles.Enqueue(particle with { position = particle.position.Up(), path = particle.path + "v" });
            
            if (inverse.ContainsKey(particle.position.Right()))
                particles.Enqueue(particle with { position = particle.position.Right(), path = particle.path + ">" });

            if (inverse.ContainsKey(particle.position.Down())) // inverted y-axis
                particles.Enqueue(particle with { position = particle.position.Down(), path = particle.path + "^" });
            
            if (inverse.ContainsKey(particle.position.Left()))
                particles.Enqueue(particle with { position = particle.position.Left(), path = particle.path + "<" });
        }

        var pathLengths = seen.GroupBy(particle => (particle.start, particle.position))
            .ToDictionary(g => g.Key, g => g.Min(gg => gg.path.Length)); 
        
        return seen
            .Where(particle => pathLengths[(particle.start, particle.position)] == particle.path.Length)
            .OrderBy(particle => particle.start)
            .ThenBy(particle => inverse[particle.position])
            .ToLookup(particle => (from: particle.start, to: inverse[particle.position]), particle => particle.path);
    }
    
    private string[] PressesForCode(string code) {
        return new[] { 'A' }.Concat(code).Pairwise()
            .Aggregate(new [] { string.Empty }, (acc, pair) => 
                acc.SelectMany(o => numericPaths[(pair.Item1, pair.Item2)].Select(p => o + p + "A")).ToArray());
    }
    
    /// <summary>
    ///     Gets the possible keypresses for each directional state change
    /// </summary>
    /// <remarks>
    ///     +---+---+
    ///     | ^ | A |
    /// +---+---+---+
    /// | « | v | » |
    /// +---+---+---+
    /// </remarks>
    private static string[] DirectionalPresses(char curr, char next) {
        if (curr == next) return [string.Empty];
        
        switch (curr) {
            case 'A' when next == '^': return ["<"];
            case 'A' when next == '<': return ["v<<","<v<"];
            case 'A' when next == 'v': return ["<v","v<"];
            case 'A' when next == '>': return ["v"];

            case '^' when next == 'A': return [">"];
            case '^' when next == '<': return ["v<"];
            case '^' when next == 'v': return ["v"];
            case '^' when next == '>': return [">v","v>"];
            
            case '<' when next == 'A': return [">>^",">^>"];
            case '<' when next == '^': return [">^"];
            case '<' when next == 'v': return [">"];
            case '<' when next == '>': return [">>"];
            
            case 'v' when next == 'A': return [">^","^>"];
            case 'v' when next == '^': return ["^"];
            case 'v' when next == '<': return ["<"];
            case 'v' when next == '>': return [">"];
            
            case '>' when next == 'A': return ["^"];
            case '>' when next == '^': return ["<^","^<"];
            case '>' when next == '<': return ["<<"];
            case '>' when next == 'v': return ["<"];
            
            default: throw new InvalidOperationException();
        }
    }
    
    private static string[] PressesForPresses(string required) {
        return new[] { 'A' }.Concat(required).Pairwise()
            .Aggregate(new[] { string.Empty }, (acc, pair) => 
                acc.SelectMany(o => DirectionalPresses(pair.Item1, pair.Item2).Select(p => o + p + "A")).ToArray());
    }
    
    private static string YourPressesForPresses(string required) {
        return new[] { 'A' }.Concat(required).Pairwise()
            .Aggregate(string.Empty, (acc, pair) => 
                acc + DirectionalPresses(pair.Item1, pair.Item2).Last() + "A");
    }
    
    private string Solve(string code) {
        var shortest = int.MaxValue;
        var candidate = string.Empty;
        
        Trace.WriteLine($"Code: {code}");
        var pressesForCode = PressesForCode(code);

        foreach (var pressesForCodeOption in pressesForCode) {
            Trace.WriteLine($"1st Keypad: {pressesForCodeOption}");
            var pressesForPresses = PressesForPresses(pressesForCodeOption);

            foreach (var pressesForPressesOption in pressesForPresses) {
                Trace.WriteLine($"2nd Keypad: {pressesForPressesOption}");
                var yourPressesForPresses = YourPressesForPresses(pressesForPressesOption);

                if (yourPressesForPresses.Length < shortest) {
                    shortest = yourPressesForPresses.Length;
                    candidate = yourPressesForPresses;
                }
            }
        }

        return candidate;
    }

    [Theory]
    [InlineData("029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A")]
    [InlineData("980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A")]
    [InlineData("179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A")]
    [InlineData("456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A")]
    [InlineData("379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A")]
    public void SolvesEachLengthOfPartOne(string input, string expected) {
        var actual = new string(Solve(input));
        Trace.WriteLine(expected);
        Trace.WriteLine(actual);
        Assert.Equal(expected.Length, actual.Length);    
    }
    
    protected override long SolvePartOne() => Shared.Split(Input).Sum(code => int.Parse(code[..^1]) * Solve(code).Length);
    protected override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = 
        """
        029A
        980A
        179A
        456A
        379A
        """;

    [Fact]
    public void GeneratesAllPotentialPaths() {
        var solver = new Day21();
        foreach (var pair in solver.numericPaths) {
            Trace.WriteLine($"({pair.Key.from} -> {pair.Key.to}): {string.Join(", ", pair)}");
        }
        
        Assert.NotEmpty(solver.numericPaths);
    }
    
    [Fact]
    public void CalculatePressesForNumericKeypadCorrectly() {
        var actual = PressesForCode("029A");
        foreach (var option in actual)
            Trace.WriteLine(option);
        
        // ReSharper disable once StringLiteralTypo
        const string expected = "<A^A>^^AvvvA";
        Assert.Contains(expected, actual);
    }
    
    [Theory]
    // ReSharper disable StringLiteralTypo
    [InlineData("<A^A>^^AvvvA", "v<<A>>^A<A>AvA<^AA>A<vAAA>^A")]
    [InlineData("v<<A>>^A<A>AvA<^AA>A<vAAA>^A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A")]
    // ReSharper restore StringLiteralTypo
    public void CalculatePressesForDirectionalKeypadCorrectly(string input, string expected) {
        var actual = PressesForPresses(input);
        foreach (var option in actual)
            Trace.WriteLine(option);
        
        Assert.Contains(expected, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day21(ExampleInput, Output).SolvePartOne();
        Assert.Equal(126384, actual);
    }
}