using System.Text;
using AdventOfCode.Core;
using AdventOfCode.Core.Points;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day21(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    private readonly ILookup<(char from, char to),string> numericPaths = CreateAllNumericPaths();
    
    private readonly Dictionary<(char, char, int), long> minimumLengthCache = [];
    
    private readonly Dictionary<(char, char, int), long> fastCache = [];

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

    private static string OptimisedDirectionalPresses(char curr, char next) {
        return (curr) switch {
            'A' when next == '^' => "A<A",
            'A' when next == '<' => "Av<<A",
            'A' when next == 'v' => "A<vA",
            'A' when next == '>' => "AvA",

            '^' when next == 'A' => "A>A",
            '^' when next == '<' => "Av<A",
            '^' when next == 'v' => "AvA",
            '^' when next == '>' => "Av>A",

            '<' when next == 'A' => "A>>^A",
            '<' when next == '^' => "A>^A",
            '<' when next == 'v' => "A>A",
            '<' when next == '>' => "A>>A",

            'v' when next == 'A' => "A^>A",
            'v' when next == '^' => "A^A",
            'v' when next == '<' => "A<A",
            'v' when next == '>' => "A>A",

            '>' when next == 'A' => "A^A",
            '>' when next == '^' => "A<^A",
            '>' when next == '<' => "A<<A",
            '>' when next == 'v' => "A<A",

            _ => "AA"
        };
    }

    private static string PressesForPresses(string required) {
        var transitions = new[] { 'A' }.Concat(required).Pairwise();
            
        var sb = new StringBuilder(required.Length * 4);

        foreach (var transition in transitions) {
            sb.Append(OptimisedDirectionalPresses(transition.Item1, transition.Item2)[1..^1]);
            sb.Append('A');
        }

        return sb.ToString();
    }

    private long FastMinimumLength(char curr, char next, int remainingEncodings) {
        if (remainingEncodings == 0 || curr == next)
            return 1;

        if (fastCache.TryGetValue((curr, next, remainingEncodings), out long result))
            return result;
            
        var presses = OptimisedDirectionalPresses(curr, next);
        
        return fastCache[(curr, next, remainingEncodings)] = presses.Pairwise().Sum(pair => FastMinimumLength(pair.Item1, pair.Item2, remainingEncodings - 1));
    }

    private long MinimumLength(char curr, char next, int remainingEncodings) {
        if (minimumLengthCache.TryGetValue((curr, next, remainingEncodings), out var value))
            return value;
        
        return minimumLengthCache[(curr, next, remainingEncodings)] = MinimumLengthInner(curr, next, remainingEncodings);
    }

    /// <summary>
    ///     Brute-force calculate the minimum length for an encoding
    /// </summary>
    /// <param name="curr"></param>
    /// <param name="next"></param>
    /// <param name="remainingEncodings"></param>
    /// <remarks>
    ///     Equivalent to the following:
    ///     <![CDATA[
    ///     private long DebuggableMinimumLengthInner(char curr, char next, int remainingEncodings) {
    ///         if (remainingEncodings == 0)
    ///             return 1;
    ///     
    ///         var directionalPresses = DirectionalPresses(curr, next);
    ///         var min = long.MaxValue;
    ///         foreach (var d in directionalPresses) {
    ///             var length = 0L;
    ///             var steps = new[] { 'A' }.Concat(d).Concat(new[] { 'A' }).Pairwise();
    ///             
    ///             foreach (var p in steps)
    ///                 length += MinimumLength(p.Item1, p.Item2, remainingEncodings - 1);
    ///     
    ///             if (length < min)
    ///                 min = length;
    ///         }
    ///     
    ///         return min;
    ///     }
    ///     ]]>
    /// </remarks>
    /// <returns></returns>
    private long MinimumLengthInner(char curr, char next, int remainingEncodings) => 
        remainingEncodings == 0 ? 1 : 
            DirectionalPresses(curr, next).Select(d => new[] { 'A' }.Concat(d).Concat(['A']).Pairwise()).Select(steps => steps.Sum(p => MinimumLength(p.Item1, p.Item2, remainingEncodings - 1))).Min();

    private long Solve(string code, int remainingEncodings) => 
        PressesForCode(code)
            .Min(p => new[] { 'A' }.Concat(p).Pairwise()
                .Sum(pair => FastMinimumLength(pair.Item1, pair.Item2, remainingEncodings)));

    protected override long SolvePartOne() => Shared.Split(Input).Sum(code => int.Parse(code[..^1]) * Solve(code, 2));

    protected override long SolvePartTwo() => Shared.Split(Input).Sum(code => int.Parse(code[..^1]) * Solve(code, 25));

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
    [InlineData("<A^A>^^AvvvA", "v<<A>>^A<A>AvA<^AA>A<vAAA^>A")]
    [InlineData("v<<A>>^A<A>AvA<^AA>A<vAAA>^A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA^>A<v<A>^A>AAvA^A<v<A>A^>AAAvA<^A>A")]
    // ReSharper restore StringLiteralTypo
    public void CalculatePressesForDirectionalKeypadCorrectly(string input, string expected) {
        // cheeky scamp, examples aren't optimal for higher number of keypads
        expected = expected.Replace("<v<", "v<<").Replace(">^>", ">>^");
            
        var actual = PressesForPresses(input);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A")]
    [InlineData("980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A")]
    [InlineData("179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A")]
    [InlineData("456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A")]
    [InlineData("379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A")]
    public void SolvesEachLengthOfPartOne(string input, string expected) {
        var actual = Solve(input, 2);
        Assert.Equal(expected.Length, actual);    
    }

    [Theory]
    [InlineData('A', '<')]
    [InlineData('A', 'v')]
    [InlineData('^', '>')]
    [InlineData('<', 'A')]
    [InlineData('v', 'A')]
    [InlineData('>', '^')]
    public void TestIfBestCandidatesChange(char curr, char next) {
        var options = DirectionalPresses(curr, next);

        for (var i = 0; i < 10; ++i) {
            var remainingEncodings = i;
            var answers = options.Select(option => (
                    option, 
                    length: new[] { 'A' }.Concat(option).Concat(['A']).Pairwise().Sum(p => MinimumLength(p.Item1, p.Item2, remainingEncodings))))
                .ToArray();

            var trace = answers.Select(a => $"{a.option}={a.length}").Aggregate((a, b) => $"{a}, {b}");
            
            if (answers.GroupBy(p => p.length).Count() == 1) {
                Trace.WriteLine($"For {i} downstream keypads, it's a tie ({trace})");
                continue;
            }
            
            var best = answers.OrderBy(p => p.length).First();
            Trace.WriteLine($"For {i} keypads, {best.option} is best ({trace})");
        }
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day21(ExampleInput, Output).SolvePartOne();
        Assert.Equal(126384, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day21(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(154115708116294, actual);
    }
}