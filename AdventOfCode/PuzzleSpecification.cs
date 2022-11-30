using System.CommandLine.Parsing;

namespace AdventOfCode;

internal struct PuzzleSpecification {
    public string Event { get; set; }

    public string Day { get; set; }

    public bool IsPart2 { get; set; }

    public static ParseArgument<PuzzleSpecification> Parser =>
        result =>
        {
            var spec = new PuzzleSpecification();
            var taken = 0;

            foreach (var token in result.Tokens) {
                var value = token.Value;
                ++taken;

                if (value.StartsWith("-")) break;

                if (value.StartsWith("day"))
                    spec.Day = value;

                else if (value.StartsWith("pt"))
                    spec.IsPart2 = "pt2".Equals(value, StringComparison.OrdinalIgnoreCase);

                else if (value.StartsWith("part"))
                    spec.IsPart2 = "part2".Equals(value, StringComparison.OrdinalIgnoreCase);

                else
                    spec.Event = value;
            }

            result.OnlyTake(taken);
            return spec;
        };
}