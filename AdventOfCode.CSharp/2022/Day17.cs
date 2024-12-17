using System.Collections;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2022;

public class Day17(string? input = null, ITestOutputHelper? outputHelper = null) : Solver(input, outputHelper) {
    private abstract class Rock {
        public int X;

        public int Y;

        public abstract IEnumerable<(int, int)> Cells { get; }

        public void TryPush(Chamber chamber, int offsetX) {
            if (chamber.IsEmptySpace(Cells.Select(c => (c.Item1 + offsetX, c.Item2))))
                X += offsetX;
        }

        public bool TryFall(Chamber chamber) {
            if (chamber.IsEmptySpace(Cells.Select(c => (c.Item1, c.Item2 - 1)))) {
                Y--;
                return true;
            }

            return false;
        }

        public void CommitPosition(Chamber chamber) => chamber.Commit(Cells);
    }

    private class Minus : Rock {
        public override IEnumerable<(int, int)> Cells
        {
            get
            {
                yield return (X, Y);
                yield return (X + 1, Y);
                yield return (X + 2, Y);
                yield return (X + 3, Y);
            }
        }
    }

    private class Plus : Rock {
        public override IEnumerable<(int, int)> Cells
        {
            get
            {
                yield return (X, Y + 1);
                yield return (X + 1, Y);
                yield return (X + 1, Y + 1);
                yield return (X + 1, Y + 2);
                yield return (X + 2, Y + 1);
            }
        }
    }

    private class Ell : Rock {
        public override IEnumerable<(int, int)> Cells
        {
            get
            {
                yield return (X, Y);
                yield return (X + 1, Y);
                yield return (X + 2, Y);
                yield return (X + 2, Y + 1);
                yield return (X + 2, Y + 2);
            }
        }
    }

    private class Bar : Rock {
        public override IEnumerable<(int, int)> Cells
        {
            get
            {
                yield return (X, Y);
                yield return (X, Y + 1);
                yield return (X, Y + 2);
                yield return (X, Y + 3);
            }
        }
    }

    private class Square : Rock {
        public override IEnumerable<(int, int)> Cells
        {
            get
            {
                yield return (X, Y);
                yield return (X + 1, Y);
                yield return (X, Y + 1);
                yield return (X + 1, Y + 1);
            }
        }
    }

    private class Chamber {
        private readonly int width;

        public int Height { get; private set; }

        private readonly Dictionary<int, char[]> map;

        public Chamber(int width = 7) {
            this.width = width;
            this.map = new Dictionary<int, char[]> {
                [0] = ("+" + new string('-', this.width) + "+").ToCharArray()
            };

            this.Height = 0;
            EnsureHeadroom();
        }

        private void EnsureHeadroom() {
            for (var y = Height + 7; y > Height; --y) {
                if (map.ContainsKey(y)) continue;
                this.map[y] = ("|" + new string('.', this.width) + "|").ToCharArray();
            }
        }

        public void Render(Action<string> writeLine, Rock? r = null) {
            var cells = r?.Cells.ToArray() ?? Array.Empty<(int, int)>();
            var maxHeight = cells.Any() ? Math.Max(Height, cells.Max(p => p.Item2 + 1)) : Height + 1;

            for (var y = maxHeight; y >= 0; --y) {
                var mapY = map[y].ToArray();
                foreach (var c in cells.Where(c => c.Item2 == y - 1))
                    if (mapY[c.Item1 + 1] == '.')
                        mapY[c.Item1 + 1] = '@';

                writeLine(new string(mapY));
            }
        }

