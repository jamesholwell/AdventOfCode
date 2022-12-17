﻿using System.Collections;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day17 : Solver {
    private readonly ITestOutputHelper io;

    public Day17(string? input = null) : base(input) { }

    //public Day17(ITestOutputHelper io, string? input = null) : base(input) {
    //    this.io = io;
    //}

    private abstract class Rock {
        public int x;

        public int y;

        public abstract IEnumerable<(int, int)> Cells { get; }

        public bool TryPush(Chamber chamber, int offsetX) {
            if (chamber.IsEmptySpace(Cells.Select(c => (c.Item1 + offsetX, c.Item2)))) {
                x += offsetX;
                return true;
            }

            return false;
        }

        public bool TryFall(Chamber chamber) {
            if (chamber.IsEmptySpace(Cells.Select(c => (c.Item1, c.Item2 - 1)))) {
                y--;
                return true;
            }

            return false;
        }

        public void CommitPosition(Chamber chamber) => chamber.Commit(Cells);
    }

    private class Minus : Rock {
        public override IEnumerable<(int, int)> Cells {
            get {
                yield return (x, y);
                yield return (x + 1, y);
                yield return (x + 2, y);
                yield return (x + 3, y);
            }
        }
    }

    private class Plus : Rock {
        public override IEnumerable<(int, int)> Cells {
            get {
                yield return (x, y + 1);
                yield return (x + 1, y);
                yield return (x + 1, y + 1);
                yield return (x + 1, y + 2);
                yield return (x + 2, y + 1);
            }
        }
    }

    private class Ell : Rock {
        public override IEnumerable<(int, int)> Cells {
            get {
                yield return (x, y);
                yield return (x + 1, y);
                yield return (x + 2, y);
                yield return (x + 2, y + 1);
                yield return (x + 2, y + 2);
            }
        }
    }

    private class Bar : Rock {
        public override IEnumerable<(int, int)> Cells {
            get {
                yield return (x, y);
                yield return (x, y + 1);
                yield return (x, y + 2);
                yield return (x, y + 3);
            }
        }
    }

    private class Square : Rock {
        public override IEnumerable<(int, int)> Cells {
            get {
                yield return (x, y);
                yield return (x + 1, y);
                yield return (x, y + 1);
                yield return (x + 1, y + 1);
            }
        }
    }

    private class Chamber {
        public readonly int Width;

        public int Height { get; private set; }

        private Dictionary<int, char[]> map;

        public Chamber(int width = 7) {
            this.Width = width;
            this.map = new Dictionary<int, char[]>();

            this.map[0] = ("+" + new string('-', this.Width) + "+").ToCharArray();
            this.Height = 0;
            EnsureHeadroom();
        }

        private void EnsureHeadroom() {
            for (var y = Height + 7; y > Height ; --y) {
                if (map.ContainsKey(y)) continue;
                this.map[y] = ("|" + new string('.', this.Width) + "|").ToCharArray();
            }
        }

        public void Render(Action<string> writeLine, Rock? r = null) {
            var cells = r?.Cells.ToArray() ?? Array.Empty<(int, int)>();
            var maxHeight = cells.Any() ? Math.Max(Height, cells.Max(p => p.Item2 + 1)) : Height + 1;

            for (var y = maxHeight; y >= 0; --y) {
                var mapy = map[y].ToArray();
                foreach (var c in cells.Where(c => c.Item2 == y - 1))
                    if (mapy[c.Item1 + 1] == '.') mapy[c.Item1 + 1] = '@';

                writeLine(new string(mapy));
            }
        }

        public bool IsEmptySpace(IEnumerable<(int x, int y)> cells) {
            foreach (var (x, y) in cells)
                if (map[y + 1][x + 1] != '.')
                    return false;

            return true;
        }

        public void Commit(IEnumerable<(int x, int y)> cells) {
            foreach (var (x, y) in cells) {
                map[y + 1][x + 1] = '#';
                if (y + 1 > Height) Height = y + 1;
            }

            EnsureHeadroom();
        }
    }

    public override long SolvePartOne() {
        var gasJetInput = Input.Trim().Select(c => c == '>' ? 1 : c == '<' ? -1 : throw new ArgumentOutOfRangeException());
        using var gasJets = new Utilities.InfiniteCyclingEnumerable<int>(gasJetInput).GetEnumerator();

        var piecesInput = new Func<Rock>[] { () => new Minus(), () => new Plus(), () => new Ell(), () => new Bar(), () => new Square()};
        using var pieces = new Utilities.InfiniteCyclingEnumerable<Rock>(piecesInput).GetEnumerator();

        var chamber = new Chamber();

        for (var i = 1; i < 2023; ++i) {
            pieces.MoveNext();

            var r = pieces.Current;
            r.y = chamber.Height + 3;
            r.x = 2;

            //io.WriteLine($"Rock {i} begins falling:");
            //chamber.Render(io.WriteLine, r);
            //io.WriteLine(string.Empty);

            while (true) {
                gasJets.MoveNext();

                var isPushed = r.TryPush(chamber, gasJets.Current);
                //io.WriteLine($"Jet of gas pushes rock {(gasJets.Current == 1 ? "right" : "left")}{(isPushed ? "" : ", but nothing happens")}:");
                //chamber.Render(io.WriteLine, r);
                //io.WriteLine(string.Empty);

                var isFallen = r.TryFall(chamber);
                //io.WriteLine($"Rock falls 1 unit{(isFallen ? "" : ", causing it to come to rest")}:");
                if (!isFallen)
                    r.CommitPosition(chamber);

                //chamber.Render(io.WriteLine, r);
                //io.WriteLine(string.Empty);

                if (!isFallen) break;
            }
        }
        
        //chamber.Render(io.WriteLine);
        //io.WriteLine(string.Empty);
        return chamber.Height;
    }

    public override long SolvePartTwo() {
        var gasJetInput = Input.Trim().Select(c => c == '>' ? 1 : c == '<' ? -1 : throw new ArgumentOutOfRangeException()).ToArray();
        var gasJetModulus = gasJetInput.Length;
        using var gasJets = new Utilities.InfiniteCyclingEnumerable<int>(gasJetInput).GetEnumerator();
        Console.WriteLine($"Gas jet cycle is {gasJetInput.Length}");

        var piecesInput = new Func<Rock>[] { () => new Minus(), () => new Plus(), () => new Ell(), () => new Bar(), () => new Square() };
        using var pieces = new Utilities.InfiniteCyclingEnumerable<Rock>(piecesInput).GetEnumerator();

        var chamber = new Chamber();
        var g = 0;
        var heightIncreases = new List<(int steps, int deltaHeight)>(50);
        var heightHistory = new List<int>(gasJetModulus * 50);
        var lastHeight = 0;
        heightHistory.Add(0);


        for (var p = 0; p < gasJetModulus * 1000; ++p) {
            pieces.MoveNext();

            var r = pieces.Current;
            r.y = chamber.Height + 3;
            r.x = 2;
            
            while (true) {
                gasJets.MoveNext();

                if (p % 5 == 0 && ++g % (5 * gasJetModulus) == 0) {
                    heightIncreases.Add((p, chamber.Height - lastHeight));
                    Console.WriteLine($"p = {p}; g = {g}; h = {chamber.Height}; dh = {chamber.Height - lastHeight}");

                    lastHeight = chamber.Height;
                }

                r.TryPush(chamber, gasJets.Current);

                if (r.TryFall(chamber)) 
                    continue;

                r.CommitPosition(chamber);
                break;
            }

            heightHistory.Add(chamber.Height);
        }


        var mostFrequentHeightIncrease = heightIncreases.GroupBy(h => h.deltaHeight).MaxBy(g => g.Count()).Key;
        var skip = heightIncreases.FindIndex(h => h.deltaHeight == mostFrequentHeightIncrease);
        var leadIn = heightIncreases[skip].steps;
        var candidateCycle = heightIncreases.Skip(skip).Select(h => h.deltaHeight).ToArray(); 

        var periodicity = Enumerable.Range(2, 200)
            .First(i => IsPeriodic(candidateCycle, i));

        var cycleLength = heightIncreases.ElementAt(skip + periodicity).steps - heightIncreases.ElementAt(skip).steps;
        var cycleHeight = candidateCycle.Take(periodicity).Sum();

        Console.WriteLine($"Detected periodicity {periodicity}, cycle length {cycleLength} with height {cycleHeight} after lead-in {leadIn}");

        var heightOfLeadIn = heightHistory[leadIn];
        
        var numberOfCycles = (1000000000000 - leadIn) / cycleLength;
        var heightOfCycles = (long)cycleHeight * numberOfCycles;

        var residual = (int)((1000000000000 - leadIn) % cycleLength);
        var heightOfResidual = heightHistory[leadIn + residual] - heightHistory[leadIn];

        Console.WriteLine($"Lead in of {leadIn} = {heightOfLeadIn} then {numberOfCycles}/{cycleLength} * {cycleHeight} = {heightOfCycles} then residual {residual} = {heightOfResidual}");
        
        return heightOfLeadIn + heightOfCycles + heightOfResidual;
    }

    private bool IsPeriodic(ICollection<int> series, int period) {
        var chunks = series.Take(period * (series.Count / period)).Chunk(period).ToArray();
        return Enumerable.Range(0, period - 1).All(p => chunks.DistinctBy(c => c[p]).Count() == 1);
    }

    private const string? ExampleInput = @"
>>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day17(ExampleInput).SolvePartOne();
        Assert.Equal(3068, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day17(ExampleInput).SolvePartTwo();
        Assert.Equal(1514285714288, actual);
    }

    private static class Utilities { 
        internal class InfiniteCyclingEnumerable<T> : IEnumerable<T> {
            private readonly EnumeratorBase enumerator;

            public InfiniteCyclingEnumerable(IEnumerable<T> values) {
                this.enumerator = new ArrayEnumerator(values.ToArray());
            }
            public InfiniteCyclingEnumerable(IEnumerable<Func<T>> factories) {
                this.enumerator = new LambdaEnumerator(factories.ToArray());
            }

            public IEnumerator<T> GetEnumerator() => this.enumerator;

            IEnumerator IEnumerable.GetEnumerator() => this.enumerator;
            
            private abstract class EnumeratorBase : IEnumerator<T> {
                protected int index = -1;
                
                public bool MoveNext() {
                    index++;
                    return true;
                }

                public void Reset() {
                    index = -1;
                }

                public abstract T Current { get; }

                object IEnumerator.Current => Current!;

                public void Dispose() { }
            }
            private class ArrayEnumerator : EnumeratorBase {
                private readonly T[] values;

                public ArrayEnumerator(T[] values) {
                    this.values = values;
                }
                
                public override T Current => index < 0 ? throw new InvalidOperationException() : values[index % values.Length];
            }

            private class LambdaEnumerator : EnumeratorBase {
                private readonly Func<T>[] factories;
                
                public LambdaEnumerator(Func<T>[] factories) {
                    this.factories = factories;
                }
                
                public override T Current => index < 0 ? throw new InvalidOperationException() : factories[index % factories.Length].Invoke();

            }
        }
    }

}