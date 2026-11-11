using AdventOfCode.Shared;
using System.Reflection;
using System.Text.RegularExpressions;
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
var isTracing = args.Any(a => string.Equals(a, "--trace", StringComparison.OrdinalIgnoreCase));
var solver = (ISolver)constructor.Invoke([input, isTracing ? new TracingConsoleOutputHelper() : new ConsoleOutputHelper()]);

var output =
    args.Any(a => string.Equals(a, "part2", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "pt2", StringComparison.OrdinalIgnoreCase))
        ? solver.SolvePartTwo()
        : solver.SolvePartOne();

Console.WriteLine(output);

namespace AdventOfCode.Shared {
    #region AdventOfCode.Shared.Input.cs
    
    public static class Input {
        public static string[] SplitBy(this string s, string b) =>
            s.Replace("\r\n", "\n").Trim('\n').Split(b);

        public static string[] Split(this string s) =>
            s.SplitBy("\n");

        public static int[] SplitInt(this string s) => Input.Split(s).Select(int.Parse).ToArray();

        public static int[] SplitInt(this string s, string separator) => s.SplitBy(separator).Select(int.Parse).ToArray();
    }
    
    #endregion
    
    #region AdvendOfCode.Shared.Output.cs
    
    public class NullOutputHelper : ITestOutputHelper {
        public void WriteLine(string value) { }

        public void WriteLine(string format, params object[] args) { }
    }

    public class ConsoleOutputHelper : ITestOutputHelper {
        public void WriteLine(string value) => Console.WriteLine(value);

        public void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);
    }

    public class TracingConsoleOutputHelper : ITestOutputHelper {
        public void WriteLine(string value) => Console.WriteLine(value);

        public void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);
    }

    public static class OutputHelperExtensions {
        public static void WriteHeading(this ITestOutputHelper outputHelper, string heading) {
            outputHelper.WriteLine(string.Empty);
            outputHelper.WriteLine(heading);
        }

        public static void WriteLine(this ITestOutputHelper outputHelper) => outputHelper.WriteLine(string.Empty);

        public static void WriteLine(this ITestOutputHelper outputHelper, int value) =>
            outputHelper.WriteLine(value.ToString());

        public static void WriteLine(this ITestOutputHelper outputHelper, long value) =>
            outputHelper.WriteLine(value.ToString());

        public static void WriteLine(this ITestOutputHelper outputHelper, char[] value) =>
            outputHelper.WriteLine(new string(value));

        public static void WriteLine(this ITestOutputHelper outputHelper, object? value) =>
            outputHelper.WriteLine(value?.ToString() ?? "<null string>");
    }
    
    #endregion
    
    #region AdventOfCode.Shared.Solver.cs
    
    public interface ISolver {
        string SolvePartOne();
        string SolvePartTwo();
    }

    /// <summary>
    ///     Strongly typed base class for solvers returning solutions of type T
    /// </summary>
    /// <typeparam name="T">solution type, often long or string</typeparam>
    public abstract class Solver<T> : ISolver {
        /// <summary>
        ///     Construct the base for puzzle solvers
        /// </summary>
        /// <param name="input">unprocessed contents of the input file</param>
        /// <param name="outputHelper">console-like interface supplied by Xunit / the runner</param>
        protected Solver(string? input = null, ITestOutputHelper? outputHelper = null) {
            Input = input ?? string.Empty;
            Output = outputHelper ?? new NullOutputHelper();
        
            // disable tracing in default cli usage, but keep it enabled for tests
            Trace = outputHelper is ConsoleOutputHelper ? new NullOutputHelper() : Output;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global - used by downstream classes
        protected string Input { get; }

        protected ITestOutputHelper Output { get; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global - used by downstream classes
        protected ITestOutputHelper Trace { get; }

        string ISolver.SolvePartOne() => Convert.ToString(SolvePartOne()) ?? throw new NotImplementedException();

        string ISolver.SolvePartTwo() => Convert.ToString(SolvePartTwo()) ?? throw new NotImplementedException();

        protected abstract T SolvePartOne();

        protected abstract T SolvePartTwo();
    }

    /// <summary>
    ///     Strongly typed base class for solvers returning integer numbers
    /// </summary>
    /// <param name="input">unprocessed contents of the input file</param>
    /// <param name="outputHelper">console-like interface supplied by Xunit / the runner</param>
    public abstract class Solver(string? input, ITestOutputHelper? outputHelper = null)
        : Solver<long>(input, outputHelper);
    
    #endregion
}