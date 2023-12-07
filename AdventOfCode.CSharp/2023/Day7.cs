using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day7 : Solver  {
    public Day7(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var tuples = Shared.Split(Input).Select(ParseHandBid);

        return tuples
            .OrderBy(hb => (hb.hand, hb.type), new HandComparer())
            .Select((hb, i) => (hb.hand, score: (1 + i) * hb.bid))
            .Sum(hs => hs.score);
    }

    (string hand, int bid, int type) ParseHandBid(string s) {
        var parts = s.Split(' ', 2, StringSplitOptions.TrimEntries);
        return (parts[0], int.Parse(parts[1]), HandTypeFor(parts[0]));
    }

    int HandTypeFor(string hand) {
        var groups = hand.ToCharArray().ToLookup(c => c, c => c);
        var hasTriple = groups.Any(g => g.Count() == 3);

        switch (groups.Count) {
            case 5: // high card
                return 0;

            case 4: // one pair
                return 1;

            case 3 when hasTriple: // three of a kind
                return 3;

            case 3: // if not, must be two pair
                return 2;

            case 2 when hasTriple: // full house
                return 4;

            case 2: // four of a kind
                return 5;

            case 1: // five of a kind
                return 6;

            default:
                throw new InvalidOperationException($"Could not get type for {hand}");
        }
    }

    class HandComparer : IComparer<(string hand, int type)> {
        public int Compare((string hand, int type) x, (string hand, int type) y) {
            if (x.type < y.type) return -1;
            if (x.type > y.type) return 1;

            for (var i = 0; i < 5; ++i) {
                var xs = Strength(x.hand[i]);
                var ys = Strength(y.hand[i]);
                
                if (xs < ys) return -1;
                if (xs > ys) return 1;
            }

            return 0;
        }
    }

    static int Strength(char card) {
        return card switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            _ => card - '0'
        };
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day7(ExampleInput, Output).SolvePartOne();
        Assert.Equal(6440, actual);
    }
}