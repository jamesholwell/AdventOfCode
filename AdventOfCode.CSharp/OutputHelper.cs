using Xunit.Abstractions;

namespace AdventOfCode.CSharp;

public class ConsoleOutputHelper : ITestOutputHelper {
    public void WriteLine(string value) => Console.WriteLine(value);

    public void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);
}

public class NullOutputHelper : ITestOutputHelper {
    public void WriteLine(string value) { return; }

    public void WriteLine(string format, params object[] args) { return; }
}

public static class OutputHelperExtensions {
    public static void WriteLine(this ITestOutputHelper outputHelper) => outputHelper.WriteLine(string.Empty);

    public static void WriteLine(this ITestOutputHelper outputHelper, int value) =>
        outputHelper.WriteLine(value.ToString());

    public static void WriteLine(this ITestOutputHelper outputHelper, long value) =>
        outputHelper.WriteLine(value.ToString());

    public static void WriteLine(this ITestOutputHelper outputHelper, char[] value) =>
        outputHelper.WriteLine(new string(value));

    public static void WriteLine(this ITestOutputHelper outputHelper, object value) =>
        outputHelper.WriteLine(value.ToString());
}