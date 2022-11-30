namespace AdventOfCode.CSharp;

public static class Shared {
    public static string[] SplitString(this string s) =>
        s.Replace("\r\n", "\n")
            .Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();

    public static int[] SplitInt(this string s) =>
        s.Replace("\r\n", "\n")
            .Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse).ToArray();
}