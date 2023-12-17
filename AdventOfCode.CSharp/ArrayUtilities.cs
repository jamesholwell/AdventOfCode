using System.Text;

namespace AdventOfCode.CSharp;

public static class ArrayUtilities {
    public static int Height<T>(this T[,] array) => array.GetLength(0);

    public static int Width<T>(this T[,] array) => array.GetLength(1);

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