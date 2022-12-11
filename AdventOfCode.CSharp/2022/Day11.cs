using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day11(string? input = null, ITestOutputHelper? outputHelper = null) : Solver(input, outputHelper) {
    private IEnumerable<Monkey> Parse(string input) {
        var monkeySpecs = input.SplitBy("\n\n");

        foreach (var monkeySpec in monkeySpecs) {
            var parts = monkeySpec.Split("\n", StringSplitOptions.TrimEntries);

            yield return new Monkey(parts[1]["Starting items: ".Length..].SplitInt(", ")) {
                OperationAdd = parts[2].Contains("+")
                    ? int.Parse(parts[2]["Operation: new = old + ".Length..])
                    : null,
                OperationMul = 
                    parts[2].Contains("* old") ? null :
                    parts[2].Contains("*") ? int.Parse(parts[2]["Operation: new = old * ".Length..])
                    : null,
                Divisor = int.Parse(parts[3]["Test: divisible by ".Length..]),
                DestinationIfDivisible = int.Parse(parts[4]["If true: throw to monkey ".Length..]),
                DestinationIfNot = int.Parse(parts[5]["If false: throw to monkey ".Length..])
            };
        }
    }
    
    protected override long SolvePartOne() => Solve(Parse(Input).ToArray(), 20, divisor: 3);

    protected override long SolvePartTwo() {
        var monkeys = Parse(Input).ToArray();
        var modulus = monkeys.Aggregate(1, (acc, charge) => acc * charge.Divisor);

        return Solve(monkeys, 10000, modulus: modulus);
    }

    private long Solve(Monkey[] monkeys, int turns, int divisor = 1, int modulus = int.MaxValue) {
        for (var i = 0; i < turns; ++i)
            foreach (var monkey in monkeys)
                monkey.TakeTurn(monkeys, divisor, modulus);


        return monkeys.OrderByDescending(m => m.Inspections).Take(2)
            .Aggregate(1L, (acc, charge) => checked(acc * charge.Inspections));
    }

    private const string ExampleInput = @"
Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1
";

    private IEnumerable<Monkey> GetExampleMonkeys() {
        yield return new Monkey(79, 98) {
            OperationMul = 19,
            Divisor = 23,
            DestinationIfDivisible = 2,
            DestinationIfNot = 3
        };

        yield return new Monkey(54, 65, 75, 74) {
            OperationAdd = 6,
            Divisor = 19,
            DestinationIfDivisible = 2,
            DestinationIfNot = 0
        };

        yield return new Monkey(79, 60, 97) {
            Divisor = 13,
            DestinationIfDivisible = 1,
            DestinationIfNot = 3
        };

        yield return new Monkey(74) {
            OperationAdd = 3,
            Divisor = 17,
            DestinationIfDivisible = 0,
            DestinationIfNot = 1
        };
    }

    internal class Monkey {
        private readonly Queue<long> items = new();

        public ICollection<long> Items => items.ToArray();
        
        public int? OperationAdd { get; init; }

        public int? OperationMul { get; init; }

        public int Divisor { get; init; }

        public int DestinationIfDivisible { get; init; }

        public int DestinationIfNot { get; init; }

        public int Inspections { get; private set; }

        public Monkey(params int[] startingItems) {
            foreach (var item in startingItems)
                items.Enqueue(item);
        }

        public void TakeTurn(Monkey[] monkeys, int divisor, int modulus) {
            while (items.TryDequeue(out var item)) {
                Inspections = checked(Inspections + 1);

                var m = OperationAdd.HasValue ? 1 : OperationMul ?? item;
                var c = OperationAdd ?? 0;

                var newWorryLevel =
                    checked(((m * (item + c)) / divisor) % modulus);

                monkeys[newWorryLevel % Divisor == 0 ? DestinationIfDivisible : DestinationIfNot]
                    .Enqueue(newWorryLevel);
            }
        }

        private void Enqueue(long i) {
            items.Enqueue(i);
        }
    }
    
    [Fact]
    public void ParsesInputCorrectly() {
        var actualMonkeys = Parse(ExampleInput);
        var expectedMonkeys = GetExampleMonkeys();

        foreach (var pair in expectedMonkeys.Zip(actualMonkeys)) {
            var expected = pair.First;
            var actual = pair.Second;
            Assert.Equal(expected.Items, actual.Items);
            Assert.Equal(expected.OperationAdd, actual.OperationAdd);
            Assert.Equal(expected.OperationMul, actual.OperationMul);
            Assert.Equal(expected.Divisor, actual.Divisor);
            Assert.Equal(expected.DestinationIfDivisible, actual.DestinationIfDivisible);
            Assert.Equal(expected.DestinationIfNot, actual.DestinationIfNot);
        }
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day11(ExampleInput, Output).SolvePartOne();
        Assert.Equal(10605, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day11(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(2713310158L, actual);
    }
}