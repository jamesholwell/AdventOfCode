using System.Collections.ObjectModel;

namespace AdventOfCode.Infrastructure;

public class AmbiguousSolverException(string[] candidates) : Exception("Multiple solvers found") {
    public ReadOnlyCollection<string> Candidates { get; } = new(candidates);
}