using AdventOfCode.Core;
using Xunit.Abstractions;

namespace AdventOfCode.Infrastructure;

public class SolverFactory {
    private readonly IDictionary<string, Type> solvers = new SortedDictionary<string, Type>(new SolverComparer());

    public ISolver? Create(string day, string @event, string input, string? solver = null, ITestOutputHelper? output = null) {
        var prefix = $"{@event.ToLowerInvariant()}-{day.ToLowerInvariant()}";
        if (solver != null)
            prefix += $"-{solver.ToLowerInvariant()}";
        
        var candidates = solvers.Keys.Where(k => k == prefix).ToArray();
        if (candidates.Length != 1)
            candidates = solvers.Keys.Where(k => k.StartsWith(prefix)).ToArray();
        
        return candidates.Length switch {
            1 => Instantiate(solvers[candidates.Single()], input, output),
            0 => null,
            _ => throw new AmbiguousSolverException(candidates)
        };
    }

    public IDictionary<string, ISolver> CreateAll(string day, string @event, string input, ITestOutputHelper? output = null) {
        return solvers.Keys.Where(k => k.StartsWith($"{@event.ToLowerInvariant()}-{day.ToLowerInvariant()}"))
            .ToDictionary(k => k, k => Instantiate(solvers[k], input, output));
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

    public ICollection<string> List() {
        return this.solvers.Keys;
    }

    public SolverFactory AddAssembly<T>(string prefix) {
        foreach (var type in typeof(T).Assembly.GetTypes().Where(t => typeof(ISolver).IsAssignableFrom(t))) {
            if (type.Name.Contains("TemplateSolver")) continue;
            if (type.IsAbstract) continue;

            var @event = type.Namespace == null
                ? string.Empty
                : type.Namespace[(type.Namespace.LastIndexOf('.') + 1)..].TrimStart('_');

            var parts = type.Name.Split(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'], 2,
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var day = parts.Length == 2 ? type.Name[..^parts[1].Length] : type.Name;
            var suffix = parts.Length == 2 ? parts[1].ToLowerInvariant() : string.Empty;
            
            solvers[
                @event.ToLowerInvariant() + "-" + day.ToLowerInvariant() +
                (prefix == string.Empty ? string.Empty : "-" + prefix) +
                (suffix == string.Empty ? string.Empty : "-" + suffix) 
            ] = type;
        }

        return this;
    }
}