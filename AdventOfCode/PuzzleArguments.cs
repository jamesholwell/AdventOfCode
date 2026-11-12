using AdventOfCode.Shared;

namespace AdventOfCode;

public readonly record struct PuzzleArguments {
    public int Event { get; init; }

    public int Day { get; init; }

    public Parts Part { get; init; }

    public string Suffix { get; init; }

    public bool IsValid => Day is >= 1 and <= 25 && Event is >= 2015 and < 2099 &&
                           (Suffix == null || Puzzle.SuffixPattern.IsMatch(Suffix));

    internal static PuzzleArguments CreateDefault() {
        var now = DateTime.UtcNow.AddHours(-5);

        return new PuzzleArguments {
            Event = now.Month == 12 ? now.Year : now.Year - 1,
            Day = now.Month == 12 ? now.Day : 0,
            Part = Parts.Part1,
            Suffix = string.Empty
        };
    }

    public override string ToString() => Suffix == null
        ? $"(Event = {Event}, Day = {Day})"
        : $"(Event = {Event}, Day = {Day} , Suffix = {Suffix})";

    public enum Parts {
        Part1 = 1,
        Part2 = 2
    }
}