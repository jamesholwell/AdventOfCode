using System.Text;

namespace AdventOfCode.CSharp;

public static class ArrayUtilities {
    public static int Height<T>(this T[,] array) => array.GetLength(0);

    public static int Width<T>(this T[,] array) => array.GetLength(1);

    public static string Render(this char[,] array) {
        var width = array.GetLength(0);
        var height = array.GetLength(1);
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
        var width = array.GetLength(0);
        var height = array.GetLength(1);
        var buffer = new StringBuilder((width + 2) * height + 2);

        for (var y = 0; y < height; ++y) {
            if (y > 0)
                buffer.Append(Environment.NewLine);

            for (var x = 0; x < width; ++x) buffer.Append(selector(x, y, array[y, x]));
        }

        return buffer.ToString();
    }

    public static TResult[] Map<T, TResult>(this T[,] array, Func<T, TResult> selector) {
        return Map(array, (_, _, t) => selector(t));
    }

    public static TResult[] Map<T, TResult>(this T[,] array, Func<int, int, T, TResult> selector) {
        var width = array.GetLength(0);
        var height = array.GetLength(1);
        var accumulator = new TResult[width * height];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            accumulator[x + width * y] = selector(x, y, array[y, x]);

        return accumulator;
    }

    public static long Sum<T>(this T[,] array, Func<T, long> selector) {
        return Map(array, selector).Sum();
    }

    public static long Sum<T>(this T[,] array, Func<int, int, T, long> selector) {
        return Map(array, selector).Sum();
    }

    public static long Count(this bool[,] array) {
        return Map(array, (_, _, b) => b ? 1 : 0).Sum();
    }

    public static long Count<T>(this T[,] array, Func<T, bool> predicate) {
        return Map(array, predicate).Count(b => b);
    }

    public static long Count<T>(this T[,] array, Func<int, int, T, bool> predicate) {
        return Map(array, predicate).Count(b => b);
    }
}