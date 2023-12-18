namespace AdventOfCode.CSharp;

public static class EnumerableUtilities {
    public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> enumerable) {
        using var e = enumerable.GetEnumerator();
        if (!e.MoveNext()) yield break;

        var previous = e.Current;
        while (e.MoveNext())
            yield return Tuple.Create(previous, previous = e.Current);
    }
}