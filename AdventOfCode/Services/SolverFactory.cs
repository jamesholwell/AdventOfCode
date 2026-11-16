using System.Reflection;
using System.Runtime.Loader;
using AdventOfCode.Shared;
using Xunit.Abstractions;

namespace AdventOfCode.Services;

public class SolverFactory(bool verbose) {
    private readonly IDictionary<PuzzleSpecification, Type> _solvers =
        new SortedDictionary<PuzzleSpecification, Type>(new SolverComparer());

    internal static SolverFactory Build(bool verbose) =>
        new SolverFactory(verbose).DiscoverAssemblies();

    public ISolver? Create(PuzzleArguments puzzle, string input, ITestOutputHelper? output = null) {
        var candidates = _solvers.Keys.Where(k => k.Event == puzzle.Event && k.Day == puzzle.Day).ToArray();
        var hint = CombineParts(puzzle.LanguagePrefix, puzzle.Suffix);
        
        // if ambiguous, first try starts-with match
        if (candidates.Length > 1 && hint != null) {
            PuzzleSpecification[] filtered = [..candidates.Where(k => k.Suffix?.StartsWith(hint) == true)];
            if (filtered.Length > 0)
                candidates = filtered;
        }

        // otherwise try exact match (which also matches default i.e. hint == null)
        if (candidates.Length > 1) {
            PuzzleSpecification[] filtered = [.. candidates.Where(k => k.Suffix == hint)];
            if (filtered.Length > 0)
                candidates = filtered;
        }

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

    private SolverFactory DiscoverAssemblies() {
        var basePath = Workflow.GetBasePath("AdventOfCode/Library/");

        foreach (var candidate in Directory.EnumerateFiles(basePath, "AdventOfCode.*.dll")) {
            if (candidate.EndsWith("AdventOfCode.Shared.dll"))
                continue;

            if (verbose)
                Console.WriteMessage($"# Discovered {candidate}");

            var assemblyPath = Path.GetFullPath(candidate);
            var context = new LibraryLoadContext(assemblyPath);
            var assembly = context.LoadFromAssemblyPath(assemblyPath);

            var discoveryType = assembly.GetTypes()
                .FirstOrDefault(t => !t.IsAbstract && typeof(ISolverDiscovery).IsAssignableFrom(t));
            var discovery = discoveryType == null ? null : Activator.CreateInstance(discoveryType) as ISolverDiscovery;

            foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && typeof(ISolver).IsAssignableFrom(t)))
                AddSolver(type, discovery?.Prefix);
        }

        return this;
    }

    private void AddSolver(Type type, string? prefix) {
        // determine the puzzle event
        var namespaceMatch = Puzzle.EventPattern.Match(type.Namespace ?? string.Empty);
        if (!namespaceMatch.Success) {
            if (verbose)
                Console.WriteMessage($"# {type.FullName} has has missing or unexpected namespace (expected: {{**}}._2015; found: {type.Namespace})");
            
            return;
        }

        var @event = Puzzle.GetEvent(namespaceMatch);

        // determine the puzzle day
        var nameMatch = Puzzle.DayPattern.Match(type.Name);
        if (!nameMatch.Success) {
            if (verbose)
                Console.WriteMessage($"# {type.FullName} has unexpected type name (expected: Day1; found: {type.Name})");
            
            return;
        }

        var day = Puzzle.GetDay(nameMatch);
        var suffix = Puzzle.GetSuffix(nameMatch);
        var hint = CombineParts(prefix, suffix);

        _solvers[new PuzzleSpecification(@event, day, hint)] = type;
    }

    private static string? CombineParts(string? prefix, string? suffix) =>
        prefix != null && suffix != null ? $"{prefix}#{suffix}" : prefix ?? suffix;

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

    private sealed class LibraryLoadContext(string basePath) : AssemblyLoadContext {
        private readonly AssemblyDependencyResolver _resolver = new(basePath);

        private static readonly string[] AllowedAssemblyNames = [
            "FSharp.Core"
        ];

        protected override Assembly? Load(AssemblyName name) {
            // redirect AdventOfCode.Shared to the existing
            if (name.Name == typeof(ISolver).Assembly.GetName().Name)
                return typeof(ISolver).Assembly;

            // redirect Xunit libraries to the existing
            if (name.Name == typeof(ITestOutputHelper).Assembly.GetName().Name)
                return typeof(ITestOutputHelper).Assembly; // Xunit.Abstractions

            var path = _resolver.ResolveAssemblyToPath(name);
            if (path != null && AllowedAssemblyNames.Contains(name.Name))
                return LoadFromAssemblyPath(path);

            if (path == null) return null;

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