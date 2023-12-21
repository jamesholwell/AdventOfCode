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
    
    public static T[,] SplitGrid<T>(this string s, Func<char, T> selector) {
        var rows = Split(s);
        var height = rows.Length;
        var width = rows[0].Length;
        var grid = new T[height, width];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            grid[y, x] = selector(rows[y][x]);

        return grid;
    }

    public static IEnumerable<(int x, int y)> SplitCoordinates(this string s, Func<char, bool> predicate) {
        var rows = Split(s);
        var height = rows.Length;
        var width = rows[0].Length;

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            if (predicate(rows[y][x]))
                yield return (x, y);
    }
    
    public static IEnumerable<(int x, int y)> CoordinatesWhere<T>(this IEnumerable<(int x, int y, T value)> points, Func<T, bool> predicate) {
        return points.Where(p => predicate(p.value)).Select(p => (p.x, p.y));
    }
    
    public static IEnumerable<(int x, int y)> CoordinatesWhere<T>(this IEnumerable<(int x, int y, T value)> points, Func<int, int, T, bool> predicate) {
        return points.Where(p => predicate(p.x, p.y, p.value)).Select(p => (p.x, p.y));
    }

    public static (int x, int y, char value)[] SplitPoints(this string s) => EnumeratePoints(s).ToArray();

    public static (int x, int y, T value)[] SplitPoints<T>(this string s, Func<int, int, char, T> selector) =>
        EnumeratePoints(s, selector).ToArray();

    public static (int x, int y, T value)[] SplitPoints<T>(this string s, Func<char, bool> predicate,
        Func<int, int, char, T> selector) => EnumeratePoints(s, predicate, selector).ToArray();

    public static IEnumerable<(int x, int y, T value)> SplitPoints<T>(this string s,
        Func<int, int, char, bool> predicate, Func<int, int, char, T> selector) =>
        EnumeratePoints(s, predicate, selector).ToArray();

    public static IEnumerable<(int x, int y, char value)> EnumeratePoints(this string s) {
        return EnumeratePoints(s, (_, _, _) => true, (_, _, c) => c);
    }

    public static IEnumerable<(int x, int y, T value)> EnumeratePoints<T>(this string s,
        Func<int, int, char, T> selector) {
        return EnumeratePoints(s, (_, _, _) => true, selector);
    }

    public static IEnumerable<(int x, int y, T value)> EnumeratePoints<T>(this string s, Func<char, bool> predicate,
        Func<int, int, char, T> selector) {
        return EnumeratePoints(s, (_, _, c) => predicate(c), selector);
    }

    public static IEnumerable<(int x, int y, T value)> EnumeratePoints<T>(this string s,
        Func<int, int, char, bool> predicate, Func<int, int, char, T> selector) {
        var rows = Split(s);
        var height = rows.Length;
        var width = rows[0].Length;

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            if (predicate(x, y, rows[y][x]))
                yield return (x, y, selector(x, y, rows[y][x]));
    }
}