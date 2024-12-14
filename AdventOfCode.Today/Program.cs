using System.Reflection;
using System.Text.RegularExpressions;
using AdventOfCode.Core;
using AdventOfCode.Core.Output;
using Xunit.Abstractions;

// locate the solver class
var solvers =
    Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(t => typeof(ISolver).IsAssignableFrom(t) && !t.IsAbstract)
        .ToArray();

if (solvers.Length != 1) {
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine(solvers.Length == 0 ? "No solver detected" : "More than one solver detected");
    foreach (var candidate in solvers)
        Console.WriteLine(candidate.FullName);
    Console.ResetColor();
    return;
}

var type = solvers[0];
var constructor = type.GetConstructor(new[] { typeof(string), typeof(ITestOutputHelper) });

if (constructor is null) {
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("Solver has incompatible constructor");
    Console.ResetColor();
    return;
}

// determine the puzzle specification
var @event = type.Namespace == null
    ? string.Empty
    : type.Namespace[(type.Namespace.LastIndexOf('.') + 1)..].TrimStart('_');

var day = type.Name;

// read the puzzle input
var sanitize = new Regex("[^a-z0-9]", RegexOptions.IgnoreCase);
var path = $"./inputs/{sanitize.Replace(@event, string.Empty)}/{sanitize.Replace(day, string.Empty)}.txt";

if (!File.Exists(path)) {
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("Input file does not exist");
    Console.ResetColor();
    return;
}

var input = File.ReadAllText(path);

// instantiate the solver
var solver = (ISolver)constructor.Invoke([input, new ConsoleOutputHelper()]);

var output =
    args.Any(a => string.Equals(a, "pt2", StringComparison.OrdinalIgnoreCase))
        ? solver.SolvePartTwo()
        : solver.SolvePartOne();

Console.WriteLine(output);