// ReSharper disable MemberCanBePrivate.Global - helpers are used downstream
// ReSharper disable UnusedMember.Global - helpers are used downstream
namespace AdventOfCode.Shared;

public static class Input {
    public static string PathFor(int @event, int day) => $"./inputs/{@event}/day{day}.txt";

    public static bool TryRead(int @event, int day, out string? contents) {
        var path = PathFor(@event, day);
            
        if (!File.Exists(path)) {
            contents = null;
            return false;
        }

        contents = File.ReadAllText(path);
        return true;
    }
    
    public static string[] SplitBy(this string s, string b) =>
        s.Replace("\r\n", "\n").Trim('\n').Split(b);

    public static string[] Split(this string s) =>
        s.SplitBy("\n");

    public static int[] SplitInt(this string s) => Input.Split(s).Select(int.Parse).ToArray();

    public static int[] SplitInt(this string s, string separator) => s.SplitBy(separator).Select(int.Parse).ToArray();
}