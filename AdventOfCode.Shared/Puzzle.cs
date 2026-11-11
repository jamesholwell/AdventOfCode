using System.Text.RegularExpressions;

namespace AdventOfCode.Shared;

public static partial class Puzzle {
    public static readonly Regex EventPattern = EventRegex();

    public static int GetEvent(Match eventMatch) => int.Parse(eventMatch.Groups[2].Value);
    
    public static readonly Regex DayPattern = DayRegex();

    public static int GetDay(Match dayMatch) => int.Parse(dayMatch.Groups[1].Value);

    public static readonly Regex SuffixPattern = SuffixRegex();
    
    public static string? GetSuffix(Match dayMatch) => dayMatch.Groups[2].Success ? dayMatch.Groups[2].Value : null;

    [GeneratedRegex(@"([a-z]\.)+_?(20[0-9]{2})", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex EventRegex();

    [GeneratedRegex("Day([12]?[0-9])([a-z]+)?", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex DayRegex();

    [GeneratedRegex("^[a-z]*$", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex SuffixRegex();
}