using Xunit;

namespace AdventOfCode.Shared;

public class NullOutputHelper : ITestOutputHelper {
    public void Write(string message) { }
    
    public void Write(string format, params object[] args) { }
    
    public void WriteLine(string value) { }

    public void WriteLine(string format, params object[] args) { }
    
    public string Output => throw new NotImplementedException();
}

public class ConsoleOutputHelper : ITestOutputHelper {
    public void Write(string message) => Console.Write(message);

    public void Write(string format, params object[] args) => Console.Write(format, args);
    
    public void WriteLine(string value) => Console.WriteLine(value);

    public void WriteLine(string format, params object[] args) => Console.WriteLine(format, args);
    
    public string Output => throw new NotImplementedException();
}

public class TracingConsoleOutputHelper : ConsoleOutputHelper { }

public static class OutputHelperExtensions {
    extension(ITestOutputHelper outputHelper) {
        public void WriteHeading(string heading) {
            outputHelper.WriteLine(string.Empty);
            outputHelper.WriteLine(heading);
        }

        public void WriteLine() => outputHelper.WriteLine(string.Empty);

        public void WriteLine(int value) =>
            outputHelper.WriteLine(value.ToString());

        public void WriteLine(long value) =>
            outputHelper.WriteLine(value.ToString());

        public void WriteLine(char[] value) =>
            outputHelper.WriteLine(new string(value));

        public void WriteLine(object? value) =>
            outputHelper.WriteLine(value?.ToString() ?? "<null string>");
    }
}