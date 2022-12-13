using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day13(string? input = null, ITestOutputHelper? outputHelper = null) : Solver(input, outputHelper) {
    private string[][] Parse(string input) => input.SplitBy("\n\n").Select(s => s.Split("\n")).ToArray();

    protected override long SolvePartOne() =>
        Parse(Input).Select((x, i) => IsOrdered(x[0], x[1]) == true ? 1 + i : 0).Sum();

    protected override long SolvePartTwo() {
        var originalPackets = Shared.Split(Input).Where(s => !string.IsNullOrWhiteSpace(s)).Select(Tokenise).ToArray();
        var divider1 = Tokenise("[[2]]");
        var divider2 = Tokenise("[[6]]");
        var packets = originalPackets.Concat(new[] { divider1, divider2 }).ToArray();

        Array.Sort(packets, Compare);

        return (1 + Array.IndexOf(packets, divider1)) * (1 + Array.IndexOf(packets, divider2));
    }

    private int Compare(Token x, Token y) {
        return IsOrdered(x, y) switch {
            true => -1,
            false => 1,
            null => 0
        };
    }

    private bool? IsOrdered(string left, string right) => IsOrdered(Tokenise(left), Tokenise(right));

    private bool? IsOrdered(Token left, Token right) {
        if (left is IntegerToken leftInteger && right is IntegerToken rightInteger) {
            if (leftInteger.Value < rightInteger.Value) return true;
            if (leftInteger.Value > rightInteger.Value) return false;

            return null;
        }

        if (left is ListToken leftList && right is ListToken rightList) {
            using var leftEnumerator = leftList.Members.GetEnumerator();
            using var rightEnumerator = rightList.Members.GetEnumerator();

            while (leftEnumerator.MoveNext()) {
                if (!rightEnumerator.MoveNext()) return false;

                var inner = IsOrdered(leftEnumerator.Current, rightEnumerator.Current);

                if (inner.HasValue) return inner.Value;
            }

            if (rightEnumerator.MoveNext()) return true;

            return null;
        }

        return left is IntegerToken
            ? IsOrdered(new ListToken(left), right)
            : IsOrdered(left, new ListToken(right));
    }

    private class Token { }

    private class IntegerToken(IEnumerable<char> buffer) : Token {
        public int Value = int.Parse(new string(buffer.ToArray()));
    }

    private class ListToken : Token {
        public List<Token> Members = new List<Token>();

        public ListToken() { }

        public ListToken(Token token) {
            Members.Add(token);
        }
    }

    private Token Tokenise(string s) {
        var stack = new Stack<ListToken>();
        var buffer = new List<char>();

        foreach (var c in s) {
            switch (c) {
                case '[':
                    stack.Push(new ListToken());
                    break;

                case ']':
                    if (buffer.Any()) {
                        stack.Peek().Members.Add(new IntegerToken(buffer));
                        buffer.Clear();
                    }

                    var p = stack.Pop();
                    if (stack.Count == 0) return p;
                    else stack.Peek().Members.Add(p);

                    break;

                case ',':
                    if (buffer.Any()) {
                        stack.Peek().Members.Add(new IntegerToken(buffer));
                        buffer.Clear();
                    }

                    break;

                default:
                    buffer.Add(c);
                    break;
            }
        }

        throw new InvalidOperationException("Fell off bottom of stack");
    }

    private const string? ExampleInput = @"
[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]
";

    [Fact]
    public void FirstExampleLeftSideIsTokenisedCorrectly() {
        var actual = Tokenise("[1,1,3,1,1]");
        Assert.NotNull(actual);
        Assert.IsType<ListToken>(actual);
        var list = (ListToken)actual;
        Assert.Equal(5, list.Members.Count);
        var members = list.Members.ToArray();
        Assert.IsType<IntegerToken>(members[0]);
        Assert.Equal(1, ((IntegerToken)members[0]).Value);

        Assert.IsType<IntegerToken>(members[1]);
        Assert.Equal(1, ((IntegerToken)members[1]).Value);

        Assert.IsType<IntegerToken>(members[2]);
        Assert.Equal(3, ((IntegerToken)members[2]).Value);

        Assert.IsType<IntegerToken>(members[3]);
        Assert.Equal(1, ((IntegerToken)members[3]).Value);

        Assert.IsType<IntegerToken>(members[4]);
        Assert.Equal(1, ((IntegerToken)members[4]).Value);
    }


    [Fact]
    public void SecondExampleRightSideIsTokenisedCorrectly() {
        var actual = Tokenise("[[1],4]");
        Assert.NotNull(actual);
        Assert.IsType<ListToken>(actual);
        var list = (ListToken)actual;
        Assert.Equal(2, list.Members.Count);
        var members = list.Members.ToArray();

        Assert.IsType<ListToken>(members[0]);
        var innerListMembers = ((ListToken)members[0]).Members;
        Assert.Single(innerListMembers);

        Assert.IsType<IntegerToken>(innerListMembers[0]);
        Assert.Equal(1, ((IntegerToken)innerListMembers[0]).Value);

        Assert.IsType<IntegerToken>(members[1]);
        Assert.Equal(4, ((IntegerToken)members[1]).Value);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day13(ExampleInput, Output).SolvePartOne();
        Assert.Equal(13, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day13(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(140, actual);
    }
}