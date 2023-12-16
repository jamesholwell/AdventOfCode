namespace AdventOfCode.CSharp;

public static class Shared {
    public static string[] SplitBy(this string s, string b) =>
        s.Replace("\r\n", "\n").Trim('\n').Split(b);

    public static string[] Split(this string s) =>
        s.SplitBy("\n");

    public static int[] SplitInt(this string s) => Shared.Split(s).Select(int.Parse).ToArray();

    public static char[,] SplitGrid(this string s) {
        var rows = Split(s);
        var height = rows.Length;
        var width = rows[0].Length;
        var grid = new char[height, width];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            grid[y, x] = rows[y][x];

        return grid;
    }
}