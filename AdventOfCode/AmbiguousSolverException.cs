using System.Collections.ObjectModel;

namespace AdventOfCode;

public class AmbiguousSolverException(IEnumerable<string> candidates) : Exception("Multiple solvers found") {
    public ReadOnlyCollection<string> Candidates { get; } = new([.. candidates]);
}