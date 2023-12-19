using Xunit;
using Xunit.Abstractions;
// ReSharper disable InconsistentNaming - puzzle uses little x, m, a, s variable names

namespace AdventOfCode.CSharp._2023;

public class Day19 : Solver {
    public Day19(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private enum Operator {
        None,
        LessThan,
        GreaterThan
    }
    
    private record Workflow {
        // precedences
        public int xp;
        public int mp;
        public int ap;
        public int sp;
        
        // operators
        public Operator xo;
        public Operator mo;
        public Operator ao;
        public Operator so;
        
        // thresholds
        public int xt;
        public int mt;
        public int at;
        public int st;
        
        // destinations
        public string xd;
        public string md;
        public string ad;
        public string sd;
        public string dd;
    }
    
    private record Part {
        public int x;
        public int m;
        public int a;
        public int s;
    }
    
    public override long SolvePartOne() {
        var inputSections = Input.SplitBy("\n\n");
        var workflowSection = inputSections[0].Split('\n');
        var partsSection = inputSections[1].Split('\n');

        var workflows = new Dictionary<string, Workflow>();
        foreach (var workflowInput in workflowSection) {
            var bracePosition = workflowInput.IndexOf('{');
            var k = workflowInput[0..bracePosition];
            
            var workflowParts = workflowInput[(bracePosition+1)..^1].Split(',');
            
            var w = new Workflow {
                dd = workflowParts[^1]
            };

            var i = 0;
            foreach (var ruleInput in workflowParts[0..^1]) {
                var ruleParts = ruleInput.Split(':');
                var v = ruleParts[0][0];
                var o = ruleParts[0][1] == '<' ? Operator.LessThan : Operator.GreaterThan;
                var t = int.Parse(ruleParts[0][2..]);
                var d = ruleParts[1];

                switch (v) {
                    case 'x':
                        w.xp = ++i;
                        w.xo = o;
                        w.xt = t;
                        w.xd = d;
                        break;
                    case 'm':
                        w.mp = ++i;
                        w.mo = o;
                        w.mt = t;
                        w.md = d;
                        break;
                    case 'a':
                        w.ap = ++i;
                        w.ao = o;
                        w.at = t;
                        w.ad = d;
                        break;
                    case 's':
                        w.sp = ++i;
                        w.so = o;
                        w.st = t;
                        w.sd = d;
                        break;
                }
            }
            
            workflows.Add(k, w);
        }
        
        var parts = new List<Part>(partsSection.Length);
        foreach (var partInput in partsSection) {
            var partParts = partInput[1..^1].Split(',');
            parts.Add(new Part
            {
                x = int.Parse(partParts[0][2..]),
                m = int.Parse(partParts[1][2..]),
                a = int.Parse(partParts[2][2..]),
                s = int.Parse(partParts[3][2..]),
            });
        }

        var accept = new List<Part>();

        foreach (var part in parts) {
            
        }
        
        return parts.Count;
    }

    public override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day19(ExampleInput, Output).SolvePartOne();
        Assert.Equal(19114, actual);
    }
}