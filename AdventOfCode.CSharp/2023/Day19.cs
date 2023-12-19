using System.Diagnostics;
using System.Text;

using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

// ReSharper disable InconsistentNaming - puzzle uses little x, m, a, s variable names
public class Day19 : Solver {
    public Day19(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() {
        var (workflows, parts) = Parse(Input);

        var accept = new List<Part>();

        foreach (var part in parts) {
            var queue = new Queue<string>();
            var currentLocation = "in";

            while (currentLocation != "A" && currentLocation != "R") {
                queue.Enqueue(currentLocation);

                var workflow = workflows[currentLocation];
                currentLocation = workflow.dd;

                foreach (var rule in workflow.Rules) {
                    var v =
                        rule.v == Variable.x ? part.x
                        : rule.v == Variable.m ? part.m
                        : rule.v == Variable.a ? part.a
                        : rule.v == Variable.s ? part.s
                        : throw new InvalidOperationException();

                    if ((rule.o == Operator.LessThan && v < rule.t) ||
                        (rule.o == Operator.GreaterThan) & (v > rule.t)) {
                        currentLocation = rule.d;
                        break;
                    }
                }
            }

            queue.Enqueue(currentLocation);

            if (currentLocation == "A")
                accept.Add(part);

            Trace.WriteLine($"{part}: {string.Join(" -> ", queue)}");
        }

        return accept.Sum(p => p.x + p.m + p.a + p.s);
    }

    public override long SolvePartTwo() {
        var (workflows, _) = Parse(Input);

        var accept = new List<PartRange>();
        var ranges = new Queue<PartRange>();
        ranges.Enqueue(
            new PartRange {
                location = "in",
                x1 = 1, x2 = 4000,
                m1 = 1, m2 = 4000,
                a1 = 1, a2 = 4000,
                s1 = 1, s2 = 4000,
            });

        while (ranges.TryDequeue(out var range)) {
            Trace.WriteLine(range);
            
            if (range.location == "A") {
                accept.Add(range);
                continue;
            }

            if (range.location == "R")
                continue;

            var workflow = workflows[range.location];
            foreach (var rule in workflow.Rules) {
                // detect empty remaining range
                if (range == default)
                    break;
                
                switch (rule.v) {
                    case Variable.x:
                        if (rule.o == Operator.LessThan && range.x1 < rule.t) {
                            ranges.Enqueue(range with { x2 = Math.Min(range.x2, rule.t - 1), location = rule.d});

                            if (rule.t < range.x2)
                                range.x1 = rule.t;
                            else
                                range = default;
                        }
                        else if (range.x2 > rule.t) {
                            ranges.Enqueue(range with { x1 = Math.Max(range.x1, rule.t + 1), location = rule.d });
                            
                            if (rule.t > range.x1)
                                range.x2 = rule.t;
                            else
                                range = default;
                        }
                        break;

                    case Variable.m:
                        if (rule.o == Operator.LessThan && range.m1 < rule.t) {
                            ranges.Enqueue(range with { m2 = Math.Min(range.m2, rule.t - 1), location = rule.d});

                            if (rule.t < range.m2)
                                range.m1 = rule.t;
                            else
                                range = default;
                        }
                        else if (range.m2 > rule.t) {
                            ranges.Enqueue(range with { m1 = Math.Max(range.m1, rule.t + 1), location = rule.d });
                            
                            if (rule.t > range.m1)
                                range.m2 = rule.t;
                            else
                                range = default;
                        }
                        break;
                    
                    case Variable.a:
                        if (rule.o == Operator.LessThan && range.a1 < rule.t) {
                            ranges.Enqueue(range with { a2 = Math.Min(range.a2, rule.t - 1), location = rule.d});

                            if (rule.t < range.a2)
                                range.a1 = rule.t;
                            else
                                range = default;
                        }
                        else if (range.a2 > rule.t) {
                            ranges.Enqueue(range with { a1 = Math.Max(range.a1, rule.t + 1), location = rule.d });
                            
                            if (rule.t > range.a1)
                                range.a2 = rule.t;
                            else
                                range = default;
                        }
                        break;
                    
                    case Variable.s:
                        if (rule.o == Operator.LessThan && range.s1 < rule.t) {
                            ranges.Enqueue(range with { s2 = Math.Min(range.s2, rule.t - 1), location = rule.d});

                            if (rule.t < range.s2)
                                range.s1 = rule.t;
                            else
                                range = default;
                        }
                        else if (range.s2 > rule.t) {
                            ranges.Enqueue(range with { s1 = Math.Max(range.s1, rule.t + 1), location = rule.d });
                            
                            if (rule.t > range.s1)
                                range.s2 = rule.t;
                            else
                                range = default;
                        }
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            if (range != default)
                ranges.Enqueue(range with { location = workflow.dd });
        }
        
        return accept.Sum(p => (long)(1 + p.x2 - p.x1) * (1 + p.m2 - p.m1) * (1 + p.a2 - p.a1) * (1 + p.s2 - p.s1));
    }

    private static (Dictionary<string, Workflow> workflows, List<Part> parts) Parse(string input) {
        var inputSections = input.SplitBy("\n\n");
        var workflowSection = inputSections[0].Split('\n');
        var partsSection = inputSections[1].Split('\n');

        var workflows = new Dictionary<string, Workflow>();
        foreach (var workflowInput in workflowSection) {
            var bracePosition = workflowInput.IndexOf('{');
            var k = workflowInput[..bracePosition];

            var workflowParts = workflowInput[(bracePosition + 1)..^1].Split(',');

            var w = new Workflow {
                dd = workflowParts[^1]
            };

            foreach (var ruleInput in workflowParts[..^1]) {
                var ruleParts = ruleInput.Split(':');
                var v = ruleParts[0][0] switch {
                    'x' => Variable.x,
                    'm' => Variable.m,
                    'a' => Variable.a,
                    's' => Variable.s,
                    _ => throw new InvalidOperationException()
                };
                var o = ruleParts[0][1] == '<' ? Operator.LessThan : Operator.GreaterThan;
                var t = int.Parse(ruleParts[0][2..]);
                var d = ruleParts[1];

                w.Rules.Add(new Rule { v = v, o = o, t = t, d = d });
            }

            Debug.Assert(workflowInput == k + w);
            workflows.Add(k, w);
        }

        var parts = new List<Part>(partsSection.Length);
        foreach (var partInput in partsSection) {
            var partParts = partInput[1..^1].Split(',');
            parts.Add(
                new Part {
                    x = int.Parse(partParts[0][2..]),
                    m = int.Parse(partParts[1][2..]),
                    a = int.Parse(partParts[2][2..]),
                    s = int.Parse(partParts[3][2..])
                });
        }
        return (workflows, parts);
    }

    private record Part {
        public int x;

        public int m;

        public int a;

        public int s;

        public override string ToString() {
            return $"x={x},m={m},a={a},s={s}";
        }
    }

    private record PartRange {
        public string location = "in";
        
        public int x1;
        
        public int x2;

        public int m1;

        public int m2;

        public int a1;

        public int a2;

        public int s1;

        public int s2;

        public override string ToString() {
            return $"x={x1}--{x2},m={m1}--{m2},a={a1}--{a2},s={s1}--{s2} @ {location}";
        }
    }

    private record Workflow {
        public readonly List<Rule> Rules = new();

        public string dd = string.Empty;

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("{");

            foreach (var rule in Rules)
                sb.Append($"{rule.v}{(rule.o == Operator.GreaterThan ? ">" : "<")}{rule.t}:{rule.d},");

            sb.Append(dd);
            sb.Append("}");
            return sb.ToString();
        }
    }

    private record Rule {
        public string d = string.Empty;

        public Operator o;

        public int t;

        public Variable v;
    }

    private enum Operator {
        LessThan,

        GreaterThan
    }

    private enum Variable {
        x,

        m,

        a,

        s
    }

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
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day19(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(167409079868000, actual);
    }
}