using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day24(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver<string>(input, outputHelper) {
    private Dictionary<string, bool> state = new();

    private ((string wire, bool state)[] assignments, (string operand, string left, string right, string output)[] gates) Parse() {
        var parts = Input.SplitBy("\n\n");
        
        var assignments = parts[0].Split('\n')
            .Select(s => s.Split(": "))
            .Select(ss => (wire: ss[0], state: (int?)int.Parse(ss[1]) == 1))
            .ToArray();

        var gates = parts[1].Split('\n')
            .Select(s => s.Split(' ', 5))
            .Select(ss => (operand: ss[1], left: ss[0], right: ss[2], output: ss[4]))
            .ToArray();

        return (assignments, gates);
    }

    protected override string SolvePartOne() {
        var (assignments, gates) = Parse();

        state = assignments.ToDictionary(a => a.wire, a => a.state);

        var queue = new Queue<(string operand, string left, string right, string output)>(gates);
        while (queue.TryDequeue(out var gate)) {
            if (!state.TryGetValue(gate.left, out var left)
                || !state.TryGetValue(gate.right, out var right)) {
                queue.Enqueue(gate);
                continue;
            }

            state[gate.output] = gate.operand switch {
                "AND" => left && right,
                "OR" => left || right,
                "XOR" => left ^ right,
                _ => throw new InvalidOperationException($"Unknown operand: {gate.operand}")
            };
        }

        foreach (var wire in state.OrderBy(p => p.Key)) 
            Trace.WriteLine($"{wire.Key}: {(wire.Value ? "1" : "0")}");

        var z = state.Where(wire => wire.Key.StartsWith('z') && wire.Value)
                    .Aggregate(0L, (acc, i) => acc + (1L << int.Parse(i.Key[1..])));

        return z.ToString();
    }

    protected override string SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? SmallExampleInput = 
        """
        x00: 1
        x01: 1
        x02: 1
        y00: 0
        y01: 1
        y02: 0
        
        x00 AND y00 -> z00
        x01 XOR y01 -> z01
        x02 OR y02 -> z02
        """;

    private const string? ExampleInput = 
        """
        x00: 1
        x01: 0
        x02: 1
        x03: 1
        x04: 0
        y00: 1
        y01: 1
        y02: 1
        y03: 1
        y04: 1
        
        ntg XOR fgs -> mjb
        y02 OR x01 -> tnw
        kwq OR kpj -> z05
        x00 OR x03 -> fst
        tgd XOR rvg -> z01
        vdt OR tnw -> bfw
        bfw AND frj -> z10
        ffh OR nrd -> bqk
        y00 AND y03 -> djm
        y03 OR y00 -> psh
        bqk OR frj -> z08
        tnw OR fst -> frj
        gnj AND tgd -> z11
        bfw XOR mjb -> z00
        x03 OR x00 -> vdt
        gnj AND wpb -> z02
        x04 AND y00 -> kjc
        djm OR pbm -> qhw
        nrd AND vdt -> hwm
        kjc AND fst -> rvg
        y04 OR y02 -> fgs
        y01 AND x02 -> pbm
        ntg OR kjc -> kwq
        psh XOR fgs -> tgd
        qhw XOR tgd -> z09
        pbm OR djm -> kpj
        x03 XOR y03 -> ffh
        x00 XOR y04 -> ntg
        bfw OR bqk -> z06
        nrd XOR fgs -> wpb
        frj XOR qhw -> z04
        bqk OR frj -> z07
        y03 OR x01 -> nrd
        hwm AND bqk -> z03
        tgd XOR rvg -> z12
        tnw OR pbm -> gnj
        """;

    [Fact]
    public void SolvesPartSmallExample() {
        var actual = new Day24(SmallExampleInput, Output).SolvePartOne();
        Assert.Equal("4", actual);
    }
    
    [Fact]
    public void VerifyPartOneRegisters() {
        var solver = new Day24(ExampleInput, Output);
        solver.SolvePartOne();
      
        Assert.True(solver.state["bfw"]);  // bfw: 1
        Assert.True(solver.state["bqk"]);  // bqk: 1
        Assert.True(solver.state["djm"]);  // djm: 1
        Assert.False(solver.state["ffh"]); // ffh: 0
        Assert.True(solver.state["fgs"]);  // fgs: 1
        Assert.True(solver.state["frj"]);  // frj: 1
        Assert.True(solver.state["fst"]);  // fst: 1
        Assert.True(solver.state["gnj"]);  // gnj: 1
        Assert.True(solver.state["hwm"]);  // hwm: 1
        Assert.False(solver.state["kjc"]); // kjc: 0
        Assert.True(solver.state["kpj"]);  // kpj: 1
        Assert.False(solver.state["kwq"]); // kwq: 0
        Assert.True(solver.state["mjb"]);  // mjb: 1
        Assert.True(solver.state["nrd"]);  // nrd: 1
        Assert.False(solver.state["ntg"]); // ntg: 0
        Assert.True(solver.state["pbm"]);  // pbm: 1
        Assert.True(solver.state["psh"]);  // psh: 1
        Assert.True(solver.state["qhw"]);  // qhw: 1
        Assert.False(solver.state["rvg"]); // rvg: 0
        Assert.False(solver.state["tgd"]); // tgd: 0
        Assert.True(solver.state["tnw"]);  // tnw: 1
        Assert.True(solver.state["vdt"]);  // vdt: 1
        Assert.False(solver.state["wpb"]); // wpb: 0
        Assert.False(solver.state["z00"]); // z00: 0
        Assert.False(solver.state["z01"]); // z01: 0
        Assert.False(solver.state["z02"]); // z02: 0
        Assert.True(solver.state["z03"]);  // z03: 1
        Assert.False(solver.state["z04"]); // z04: 0
        Assert.True(solver.state["z05"]);  // z05: 1
        Assert.True(solver.state["z06"]);  // z06: 1
        Assert.True(solver.state["z07"]);  // z07: 1
        Assert.True(solver.state["z08"]);  // z08: 1
        Assert.True(solver.state["z09"]);  // z09: 1
        Assert.True(solver.state["z10"]);  // z10: 1
        Assert.False(solver.state["z11"]); // z11: 0
        Assert.False(solver.state["z12"]); // z12: 0
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day24(ExampleInput, Output).SolvePartOne();
        Assert.Equal("2024", actual);
    }
}