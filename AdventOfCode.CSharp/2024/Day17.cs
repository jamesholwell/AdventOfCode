// ReSharper disable InconsistentNaming
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day17(string? input = null, ITestOutputHelper? outputHelper = null)
    : Solver<string>(input, outputHelper) {
    private long registerA;
    private long registerB;
    private long registerC;
    private readonly List<int> output = [];
    private int[] program = [];
    private int pointer;

    private static (int registerA, int registerB, int registerC, int[] program) Parse(string input) {
        var lines = Shared.Split(input);
        return (
            int.Parse(lines[0]["Register A: ".Length..]), 
            int.Parse(lines[1]["Register B: ".Length..]), 
            int.Parse(lines[2]["Register C: ".Length..]), 
            lines[4]["Program: ".Length..].SplitInt(","));
    }

    private void exec(Instructions instruction, int operand) {
        switch (instruction) {
            case Instructions.adv:
                registerA >>= (int)combo(operand);
                break;

            case Instructions.bxl:
                registerB ^= operand;
                break;

            case Instructions.bst:
                registerB = combo(operand) % 8;
                break;

            case Instructions.jnz:
                if (registerA == 0) 
                    break;

                pointer = operand;
                return;
                
            case Instructions.bxc:
                registerB ^= registerC;
                break;

            case Instructions.@out:
                output.Add((int)(combo(operand) % 8));
                break;
            
            case Instructions.bdv:
                registerB = registerA >> (int)combo(operand);
                break;

            case Instructions.cdv:
                registerC = registerA >> (int)combo(operand);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        pointer += 2;
        return;

        long combo(int code) => code switch {
            4 => registerA,
            5 => registerB,
            6 => registerC,
            7 => throw new InvalidOperationException(),
            _ => code
        };
    }

    private string PrepareTraceMessage() {
        var a = registerA;
        var b = registerB;
        var c = registerC;
        var ap = string.Join(string.Empty, program.Select((p, i) => 
            i == pointer ? $"[{p} " :
            i == pointer + 1 ? $"{p}]" : 
            i % 2 == 0 ? $" {p} " : $"{p} "));
        if (ap.Length < 10) ap += new string(' ', 10-ap.Length);

        var instruction = (Instructions)program[pointer];
        var operand = program[pointer + 1];
        var comboOperand = operand switch {
            4 => "a",
            5 => "b",
            6 => "c",
            7 => "!!!",
            _ => $"{operand}"
        };

        var narrative = instruction switch {
            Instructions.adv when operand is >= 4 and <= 6 => $"a := a >> {comboOperand}",
            Instructions.adv => $"a := a >> {operand}",
            Instructions.bxl => $"b := b ^ {operand}",
            Instructions.bst => $"b := {comboOperand} % 8",
            Instructions.jnz when registerA == 0 => "quit",
            Instructions.jnz => $"jmp {operand}",
            Instructions.bxc => $"b := b ^ c",
            Instructions.@out => $"out {comboOperand} % 8",
            Instructions.bdv => $"b := a >> {comboOperand}",
            Instructions.cdv => $"c := a >> {comboOperand}",
            _ => string.Empty
        };

        return
            $"{ap}   " + 
            $"{instruction.ToString()} {operand}   " +
            $"{narrative}{new string(' ', narrative.Length > 20 ? 0 : 20 - narrative.Length)}" +
            $"| {a,10} | {b,10} | {c,10} | ";
    }
    
    protected override string SolvePartOne() {
        (registerA, registerB, registerC, program) = Parse(Input);

        Trace.WriteLine($"program{new string(' ', program.Length < 4 ? 3 : (program.Length * 5)/2 - 7)}   op      narrative           | register a | register b | register c | output");
        Trace.WriteLine($"-------{new string('-', program.Length < 4 ? 3 : (program.Length * 5)/2 - 7)}-------------------------------|------------|------------|------------|-----------------");
        while (pointer < program.Length) {
            var traceMessage = PrepareTraceMessage();
            
            exec((Instructions)program[pointer], program[pointer + 1]);
                
            Trace.WriteLine(traceMessage + string.Join(",", output));
        }

        return string.Join(",", output);
    }

    protected override string SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    public enum Instructions : byte {
        adv = 0,
        bxl = 1,
        bst = 2,
        jnz = 3,
        bxc = 4,
        @out = 5,
        bdv = 6,
        cdv = 7
    }

    private const string ExampleInput = 
        """
        Register A: 729
        Register B: 0
        Register C: 0
        
        Program: 0,1,5,4,3,0
        """;

    private const string ExampleInputPartTwo = 
        """
        Register A: 2024
        Register B: 0
        Register C: 0
        
        Program: 0,3,5,4,3,0
        """;
    
    [Fact]
    public void ParsesInputCorrectly() {
        var actual = Parse(ExampleInput);
        Assert.Equal(729, actual.registerA);
        Assert.Equal(0, actual.registerB);
        Assert.Equal(0, actual.registerC);
        Assert.Equal([0, 1, 5, 4, 3, 0], actual.program);
    }
    
    [Theory]
    [InlineData(Instructions.adv, 2, 256, 0, 0, 256 / 4, 0, 0)]
    [InlineData(Instructions.adv, 5, 643, 3, 0, 643 / 8, 3, 0)]
    [InlineData(Instructions.bdv, 2, 256, 0, 0, 256, 256 / 4, 0)]
    [InlineData(Instructions.bdv, 5, 643, 3, 0, 643, 643 / 8, 0)]
    [InlineData(Instructions.cdv, 2, 256, 0, 0, 256, 0, 256 / 4)]
    [InlineData(Instructions.cdv, 5, 643, 3, 0, 643, 3, 643 / 8)]
    public void CalculatesDivisionCorrectly(Instructions instruction, int operand, int a, int b, int c, int expectedA, int expectedB, int expectedC) {
        registerA = a;
        registerB = b;
        registerC = c;
        
        exec(instruction, operand);
        
        Assert.Equal(expectedA, registerA);
        Assert.Equal(expectedB, registerB);
        Assert.Equal(expectedC, registerC);
    }
    
    [Theory]
    [InlineData(0, 0, 9, "2,6", null, 1, null, null)]
    [InlineData(10, 0, 0, "5,0,5,1,5,4", null, null, null, "0,1,2")]
    [InlineData(2024, 0, 0, "0,1,5,4,3,0", 0, null, null, "4,2,5,6,7,7,7,7,3,1,0")]
    [InlineData(0, 29, 0, "1,7", null, 26, null, null)]
    [InlineData(0, 2024, 43690, "4,0", null, 44354, null, null)]
    public void CalculatesExamples(int a, int b, int c, string prog, int? expectedA, int? expectedB, int? expectedC, string? expectedOutput) {
        var input = 
            $"""
             Register A: {a}
             Register B: {b}
             Register C: {c}

             Program: {prog}
             """;

        var solver = new Day17(input, Output);
        var actual = solver.SolvePartOne();
        
        if (expectedA.HasValue)
            Assert.Equal(expectedA, (int)solver.registerA);
        
        if (expectedB.HasValue)
            Assert.Equal(expectedB, (int)solver.registerB);
        
        if (expectedC.HasValue)
            Assert.Equal(expectedC, (int)solver.registerC);
        
        if (expectedOutput != null)
            Assert.Equal(expectedOutput, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day17(ExampleInput, Output).SolvePartOne();
        Assert.Equal("4,6,3,5,6,3,5,2,1,0", actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day17(ExampleInputPartTwo, Output).SolvePartTwo();
        Assert.Equal("117440", actual);
    }
}