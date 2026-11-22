// ReSharper disable MemberCanBePrivate.Global - helpers are used downstream
// ReSharper disable UnusedMember.Global - helpers are used downstream
namespace AdventOfCode.Shared;

public static class Input {
    public static string PathFor(int @event, int day) => $"./inputs/{@event}/day{day}.txt";

    public static bool TryGetContents(int @event, int day, out string? contents) {
        var path = PathFor(@event, day);

        if (!File.Exists(path)) {
            contents = null;
            return false;
        }

        contents = File.ReadAllText(path);
        return true;
    }

    extension(string s) {
        public string Read() =>
            s.Replace("\r\n", "\n").Trim('\n');

        public string[] Chunks() =>
            s.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        public string[] Lines() =>
            s.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        public string[] Words() =>
            s.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        public int[] Ints(string separator = "\n") =>
            [.. s.Split(separator).Select(int.Parse)];

        public long[] Longs(string separator = "\n") =>
            [.. s.Split(separator).Select(long.Parse)];
    }

    extension(string[] a) {
        public int[] Ints() =>
            [.. a.Select(int.Parse)];

        public long[] Longs() =>
            [.. a.Select(long.Parse)];
    }
}