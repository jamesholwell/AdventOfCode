using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day7 : Solver  {
    public Day7(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var tuples = Shared.Split(Input).Select(s => ParseHandBid(s));

        return tuples
            .OrderBy(hb => (hb.hand, hb.type), new HandComparer(Strength))
            .Select((hb, i) => (hb.hand, score: (1 + i) * hb.bid))
            .Sum(hs => hs.score);
    }
    
    public override long SolvePartTwo() {
        var tuples = Shared.Split(Input).Select(s => ParseHandBid(s, true));

        return tuples
            .OrderBy(hb => (hb.hand, hb.type), new HandComparer(WildcardStrength))
            .Select((hb, i) => (hb.hand, score: (1 + i) * hb.bid))
            .Sum(hs => hs.score);
    }

    (string hand, int bid, int type) ParseHandBid(string s, bool isJackWildcard = false) {
        var parts = s.Split(' ', 2, StringSplitOptions.TrimEntries);
        var hand = parts[0];
        var bid = int.Parse(parts[1]);
        var type = HandTypeFor(hand);

        if (isJackWildcard && hand.Contains('J')) {
            var originalType = type;
            var newHand = ApplyWildcard(hand);
            type = HandTypeFor(newHand);
            Debug.Assert(originalType <= type);
            
            Output.WriteLine($"{hand} ({originalType}) -> {newHand} ({type})");
        }

        return (hand, bid, type);
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

    private string ApplyWildcard(string hand) {
        var handArray = hand.ToCharArray();
        var strongestNotJack = handArray.Where(c => c != 'J')
            .GroupBy(c => c, c => c)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => Strength(g.Key))
            .FirstOrDefault();
        
        if (strongestNotJack != default)
            return hand.Replace('J', strongestNotJack.Key);

        return hand;
    }

    class HandComparer : IComparer<(string hand, int type)> {
        private readonly Func<char, int> strengthFunction;

        public HandComparer(Func<char, int> strengthFunction) {
            this.strengthFunction = strengthFunction;
        }

        public int Compare((string hand, int type) x, (string hand, int type) y) {
            if (x.type < y.type) return -1;
            if (x.type > y.type) return 1;

            for (var i = 0; i < 5; ++i) {
                var xs = strengthFunction(x.hand[i]);
                var ys = strengthFunction(y.hand[i]);
                
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
    
    static int WildcardStrength(char card) {
        return card switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 1,
            'T' => 10,
            _ => card - '0'
        };
    }

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
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day7(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(5905, actual);
    }
}