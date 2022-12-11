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
    
    public override long SolvePartOne() {
        var monkeys = GetRealMonkeys().ToArray();

        for (var i = 0; i < 20; ++i)
            foreach(var monkey in monkeys)
                monkey.TakeTurn(monkeys);


        return monkeys.OrderByDescending(m => m.Inspections).Take(2).Aggregate(1, (acc, charge) => acc * charge.Inspections);
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
            Operation = i => i * 19,
            Test = i => i % 23 == 0 ? 2 : 3
        };

        yield return new Monkey(54, 65, 75, 74) {
            Operation = i => i + 6,
            Test = i => i % 19 == 0 ? 2 : 0 
        };

        yield return new Monkey(79, 60, 97) {
            Operation = i => i * i,
            Test = i => i % 13 == 0 ? 1 : 3
        };

        yield return new Monkey(74) {
            Operation = i => i + 3,
            Test = i => i % 17 == 0 ? 0 : 1
        };
    }

    private IEnumerable<Monkey> GetRealMonkeys() {
        yield return new Monkey(98, 97, 98, 55, 56, 72) {
            Operation = i => i * 13,
            Test = i => 0 == i % 11 ? 4 : 7
        };

        yield return new Monkey(73, 99, 55, 54, 88, 50, 55) {
            Operation = i => i + 4,
            Test = i => 0 == i % 17 ? 2 : 6
        };

        yield return new Monkey(67, 98) {
            Operation = i => i * 11,
            Test = i => 0 == i % 5 ? 6 : 5
        };

        yield return new Monkey(82, 91, 92, 53, 99) {
            Operation = i => i + 8,
            Test = i => 0 == i % 13 ? 1 : 2
        };

        yield return new Monkey(52, 62, 94, 96, 52, 87, 53, 60) {
            Operation = i => i * i,
            Test = i => 0 == i % 19 ? 3 : 1
        };

        yield return new Monkey(94, 80, 84, 79) {
            Operation = i => i + 5,
            Test = i => 0 == i % 2 ? 7 : 0
        };

        yield return new Monkey(89) {
            Operation = i => i + 1,
            Test = i => 0 == i % 3 ? 0 : 5
        };

        yield return new Monkey(70, 59, 63) {
            Operation = i => i + 3,
            Test = i => 0 == i % 7 ? 4 : 3
        };
    }

    internal class Monkey {
        private readonly Queue<int> items = new();

        public Func<int, int>? Operation { private get; init; }

        public Func<int, int>? Test { private get; init; }

        public int Inspections { get; private set; }

        public Monkey(params int[] startingItems) {
            foreach (var item in startingItems)
                items.Enqueue(item);
        }

        public void TakeTurn(Monkey[] monkeys) {
            while (items.TryDequeue(out var item)) {
                Inspections++;
                var w = Operation!(item) / 3;
                monkeys[Test!(w)].Enqueue(w);
            }
        }

        private void Enqueue(int i) {
            items.Enqueue(i);
        }
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day11(ExampleInput).SolvePartOne();
        Assert.Equal(10605, actual);
    }
}