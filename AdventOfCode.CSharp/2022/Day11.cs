using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day11 : Solver {
    private readonly ITestOutputHelper? io;

    public Day11(string? input = null) : base(input) { }

    //public Day11(string? input = null, ITestOutputHelper? io = null) : base(input) {
    //    this.io = io;
    //}
    public override long SolvePartOne() => Solve(GetExampleMonkeys().ToArray(), 20, divisor: 3);

    public override long SolvePartTwo() {
        var monkeys = GetRealMonkeys().ToArray();
        var modulus = monkeys.Aggregate(1, (acc, charge) => acc * charge.Divisor);

        return Solve(monkeys, 10000, modulus: modulus);
    }


    private long Solve(Monkey[] monkeys, int turns, int divisor = 1, int modulus = int.MaxValue) {
        for (var i = 0; i < turns; ++i)
            foreach(var monkey in monkeys)
                monkey.TakeTurn(monkeys, divisor, modulus);


        return monkeys.OrderByDescending(m => m.Inspections).Take(2).Aggregate(1L, (acc, charge) => checked(acc * charge.Inspections));
    }

    private const string? ExampleInput = @"
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

    private IEnumerable<Monkey> GetRealMonkeys() {
        yield return new Monkey(98, 97, 98, 55, 56, 72) {
            OperationMul = 13,
            Divisor = 11,
            DestinationIfDivisible = 4,
            DestinationIfNot = 7
        };

        yield return new Monkey(73, 99, 55, 54, 88, 50, 55) {
            OperationAdd = 4,
            Divisor = 17,
            DestinationIfDivisible = 2,
            DestinationIfNot = 6
        };

        yield return new Monkey(67, 98) {
            OperationMul = 11,
            Divisor = 5,
            DestinationIfDivisible = 6,
            DestinationIfNot = 5
        };

        yield return new Monkey(82, 91, 92, 53, 99) {
            OperationAdd = 8,
            Divisor = 13,
            DestinationIfDivisible = 1,
            DestinationIfNot = 2
        };

        yield return new Monkey(52, 62, 94, 96, 52, 87, 53, 60) {
            Divisor = 19,
            DestinationIfDivisible = 3,
            DestinationIfNot = 1
        };

        yield return new Monkey(94, 80, 84, 79) {
            OperationAdd = 5,
            Divisor = 2,
            DestinationIfDivisible = 7,
            DestinationIfNot = 0
        };

        yield return new Monkey(89) {
            OperationAdd = 1,
            Divisor = 3,
            DestinationIfDivisible = 0,
            DestinationIfNot = 5
        };

        yield return new Monkey(70, 59, 63) {
            OperationAdd = 3,
            Divisor = 7,
            DestinationIfDivisible = 4,
            DestinationIfNot = 3
        };
    }

    internal class Monkey {
        private readonly Queue<long> items = new();

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

                monkeys[newWorryLevel % Divisor == 0 ? DestinationIfDivisible : DestinationIfNot].Enqueue(newWorryLevel);
            }
        }

        private void Enqueue(long i) {
            items.Enqueue(i);
        }
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day11(ExampleInput).SolvePartOne();
        Assert.Equal(10605, actual);
    }


    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day11(ExampleInput).SolvePartTwo();
        Assert.Equal(2713310158, actual);
    }
}