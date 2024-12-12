// ReSharper disable MemberCanBePrivate.Global - helpers are used downstream
// ReSharper disable UnusedMember.Global - helpers are used downstream
using System.Text;

namespace AdventOfCode.Core.Grid;

public static class GridHelpers {
    public static char[,] SplitGrid(this string s) {
        var rows = Shared.Split(s);
        var height = rows.Length;
        var width = rows[0].Length;
        var grid = new char[height, width];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            grid[y, x] = rows[y][x];

        return grid;
    }
    
    public static T[,] SplitGrid<T>(this string s, Func<char, T> selector) {
        var rows = Shared.Split(s);
        var height = rows.Length;
        var width = rows[0].Length;
        var grid = new T[height, width];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            grid[y, x] = selector(rows[y][x]);

        return grid;
    }
    
    public static int Height<T>(this T[,] array) => array.GetLength(0);

    public static int Width<T>(this T[,] array) => array.GetLength(1);

    public static T At<T>(this T[,] array, (int x, int y) index) => array[index.y, index.x];

    public static T At<T>(this T[,] array, int x, int y) => array[y, x];
    
    public static T? MaybeAt<T>(this T[,] array, (int x, int y) index) => 
        index.y < 0 || index.y >= array.GetLength(0) ||
        index.x < 0 || index.x >= array.GetLength(1) ? default : 
        array[index.y, index.x];

    public static T? MaybeAt<T>(this T[,] array, int x, int y) => 
        y < 0 || y >= array.GetLength(0) ||
        x < 0 || x >= array.GetLength(1) ? default : 
            array[y, x];

    public static bool Contains<T>(this T[,] array, (int x, int y) index) => 
        0 <= index.y && index.y < array.Height() &&
        0 <= index.x && index.x < array.Width();

    public static bool Contains<T>(this T[,] array, int x, int y) => 
        0 <= y && y < array.Height() &&
        0 <= x && x < array.Width();

    public static void Initialize<T>(this T[,] array, T value) {
        var width = array.Width();
        var height = array.Height();

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            array[y, x] = value;
    }
    
    public static T[,] Duplicate<T>(this T[,] array) {
        var copy = new T[array.GetLength(0), array.GetLength(1)];
        Array.Copy(array, copy, array.Length);
        return copy;
    }
    
    public static string Render(this char[,] array) {
        var width = array.Width();
        var height = array.Height();
        var buffer = new StringBuilder((width + 2) * height + 2);

        for (var y = 0; y < height; ++y) {
            if (y > 0)
                buffer.Append(Environment.NewLine);

            for (var x = 0; x < width; ++x) buffer.Append(array[y, x]);
        }

        return buffer.ToString();
    }

    public static string Render<T>(this T[,] array, Func<T, char> selector) {
        return Render(array, (_, _, t) => selector(t));
    }

    public static string Render<T>(this T[,] array, Func<int, int, T, char> selector) {
        var width = array.Width();
        var height = array.Height();
        var buffer = new StringBuilder((width + 2) * height + 2);

        for (var y = 0; y < height; ++y) {
            if (y > 0)
                buffer.Append(Environment.NewLine);

            for (var x = 0; x < width; ++x) buffer.Append(selector(x, y, array[y, x]));
        }

        return buffer.ToString();
    }

    public static TResult[,] Map<T, TResult>(this T[,] array, Func<T, TResult> selector) {
        return Map(array, (_, _, t) => selector(t));
    }

    public static TResult[,] Map<T, TResult>(this T[,] array, Func<int, int, T, TResult> selector) {
        var width = array.Width();
        var height = array.Height();
        var result = new TResult[height, width];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            result[y, x] = selector(x, y, array[y, x]);

        return result;
    }
    
    public static TResult[] Flatten<T, TResult>(this T[,] array, Func<T, TResult> selector) {
        return Flatten(array, (_, _, t) => selector(t));
    }

    public static TResult[] Flatten<T, TResult>(this T[,] array, Func<int, int, T, TResult> selector) {
        var width = array.Width();
        var height = array.Height();
        var accumulator = new TResult[width * height];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            accumulator[x + width * y] = selector(x, y, array[y, x]);

        return accumulator;
    }

    public static long Sum<T>(this T[,] array, Func<T, long> selector) {
        return Flatten(array, selector).Sum();
    }

    public static long Sum<T>(this T[,] array, Func<int, int, T, long> selector) {
        return Flatten(array, selector).Sum();
    }

    public static long Count(this bool[,] array) {
        return Flatten(array, (_, _, b) => b ? 1 : 0).Sum();
    }

    public static long Count<T>(this T[,] array, Func<T, bool> predicate) {
        return Flatten(array, predicate).Count(b => b);
    }

    public static long Count<T>(this T[,] array, Func<int, int, T, bool> predicate) {
        return Flatten(array, predicate).Count(b => b);
    }
}