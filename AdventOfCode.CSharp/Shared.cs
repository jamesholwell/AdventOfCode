namespace AdventOfCode.CSharp;

public static class Shared {
    public static string[] SplitBy(this string s, string b) =>
        s.Replace("\r\n", "\n").Trim('\n').Split(b);

    public static string[] Split(this string s) =>
        s.Split("\n");

    public static int[] SplitInt(this string s) => s.Split().Select(int.Parse).ToArray();
}