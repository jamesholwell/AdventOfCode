using AdventOfCode.Core;
using AdventOfCode.Core.Output;
using Xunit;
using Xunit.Abstractions;
using InvalidOperationException = System.InvalidOperationException;

namespace AdventOfCode.CSharp._2022;

public class Day21(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver(input, outputHelper) {
    private interface IYellingMonkey {
        string Name { get; }

        long Value { get; }
    }

    private class SpecificNumberYellingMonkey(string name, long value) : IYellingMonkey {
        public string Name { get; } = name;

        public long Value { get; } = value;

        public override string ToString() => $"{Name}: {Value}";
    }

    private class MathOperationYellingMonkey(
        string name,
        string leftMonkey,
        MathOperationYellingMonkey.Operators @operator,
        string rightMonkey)
        : IYellingMonkey {
        public string Name { get; } = name;

        public Operators Operator { get; set; } = @operator;

        public string LeftMonkey { get; init; } = leftMonkey;

        public IYellingMonkey? Left { get; set; }

        public string RightMonkey { get; init; } = rightMonkey;

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

    private class VariableYellingMonkey(string name) : IYellingMonkey {
        public string Name { get; } = name;

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

    protected override long SolvePartOne() => ResolveLinks(Parse(Input))["root"].Value;

    private IDictionary<string, IYellingMonkey> ResolveLinks(IEnumerable<IYellingMonkey> inputMonkeys) {
        var monkeys = inputMonkeys.ToDictionary(m => m.Name, m => m);

        foreach (var monkey in monkeys)
            Trace.WriteLine(monkey.Value);

        foreach (var monkey in monkeys.Values.Where(m => m is MathOperationYellingMonkey)
                     .Cast<MathOperationYellingMonkey>()) {
            if (!monkeys.TryGetValue(monkey.LeftMonkey, out var left) ||
                !monkeys.TryGetValue(monkey.RightMonkey, out var right)) {
                throw new InvalidOperationException();
            }

            monkey.Left = left;
            monkey.Right = right;
        }

        return monkeys;
    }

    protected override long SolvePartTwo() {
        var inputMonkeys = Parse(Input).ToDictionary(m => m.Name, m => m);
        var root = (MathOperationYellingMonkey)inputMonkeys["root"];
        root.Operator = MathOperationYellingMonkey.Operators.IsEqual;
        // ReSharper disable StringLiteralTypo
        inputMonkeys["humn"] = new VariableYellingMonkey("humn");
        // ReSharper restore StringLiteralTypo

        ResolveLinks(inputMonkeys.Values);

        Trace.WriteLine();
        Trace.WriteLine("Left:");
        var leftEvaluated = Evaluate(root.Left ?? throw new InvalidOperationException());
        Trace.WriteLine(leftEvaluated);

        Trace.WriteLine();
        Trace.WriteLine("Right:");
        var rightEvaluated = Evaluate(root.Right ?? throw new InvalidOperationException());
        Trace.WriteLine(rightEvaluated);

        var isLeftResult = leftEvaluated is SpecificNumberYellingMonkey;
        var result = isLeftResult ? leftEvaluated.Value : rightEvaluated.Value;
        var operation = isLeftResult ? rightEvaluated : leftEvaluated;

        while (true) {
            if (operation is VariableYellingMonkey)
                return result;

            if (operation is MathOperationYellingMonkey mom) {
                if (mom.Left is SpecificNumberYellingMonkey) {
                    switch (mom.Operator) {
                        // result = x + ?
                        case MathOperationYellingMonkey.Operators.Add:
                            result -= mom.Left.Value;
                            break;

                        // result = x - ?
                        case MathOperationYellingMonkey.Operators.Subtract:
                            result -= mom.Left.Value;
                            result *= -1L;
                            break;

                        // result = x * ?
                        case MathOperationYellingMonkey.Operators.Multiply:
                            result /= mom.Left.Value;
                            break;

                        // result = x / ? -> result/x = 1/? -> ? = x/result
                        case MathOperationYellingMonkey.Operators.Divide:
                            result = mom.Left.Value / result;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    operation = mom.Right;
                    continue;
                }

                if (mom.Right is SpecificNumberYellingMonkey) {
                    switch (mom.Operator) {
                        // result = ? + x
                        case MathOperationYellingMonkey.Operators.Add:
                            result -= mom.Right.Value;
                            break;

                        // result = ? - x
                        case MathOperationYellingMonkey.Operators.Subtract:
                            result += mom.Right.Value;
                            break;

                        // result = ? * x
                        case MathOperationYellingMonkey.Operators.Multiply:
                            result /= mom.Right.Value;
                            break;

                        // result = ? / x -> result/x = 1/? -> ? = x/result
                        case MathOperationYellingMonkey.Operators.Divide:
                            result *= mom.Right.Value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    operation = mom.Left;
                    continue;
                }
            }

            throw new InvalidOperationException();
        }
    }

    private IYellingMonkey Evaluate(IYellingMonkey monkey) {
        switch (monkey) {
            case SpecificNumberYellingMonkey svm:
                return svm;

            case VariableYellingMonkey vvm:
                return vvm;

            case MathOperationYellingMonkey mom:
                var left = Evaluate(mom.Left ?? throw new InvalidOperationException());
                var right = Evaluate(mom.Right ?? throw new InvalidOperationException());

                if (left is SpecificNumberYellingMonkey && right is SpecificNumberYellingMonkey)
                    return mom.Operator switch {
                        MathOperationYellingMonkey.Operators.Add => new SpecificNumberYellingMonkey(string.Empty,
                            left.Value + right.Value),
                        MathOperationYellingMonkey.Operators.Subtract => new SpecificNumberYellingMonkey(string.Empty,
                            left.Value - right.Value),
                        MathOperationYellingMonkey.Operators.Multiply => new SpecificNumberYellingMonkey(string.Empty,
                            left.Value * right.Value),
                        MathOperationYellingMonkey.Operators.Divide => new SpecificNumberYellingMonkey(string.Empty,
                            left.Value / right.Value),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                return new MathOperationYellingMonkey(string.Empty, left.ToString() ?? string.Empty, mom.Operator,
                    right.ToString() ?? string.Empty) { Left = left, Right = right };
            default:
                throw new InvalidOperationException();
        }
    }

    private const string ExampleInput = @"
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
        Assert.Equal(ExampleInput,
            "\r\n" + string.Join("\r\n", Parse(ExampleInput).Select(m => m.ToString())) + "\r\n");
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