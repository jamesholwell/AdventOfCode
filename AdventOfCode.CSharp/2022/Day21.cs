using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day21 : Solver {
    public Day21(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private interface IYellingMonkey {
        string Name { get; }

        long Value { get; }
    }

    private class SpecificNumberYellingMonkey : IYellingMonkey {
        public SpecificNumberYellingMonkey(string name, long value) {
            Value = value;
            Name = name;
        }

        public string Name { get; }

        public long Value { get; }

        public override string ToString() => $"{Name}: {Value}";
    }

    private class MathOperationYellingMonkey : IYellingMonkey {
        public MathOperationYellingMonkey(string name, string leftMonkey, Operators @operator, string rightMonkey) {
            Operator = @operator;
            LeftMonkey = leftMonkey;
            RightMonkey = rightMonkey;
            Name = name;
        }

        public string Name { get; }
        
        public Operators Operator { get; set; }

        public string LeftMonkey { get; init; }
        
        public IYellingMonkey? Left { get; set; }

        public string RightMonkey { get; init; }

        public IYellingMonkey? Right { get; set; }

        public long Value => Left == null || Right == null
            ? throw new InvalidOperationException()
            : Operator switch {
                Operators.Add => Left.Value + Right.Value,
                Operators.Subtract => Left.Value - Right.Value,
                Operators.Multiply => Left.Value * Right.Value,
                Operators.Divide => Left.Value / Right.Value,
                Operators.IsEqual => Left.Value == Right.Value ? 1 : 0,
                _ => throw new ArgumentOutOfRangeException()
            };

        public enum Operators {
            Add,
            Subtract,
            Multiply,
            Divide,
            IsEqual
        }

        public override string ToString() => $"{Name}: {LeftMonkey} {OperatorSymbol} {RightMonkey}";

        private string OperatorSymbol => Operator switch {
            Operators.Add => "+",
            Operators.Subtract => "-",
            Operators.Multiply => "*",
            Operators.Divide => "/",
            Operators.IsEqual => "==",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private class VariableYellingMonkey : IYellingMonkey {
        public VariableYellingMonkey(string name) {
            Name = name;
        }

        public string Name { get; }

        public long Value => throw new InvalidOperationException();

        public override string ToString() => $"{Name}: ???";
    }

    private static IEnumerable<IYellingMonkey> Parse(string input) {
        foreach (var line in Shared.Split(input)) {
            var parts = line.Split(": ");

            if (int.TryParse(parts[1], out var value)) {
                yield return new SpecificNumberYellingMonkey(parts[0], value);
                continue;
            }

            var mathParts = parts[1].Split(' ');

            var @operator = mathParts[1] switch {
                "+" => MathOperationYellingMonkey.Operators.Add,
                "-" => MathOperationYellingMonkey.Operators.Subtract,
                "*" => MathOperationYellingMonkey.Operators.Multiply,
                "/" => MathOperationYellingMonkey.Operators.Divide,
                _ => throw new ArgumentOutOfRangeException()
            };

            yield return new MathOperationYellingMonkey(parts[0], mathParts[0], @operator, mathParts[2]);
        }
    }

    public override long SolvePartOne() => ResolveLinks(Parse(Input))["root"].Value;

    private IDictionary<string, IYellingMonkey> ResolveLinks(IEnumerable<IYellingMonkey> inputMonkeys) {
        var monkeys = inputMonkeys.ToDictionary(m => m.Name, m => m);

        foreach (var monkey in monkeys) 
            Trace.WriteLine(monkey.Value);

        foreach (var monkey in monkeys.Values.Where(m => m is MathOperationYellingMonkey).Cast<MathOperationYellingMonkey>()) {
            if (!monkeys.TryGetValue(monkey.LeftMonkey, out var left) ||
                !monkeys.TryGetValue(monkey.RightMonkey, out var right)) {
                throw new InvalidOperationException();
            }

            monkey.Left = left;
            monkey.Right = right;
        }

        return monkeys;
    }

    public override long SolvePartTwo() {
        var inputMonkeys = Parse(Input).ToDictionary(m => m.Name, m => m);
        var root = (MathOperationYellingMonkey) inputMonkeys["root"];
        root.Operator = MathOperationYellingMonkey.Operators.IsEqual;
        inputMonkeys["humn"] = new VariableYellingMonkey("humn");

        var linkedMonkeys = ResolveLinks(inputMonkeys.Values);
        
        Trace.WriteLine();
        Trace.WriteLine("Left:");
        Trace.WriteLine(root.Left!);
        Trace.WriteLine();
        Trace.WriteLine("Right:");
        Trace.WriteLine(root.Right!);

        return -1;
    }

    private const string? ExampleInput = @"
root: pppw + sjmn
dbpl: 5
cczh: sllz + lgvd
zczc: 2
ptdq: humn - dvpt
dvpt: 3
lfqf: 4
humn: 5
ljgn: 2
sjmn: drzm * dbpl
sllz: 4
pppw: cczh / lfqf
lgvd: ljgn * ptdq
drzm: hmdt - zczc
hmdt: 32
";

    [Fact]
    public void ParsesCorrectly() {
        Assert.Equal(ExampleInput, "\r\n" + string.Join("\r\n", Parse(ExampleInput!).Select(m => m.ToString())) + "\r\n");
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day21(ExampleInput, Output).SolvePartOne();
        Assert.Equal(152, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day21(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(301, actual);
    }
}