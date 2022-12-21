using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day21 : Solver {
    public Day21(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private interface IYellingMonkey {
        string Name { get; }

        bool IsReady { get; }

        long Value { get; }
    }

    private class SpecificNumberYellingMonkey : IYellingMonkey {
        public SpecificNumberYellingMonkey(string name, long value) {
            Value = value;
            Name = name;
        }

        public string Name { get; }

        public bool IsReady => true;

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

        public bool IsReady => LeftValue.HasValue && RightValue.HasValue;

        public Operators Operator { get; init; }

        public string LeftMonkey { get; init; }
        
        public long? LeftValue { get; set; }

        public string RightMonkey { get; init; }

        public long? RightValue { get; set; }

        public long Value => !LeftValue.HasValue || !RightValue.HasValue
            ? throw new InvalidOperationException()
            : Operator switch {
                Operators.Add => LeftValue.Value + RightValue.Value,
                Operators.Subtract => LeftValue.Value - RightValue.Value,
                Operators.Multiply => LeftValue.Value * RightValue.Value,
                Operators.Divide => LeftValue.Value / RightValue.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

        public enum Operators {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public override string ToString() => IsReady 
            ? $"{Name}: {Value} ({LeftMonkey} {OperatorSymbol} {RightMonkey})" 
            : $"{Name}: {LeftMonkey} {OperatorSymbol} {RightMonkey}";

        private string OperatorSymbol => Operator switch {
            Operators.Add => "+",
            Operators.Subtract => "-",
            Operators.Multiply => "*",
            Operators.Divide => "/",
            _ => throw new ArgumentOutOfRangeException()
        };
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

    public override long SolvePartOne() {
        var monkeys = Parse(Input).ToArray();

        foreach (var monkey in monkeys) 
            Trace.WriteLine(monkey);

        var ready = monkeys.Where(m => m.IsReady).ToDictionary(m => m.Name, m => m);
        var unready = monkeys.Except(ready.Values).Cast<MathOperationYellingMonkey>().ToList();

        while (unready.Any()) {
            var stillUnready = new List<MathOperationYellingMonkey>(unready.Count);
            
            foreach (var monkey in unready) {
                if (ready.TryGetValue(monkey.LeftMonkey, out var left) &&
                    ready.TryGetValue(monkey.RightMonkey, out var right)) {
                    monkey.LeftValue = left.Value;
                    monkey.RightValue = right.Value;
                    ready.Add(monkey.Name, monkey);
                }
                else {
                    stillUnready.Add(monkey);
                }
            }

            unready = stillUnready;
        }

        Trace.WriteLine();
        foreach (var monkey in monkeys)
            Trace.WriteLine(monkey);

        return ready["root"].Value;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

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
}