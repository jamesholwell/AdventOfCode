using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day20 : Solver {
    public Day20(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    // in AoC land, everything is upside down. 
    public enum Signal {
        High = 0,
        Low = 1
    }
    
    public override long SolvePartOne() {
        int lowPulses = 0, highPulses = 0;
        var circuit = Parse(Input);
        var pulses = new Queue<(string sender, string receiver, int index, Signal signal)>();

        for (var i = 0; i < 1000; ++i) {
            // queue button push
            pulses.Enqueue(("button", "broadcaster", 0, Signal.Low));

            // now dequeue until done
            while (pulses.TryDequeue(out var pulse)) {
                Trace.WriteLine($"{pulse.sender} -{(pulse.signal == Signal.Low ? "low" : "high")}-> {pulse.receiver} @ {pulse.index}");
                // record the pulse
                if (pulse.signal == Signal.High)
                    highPulses++;
                else
                    lowPulses++;

                // skip circuits that are output-only
                if (!circuit.Destinations.TryGetValue(pulse.receiver, out var destinations))
                    continue;
                
                // determine the module type
                switch (pulse.receiver[0]) {
                    case 'b':
                        foreach (var (destinationModule, destinationIndex) in destinations)
                            pulses.Enqueue((pulse.receiver, destinationModule, destinationIndex, pulse.signal));
                        break;
                    
                    case '%':
                        if (pulse.signal == Signal.High)
                            continue;
                        
                        var flopSignal = circuit.ToggleRegister(pulse.receiver);
                        foreach (var (destinationModule, destinationIndex) in destinations)
                            pulses.Enqueue((pulse.receiver, destinationModule, destinationIndex, flopSignal));
                        break;
                    
                    case '&':
                        circuit.SetRegister(pulse.receiver, pulse.index, pulse.signal);
                        var conSignal = circuit.Registers[pulse.receiver] == (int)Signal.High ? Signal.Low : Signal.High;
                        
                        foreach (var (destinationModule, destinationIndex) in destinations)
                            pulses.Enqueue((pulse.receiver, destinationModule, destinationIndex, conSignal));
                        break;
                }
            }
        }
        
        return lowPulses * highPulses;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private Circuit Parse(string input) {
        var c = new Circuit();

        // preprocess 
        var processedInput = input;
        var registerIndexes = new Dictionary<string, int>();
        foreach (var line in Shared.Split(input)) {
            var parts = line.Split("->", 2, StringSplitOptions.TrimEntries);
            var module = parts[0];
            
            if (module[0] != '%' && module[0] != '&')
                continue;
            
            // prepend the module type
            processedInput = processedInput.Replace(" " + module[1..], module);
            
            // initialize the register indexer
            registerIndexes[module] = 0;
        }
        
        // process
        foreach (var line in Shared.Split(processedInput)) {
            var parts = line.Split("->", 2, StringSplitOptions.TrimEntries);
            var destinations = parts[1].Split(',', StringSplitOptions.TrimEntries);

            var destinationTuples = destinations.Select(destination => {
                var registerIndex = destination[0] == '&' ? registerIndexes[destination]++ : 0;
                c.SetRegister(destination, registerIndex, Signal.Low);
                return (d: destination, registerIndex);
            }).ToArray();  
                
            c.Destinations.Add(parts[0], destinationTuples);
        }
        
        return c;
    }

    private class Circuit {
        public readonly Dictionary<string, (string, int)[]> Destinations = new();

        public readonly Dictionary<string, int> Registers = new();

        public void SetRegister(string module, int registerIndex, Signal signal) {
            // initialize register if required
            Registers.TryAdd(module, 0);
            
            // find the correct register bit
            var bit = 1 << registerIndex;
            
            if (signal == Signal.High)
                Registers[module] &= ~bit; // unset the bit, life is backwards
            else
                Registers[module] |= bit; // set the bit, life is backwards
        }

        public Signal ToggleRegister(string module) {
            return (Signal)(Registers[module] = 1 - Registers[module]);
        }
    }
    
    private const string? ExampleInput1 = @"
broadcaster -> a, b, c
%a -> b
%b -> c
%c -> inv
&inv -> a
";
    
    private const string? ExampleInput2 = @"
broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> output
";

    [Fact]
    public void ParsesExampleInput1() {
        var actual = Parse(ExampleInput1!);

        var broadcastDestinations = actual.Destinations["broadcaster"];
        Assert.Equal(3, broadcastDestinations.Length);
        Assert.Equal(("%a", 0), broadcastDestinations[0]);
        Assert.Equal(("%b", 0), broadcastDestinations[1]);
        Assert.Equal(("%c", 0), broadcastDestinations[2]);
        
        var aDestinations = actual.Destinations["%a"];
        Assert.Single(aDestinations);
        Assert.Equal(("%b", 0), aDestinations[0]);
        
        var bDestinations = actual.Destinations["%b"];
        Assert.Single(bDestinations);
        Assert.Equal(("%c", 0), bDestinations[0]);
        
        var cDestinations = actual.Destinations["%c"];
        Assert.Single(cDestinations);
        Assert.Equal(("&inv", 0), cDestinations[0]);
        
        var invDestinations = actual.Destinations["&inv"];
        Assert.Single(invDestinations);
        Assert.Equal(("%a", 0), invDestinations[0]);
        
        var aRegister = actual.Registers["%a"];
        Assert.Equal((int)Signal.Low, aRegister);
        
        var bRegister = actual.Registers["%b"];
        Assert.Equal((int)Signal.Low, bRegister);
        
        var cRegister = actual.Registers["%c"];
        Assert.Equal((int)Signal.Low, cRegister);
        
        var invRegister = actual.Registers["&inv"];
        Assert.Equal((int)Signal.Low, invRegister);
    }
    
    [Fact]
    public void ParsesExampleInput2() {
        var actual = Parse(ExampleInput2!);
        
        var broadcastDestinations = actual.Destinations["broadcaster"];
        Assert.Single(broadcastDestinations);
        Assert.Equal(("%a", 0), broadcastDestinations[0]);
        
        var aDestinations = actual.Destinations["%a"];
        Assert.Equal(2, aDestinations.Length);
        Assert.Equal(("&inv", 0), aDestinations[0]);
        Assert.Equal(("&con", 0), aDestinations[1]);
        
        var invDestinations = actual.Destinations["&inv"];
        Assert.Single(invDestinations);
        Assert.Equal(("%b", 0), invDestinations[0]);
        
        var bDestinations = actual.Destinations["%b"];
        Assert.Single(bDestinations);
        Assert.Equal(("&con", 1), bDestinations[0]);
        
        var conDestinations = actual.Destinations["&con"];
        Assert.Single(conDestinations);
        Assert.Equal(("output", 0), conDestinations[0]);
        
        var aRegister = actual.Registers["%a"];
        Assert.Equal((int)Signal.Low, aRegister);
        
        var invRegister = actual.Registers["&inv"];
        Assert.Equal((int)Signal.Low, invRegister);
        
        var bRegister = actual.Registers["%b"];
        Assert.Equal((int)Signal.Low, bRegister);
        
        var conRegister = actual.Registers["&con"];
        Assert.Equal(3, conRegister); // 3 = 0b11
    }

    [Fact]
    public void SolvesPartOneExample1() {
        var actual = new Day20(ExampleInput1, Output).SolvePartOne();
        Assert.Equal(32000000, actual);
    }
    
    [Fact]
    public void SolvesPartOneExample2() {
        var actual = new Day20(ExampleInput2, Output).SolvePartOne();
        Assert.Equal(11687500, actual);
    }
}