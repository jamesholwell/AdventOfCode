using System.Reflection;
using System.Runtime.Loader;
using AdventOfCode.Shared;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Services;

public class SolverFactory {
    private readonly IDictionary<PuzzleSpecification, Type> _solvers =
        new SortedDictionary<PuzzleSpecification, Type>(new SolverComparer());

    internal static SolverFactory Build(bool verbose) =>
        new SolverFactory()
            .DiscoverAssemblies(verbose)
            .AddAssembly<FSharp.Discovery>("fsharp");

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

    public SolverFactory DiscoverAssemblies(bool verbose = false) { 
        var basePath = Workflow.GetBasePath("AdventOfCode/Library/");

        foreach (var candidate in Directory.EnumerateFiles(basePath, "AdventOfCode.*.dll")) {
            if (candidate.EndsWith("AdventOfCode.Shared.dll"))
                continue;

            if (verbose)
                Console.WriteMessage($"# Discovered {candidate}");

            var context = new LibraryLoadContext(Path.GetFullPath(candidate), verbose);
            var assembly = context.LoadFromAssemblyPath(Path.GetFullPath(candidate));
            AddAssembly(assembly, string.Empty);
        }

        return this;
    }

    public SolverFactory AddAssembly<T>(string prefix) => AddAssembly(typeof(T).Assembly, prefix);

    private SolverFactory AddAssembly(Assembly assembly, string prefix) {
        foreach (var type in assembly.GetTypes().Where(t => typeof(ISolver).IsAssignableFrom(t))) {
            if (type.Name.Contains("TemplateSolver")) continue;
            if (type.IsAbstract) continue;

            // determine the puzzle event
            var namespaceMatch = Puzzle.EventPattern.Match(type.Namespace ?? string.Empty);
            if (!namespaceMatch.Success) {
                Console.WriteMessage($"# {type.FullName} has has missing or unexpected namespace (expected: {{**}}._2015; found: {type.Namespace})");
                continue;
            }

            var @event = Puzzle.GetEvent(namespaceMatch);

            // determine the puzzle day
            var nameMatch = Puzzle.DayPattern.Match(type.Name);
            if (!nameMatch.Success) {
                Console.WriteMessage($"# {type.FullName} has unexpected type name (expected: Day1; found: {type.Name})");
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

    private sealed class LibraryLoadContext(string basePath, bool verbose) : AssemblyLoadContext {
        private readonly AssemblyDependencyResolver _resolver = new(basePath);

        protected override Assembly? Load(AssemblyName name) {
            // redirect AdventOfCode.Shared to the existing
            if (name.Name == typeof(ISolver).Assembly.GetName().Name)
                return typeof(ISolver).Assembly;

            // redirect Xunit libraries to the existing
            if (name.Name == typeof(ITestOutputHelper).Assembly.GetName().Name)
                return typeof(ITestOutputHelper).Assembly; // Xunit.Abstractions

            var path = _resolver.ResolveAssemblyToPath(name);
            if (!verbose || path == null) return null;

            // show a warning if we refuse to load an unknown assembly
            Console.WriteError($"# Rejected request to load {name} from {path}", false);
            return null;
        }

        protected override nint LoadUnmanagedDll(string name) {
            Console.WriteError($"# Rejected request to load {name} (unmanaged)", false);
            return nint.Zero;
        }
    }
}