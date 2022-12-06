using AdventOfCode.Core;
using Xunit;

namespace AdventOfCode.CSharp._2022;

public class Day5 : Solver<string> {
    public Day5(string? input = null) : base(input) { }

    public override string SolvePartOne() => Solve(false);

    public override string SolvePartTwo() => Solve(true);

    public string Solve(bool isCrateMover9001) {
        var parts = Input.SplitBy("\n\n");
        var crateState = ParseCrates(parts[0]);
        var instructions = ParseInstructions(parts[1]);
        var queue = new List<char>();

        foreach (var instruction in instructions)
            if (isCrateMover9001) {
                queue.Clear();

                for (var i = 0; i < instruction.Item1; ++i)
                    queue.Add(crateState[instruction.Item2].Pop());

                queue.Reverse();

                foreach (var crate in queue)
                    crateState[instruction.Item3].Push(crate);
            }
            else {
                for (var i = 0; i < instruction.Item1; ++i)
                    crateState[instruction.Item3].Push(crateState[instruction.Item2].Pop());
            }

        return new string(crateState.Select(cs => cs.Peek()).ToArray());
    }

    private Stack<char>[] ParseCrates(string s) {
        var lines = s.Split("\n");
        var stacks = Enumerable.Range(0, (1 + lines[0].Length) / 4).Select((e) => new Stack<char>()).ToArray();

        foreach (var line in lines.Reverse().Skip(1)) {
            var stackEntries = line.Chunk(4).Select((c, i) => Tuple.Create(i, c[1]));

            foreach (var stackEntry in stackEntries.Where(se => se.Item2 != ' '))
                stacks[stackEntry.Item1].Push(stackEntry.Item2);
        }

        return stacks;
    }

    private IEnumerable<Tuple<int, int, int>> ParseInstructions(string s) {
        foreach (var line in s.Split("\n")) {
            // move 1 from 2 to 1
            // 0    1 2    3 4  5
            var parts = line.Split(" ");

            yield return Tuple.Create(int.Parse(parts[1]), int.Parse(parts[3]) - 1, int.Parse(parts[5]) - 1);
        }
    }

    private const string ExampleInput = @"
    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2
";

    [Fact]
    public void SolvesExample() {
        var solver = new Day5(ExampleInput);
        var actual = solver.Solve(false);

        Assert.Equal("CMZ", actual);
    }

    [Fact]
    public void SolvesExamplePart2() {
        var solver = new Day5(ExampleInput);
        var actual = solver.Solve(true);

        Assert.Equal("MCD", actual);
    }
}