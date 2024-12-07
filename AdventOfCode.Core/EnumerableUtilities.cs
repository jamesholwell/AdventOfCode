// ReSharper disable UnusedMember.Global - helpers are used downstream
// ReSharper disable UnusedType.Global - helpers are used downstream
namespace AdventOfCode.Core;

public static class EnumerableUtilities {
    public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> enumerable) {
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext()) yield break;

        var previous = e.Current;
        while (e.MoveNext())
            yield return Tuple.Create(previous, previous = e.Current);
    }

    public static IEnumerable<(T left, T right)> Combinations<T>(this IEnumerable<T> enumerable) {
        var array = enumerable as T[] ?? enumerable.ToArray();

        if (array.Length < 2) 
            return Array.Empty<(T, T)>();

        var ret = new (T, T)[(array.Length * (array.Length - 1)) / 2];

        var o = 0;
        for (var i = 0; i < array.Length - 1; ++i)
        for (var j = i + 1; j < array.Length; ++j)
            ret[o++] = (array[i], array[j]);

        return ret;
    }
}