// ReSharper disable MemberCanBePrivate.Global - helpers are used downstream
// ReSharper disable UnusedMember.Global - helpers are used downstream
// ReSharper disable UnusedType.Global - helpers are used downstream

namespace AdventOfCode.Core.Points;

public static class PointHelpers {
    public static (int x, int y) Up(this (int x, int y) coordinate) => (coordinate.x, coordinate.y - 1);

    public static (int x, int y) Right(this (int x, int y) coordinate) => (coordinate.x + 1, coordinate.y);

    public static (int x, int y) Down(this (int x, int y) coordinate) => (coordinate.x, coordinate.y + 1);

    public static (int x, int y) Left(this (int x, int y) coordinate) => (coordinate.x - 1, coordinate.y);

    public static IEnumerable<(int x, int y)> SplitCoordinates(this string s, Func<char, bool> predicate) {
        var rows = Shared.Split(s);
        var height = rows.Length;
        var width = rows[0].Length;

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            if (predicate(rows[y][x]))
                yield return (x, y);
    }

    public static IEnumerable<(int x, int y)> CoordinatesWhere<T>(this IEnumerable<(int x, int y, T value)> points,
        Func<T, bool> predicate) {
        return points.Where(p => predicate(p.value)).Select(p => (p.x, p.y));
    }

    public static IEnumerable<(int x, int y)> CoordinatesWhere<T>(this IEnumerable<(int x, int y, T value)> points,
        Func<int, int, T, bool> predicate) {
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
        var rows = Shared.Split(s);
        var height = rows.Length;
        var width = rows[0].Length;

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            if (predicate(x, y, rows[y][x]))
                yield return (x, y, selector(x, y, rows[y][x]));
    }
}