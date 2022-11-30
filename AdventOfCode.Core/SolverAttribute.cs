namespace AdventOfCode.Core;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SolverAttribute : Attribute {
    public SolverAttribute(string day, string name) {
        Key = $"{day.ToLowerInvariant()}-{name.ToLowerInvariant()}";
    }

    public string Key { get; }
}