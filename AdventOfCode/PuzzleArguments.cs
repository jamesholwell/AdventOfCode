using AdventOfCode.Shared;

namespace AdventOfCode;

public readonly record struct PuzzleArguments {
    public int Event { get; init; }

    public int Day { get; init; }

    public Parts Part { get; init; }
    
    public Languages Language { get; init; }

    public string? Suffix { get; init; }

    public bool IsValid => Day is >= 1 and <= 25 && Event is >= 2015 and < 2099 &&
                           (Suffix == null || Puzzle.SuffixPattern.IsMatch(Suffix));

    /// <remarks>
    ///     There is a kind of 'gotcha' here where the discovery needs to register the
    ///     magic string prefix of "fs" to make the detection work as expected
    /// </remarks>>
    public string? LanguagePrefix => Language switch {
        Languages.FSharp => "fs",
        _ => null
    };

    internal static PuzzleArguments CreateDefault() {
        var now = DateTime.UtcNow.AddHours(-5);

        return new PuzzleArguments {
            Event = now.Month == 12 ? now.Year : now.Year - 1,
            Day = now.Month == 12 ? now.Day : 0,
            Part = Parts.Part1,
            Suffix = null
        };
    }

    public override string ToString() => Suffix == null
        ? $"(Event = {Event}, Day = {Day})"
        : $"(Event = {Event}, Day = {Day} , Suffix = {Suffix})";

    public enum Parts {
        Part1 = 1,
        Part2 = 2
    }

    public enum Languages {
        Default,
        
        CSharp,
        
        FSharp
    }
}