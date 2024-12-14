using System.Reflection;
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

            var solverAttribute = type.GetCustomAttribute<SolverAttribute>();
            solvers[
                @event.ToLowerInvariant() + "-" +
                (solverAttribute?.Key ?? type.Name.ToLowerInvariant() + (prefix == string.Empty ? string.Empty : "-" + prefix))] = type;
        }

        return this;
    }
}