        public string GetTop() {
            if (Height > 2)
                return new string(map[Height].Concat(map[Height - 1]).Concat(map[Height - 2]).Concat(map[Height - 3])
                    .ToArray());

            if (Height > 1)
                return new string(map[Height].Concat(map[Height - 1]).Concat(map[Height - 2]).ToArray());

            if (Height > 0)
                return new string(map[Height].Concat(map[Height - 1]).ToArray());

            return new string(map[Height]);
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

    protected override long SolvePartOne() {
        var gasJetInput =
            Input.Trim().Select(c => c == '>' ? 1 : c == '<' ? -1 : throw new ArgumentOutOfRangeException());
        using var gasJets = new Utilities.InfiniteCyclingEnumerable<int>(gasJetInput).GetEnumerator();

        var piecesInput = new Func<Rock>[]
            { () => new Minus(), () => new Plus(), () => new Ell(), () => new Bar(), () => new Square() };
        using var pieces = new Utilities.InfiniteCyclingEnumerable<Rock>(piecesInput).GetEnumerator();

        var chamber = new Chamber();

        for (var i = 1; i < 2023; ++i) {
            pieces.MoveNext();

            var r = pieces.Current;
            r.Y = chamber.Height + 3;
            r.X = 2;

            while (true) {
                gasJets.MoveNext();

                r.TryPush(chamber, gasJets.Current);
                var isFallen = r.TryFall(chamber);
                if (!isFallen)
                    r.CommitPosition(chamber);

                // chamber.Render(Trace.WriteLine);
                // Trace.WriteLine(string.Empty);

                if (!isFallen) break;
            }
        }

        chamber.Render(Trace.WriteLine);
        Trace.WriteLine(string.Empty);
        return chamber.Height;
    }

    protected override long SolvePartTwo() {
        var gasJetInput = Input.Trim()
            .Select(c => c == '>' ? 1 : c == '<' ? -1 : throw new ArgumentOutOfRangeException()).ToArray();
        var gasJetModulus = gasJetInput.Length;
        using var gasJets = new Utilities.InfiniteCyclingEnumerable<int>(gasJetInput).GetEnumerator();
        Trace.WriteLine($"Gas jet cycle is {gasJetInput.Length}");

        var piecesInput = new Func<Rock>[]
            { () => new Minus(), () => new Plus(), () => new Ell(), () => new Bar(), () => new Square() };
        using var pieces = new Utilities.InfiniteCyclingEnumerable<Rock>(piecesInput).GetEnumerator();

        var chamber = new Chamber();
        var g = 0;
        var heightHistory = new Dictionary<int, int>();
        var topHistory = new Dictionary<(string, int, int), int>();
        var leadIn = 0;
        var cycleLength = 0;
        var cycleHeight = 0;

        for (var p = 0; p < gasJetModulus * 1000; ++p) {
            pieces.MoveNext();

            var r = pieces.Current;
            r.Y = chamber.Height + 3;
            r.X = 2;

            while (true) {
                gasJets.MoveNext();
                g++;

                r.TryPush(chamber, gasJets.Current);

                if (r.TryFall(chamber))
                    continue;

                r.CommitPosition(chamber);
                break;
            }

            heightHistory[p + 1] = chamber.Height;

            var k = (chamber.GetTop(), p % 5, g % gasJetModulus);

            if (leadIn > 0) {
                if (p > leadIn + cycleLength * 2) break;
            }
            else if (topHistory.ContainsKey(k) && leadIn == 0) {
                Trace.WriteLine($"Detected loop: {k}");
                Trace.WriteLine(
                    $"...step {topHistory[k]} @ height {heightHistory[topHistory[k]]} === step {p} @ height {chamber.Height}");

                leadIn = topHistory[k]; // first 11 pieces are the run-up to the cycle

                cycleLength = p - leadIn;
                cycleHeight =
                    heightHistory[p] -
                    heightHistory
                        [leadIn]; // the "add" for each cycle is the height before the cycle to the height at the end

                Trace.WriteLine(
                    $"... lead in of {leadIn} followed by cycle of {cycleLength} for rise of {cycleHeight}");
            }
            else
                topHistory.Add(k, p);
        }

        for (var i = leadIn + 1; i < heightHistory.Count; i++) {
            var prediction = PredictionAtHeight(heightHistory, leadIn, cycleLength, cycleHeight, i);
            Assert.Equal(heightHistory[i], prediction);
        }

        return PredictionAtHeight(heightHistory, leadIn, cycleLength, cycleHeight, 1000000000000);
    }

    private static long PredictionAtHeight(Dictionary<int, int> heightHistory, int leadIn, int cycleLength,
        int cycleHeight, long predictionHeight) {
        var heightOfLeadIn = heightHistory[leadIn];

        var numberOfCycles = (predictionHeight - leadIn) / cycleLength;
        var heightOfCycles = cycleHeight * numberOfCycles;

        var residual = (int)(predictionHeight - leadIn - numberOfCycles * cycleLength);
        var heightOfResidual = heightHistory[leadIn + cycleLength + residual] - heightHistory[leadIn + cycleLength];

        return heightOfLeadIn + heightOfCycles + heightOfResidual;
    }

    private const string? ExampleInput = @"
>>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day17(ExampleInput, Output).SolvePartOne();
        Assert.Equal(3068, actual);
    }

    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day17(ExampleInput, Output).SolvePartTwo();
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
                protected int Index = -1;

                public bool MoveNext() {
                    Index++;
                    return true;
                }

                public void Reset() {
                    Index = -1;
                }

                public abstract T Current { get; }

                object IEnumerator.Current => Current ?? throw new InvalidOperationException();

                public void Dispose() { }
            }

            private class ArrayEnumerator(T[] values) : EnumeratorBase {
                public override T Current =>
                    Index < 0 ? throw new InvalidOperationException() : values[Index % values.Length];
            }

            private class LambdaEnumerator(Func<T>[] factories) : EnumeratorBase {
                public override T Current => Index < 0
                    ? throw new InvalidOperationException()
                    : factories[Index % factories.Length].Invoke();
            }
        }
    }
}