// ReSharper disable MemberCanBePrivate.Global - helpers are used downstream
// ReSharper disable UnusedMember.Global - helpers are used downstream
namespace AdventOfCode.Core;

public static class Shared {
    public static string[] SplitBy(this string s, string b) =>
        s.Replace("\r\n", "\n").Trim('\n').Split(b);

    public static string[] Split(this string s) =>
        s.SplitBy("\n");

    public static int[] SplitInt(this string s) => Shared.Split(s).Select(int.Parse).ToArray();

}