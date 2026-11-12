using AdventOfCode.Shared;
using Xunit.Abstractions;

namespace AdventOfCode.Services;

public class SolverFactory {
    private readonly IDictionary<PuzzleSpecification, Type> _solvers =
        new SortedDictionary<PuzzleSpecification, Type>(new SolverComparer());

    public ISolver? Create(int @event, int day, string? suffix, string input, ITestOutputHelper? output = null) {
        var candidates = _solvers.Keys.Where(k => k.Event == @event && k.Day == day).ToArray();

        if (candidates.Length > 1 && !string.IsNullOrWhiteSpace(suffix))
            candidates = [.. candidates.Where(k => k.Suffix?.StartsWith(suffix) == true)];

        if (candidates.Length > 1)
            candidates = [.. candidates.Where(k => k.Suffix == suffix)];

        return candidates.Length switch {
            1 => Instantiate(_solvers[candidates[0]], input, output),
            0 => null,
            _ => throw new AmbiguousSolverException(candidates.Select(c => c.ToString()))
        };
    }

    public IDictionary<string, ISolver> CreateAll(int @event, int day, string input, ITestOutputHelper? output = null) {
        return _solvers.Keys.Where(k => k.Event == @event && k.Day == day)
            .ToDictionary(k => k.ToString(), k => Instantiate(_solvers[k], input, output));
    }

    private static ISolver Instantiate(Type solver, string input, ITestOutputHelper? output = null) {
        var constructor = solver.GetConstructors().First();

        var parameters =
            constructor.GetParameters()
                .Select(parameter =>
                    parameter.ParameterType == typeof(string) ? (object)input :
                    parameter.ParameterType == typeof(ITestOutputHelper) ? output : null)
                .ToArray();

        return (ISolver)constructor.Invoke(parameters);
    }

    public IEnumerable<string> List() {
        return _solvers.Keys.Select(ps => ps.ToString());
    }

    public SolverFactory AddAssembly<T>(string prefix) {
        foreach (var type in typeof(T).Assembly.GetTypes().Where(t => typeof(ISolver).IsAssignableFrom(t))) {
            if (type.Name.Contains("TemplateSolver")) continue;
            if (type.IsAbstract) continue;

            // determine the puzzle event
            var namespaceMatch = Puzzle.EventPattern.Match(type.Namespace ?? string.Empty);
            if (!namespaceMatch.Success) {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"# {type.FullName} has has missing or unexpected namespace (expected: {{**}}._2015; found: {type.Namespace})");
                Console.ResetColor();
                continue;
            }

            var @event = Puzzle.GetEvent(namespaceMatch);

            // determine the puzzle day
            var nameMatch = Puzzle.DayPattern.Match(type.Name);
            if (!nameMatch.Success) {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"# {type.FullName} has unexpected type name (expected: Day1; found: {type.Name})");
                Console.ResetColor();
                continue;
            }

            var day = Puzzle.GetDay(nameMatch);
            var suffix = Puzzle.GetSuffix(nameMatch);
            var hint = string.IsNullOrWhiteSpace(prefix) ? suffix : $"{suffix}#{prefix}";

            _solvers[new PuzzleSpecification(@event, day, hint)] = type;
        }

        return this;
    }

    private readonly record struct PuzzleSpecification(int Event, int Day, string? Suffix) {
        public override string ToString() =>
            string.IsNullOrWhiteSpace(Suffix)
                ? $"{Event} Day {Day}"
                : $"{Event} Day {Day} ({Suffix})";
    }

    private class SolverComparer : IComparer<PuzzleSpecification> {
        public int Compare(PuzzleSpecification x, PuzzleSpecification y) {
            if (x.Event > y.Event) return 1;
            if (x.Event < y.Event) return -1;

            if (x.Day > y.Day) return 1;
            if (x.Day < y.Day) return -1;

            return string.CompareOrdinal(x.Suffix, y.Suffix);
        }
    }
}