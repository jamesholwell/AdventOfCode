using System.Reflection;

namespace AdventOfCode.Core;

public class SolverFactory {
    private readonly Dictionary<string, Type> solvers;

    public SolverFactory() {
        solvers = new Dictionary<string, Type>();
    }

    public ISolver? Create(string day, string @event, string input, string? solver = null) {
        var prefix = $"{@event.ToLowerInvariant()}-{day.ToLowerInvariant()}-{solver}";
        var candidates = solvers.Keys.Where(k => k.StartsWith(prefix)).ToArray();

        return candidates.Length switch {
            1 => Instantiate(solvers[candidates.Single()], input),
            0 => null,
            _ => throw new AmbiguousSolverException(candidates)
        };
    }

    public IDictionary<string, ISolver> CreateAll(string day, string @event, string input) {
        return solvers.Keys.Where(k => k.StartsWith($"{@event.ToLowerInvariant()}-{day.ToLowerInvariant()}-"))
            .ToDictionary(k => k, k => Instantiate(solvers[k], input));
    }

    private ISolver Instantiate(Type solver, string input) {
        var constructor = solver.GetConstructors().First();

        var parameters =
            constructor.GetParameters()
                .Select(parameter => parameter.ParameterType == typeof(string) ? input : null)
                .Cast<object?>()
                .ToArray();

        return (ISolver)constructor.Invoke(parameters);
    }

    public ICollection<string> List() {
        return this.solvers.Keys;
    }

    public SolverFactory AddAssembly<T>(string prefix) {
        foreach (var type in typeof(T).Assembly.GetTypes().Where(t => typeof(ISolver).IsAssignableFrom(t))) {
            if (type.Name.Contains("TemplateSolver")) continue;

            var @event = type.Namespace == null
                ? string.Empty
                : type.Namespace[(type.Namespace.LastIndexOf('.') + 1)..].TrimStart('_');
            
            var solverAttribute = type.GetCustomAttribute<SolverAttribute>();
            solvers[@event.ToLowerInvariant() + "-" + (solverAttribute?.Key ?? type.Name.ToLowerInvariant() + "-" + prefix)] = type;
        }

        return this;
    }
}