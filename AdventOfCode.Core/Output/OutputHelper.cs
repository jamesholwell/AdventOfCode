using System.Text;
using Xunit.Abstractions;

namespace AdventOfCode.Core.Output;

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

public class StringBuilderOutputHelper : ITestOutputHelper {
    private readonly StringBuilder buffer = new();

    public void WriteLine(string value) => buffer.AppendLine(value);

    public void WriteLine(string format, params object[] args) => buffer.AppendFormat(format, args);

    public override string ToString() => buffer.ToString();
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