using System.Diagnostics;
using System.Text;

using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

// ReSharper disable InconsistentNaming - puzzle uses little x, m, a, s variable names
public class Day19 : Solver {
    public Day19(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private enum Operator {
        None,

        LessThan,

        GreaterThan
    }

    private enum Variable {
        x,

        m,

        a,

        s
    }

    private record Rule {
        public Variable v;

        public Operator o;

        public int t;

        public string d = string.Empty;
    }

    private record Workflow {
        public List<Rule> Rules = new List<Rule>();

        public string dd = string.Empty;

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("{");

            foreach (var rule in Rules) {
                sb.Append($"{rule.v}{(rule.o == Operator.GreaterThan ? ">" : "<")}{rule.t}:{rule.d},");
            }

            sb.Append(dd);
            sb.Append("}");
            return sb.ToString();
        }
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

    public override long SolvePartOne() {
        var inputSections = Input.SplitBy("\n\n");
        var workflowSection = inputSections[0].Split('\n');
        var partsSection = inputSections[1].Split('\n');

        var workflows = new Dictionary<string, Workflow>();
        foreach (var workflowInput in workflowSection) {
            var bracePosition = workflowInput.IndexOf('{');
            var k = workflowInput[0..bracePosition];

            var workflowParts = workflowInput[(bracePosition + 1)..^1].Split(',');

            var w = new Workflow {
                dd = workflowParts[^1]
            };

            foreach (var ruleInput in workflowParts[0..^1]) {
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
                    s = int.Parse(partParts[3][2..]),
                });
        }

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
                        (rule.v == Variable.x) ? part.x
                        : (rule.v == Variable.m) ? part.m
                        : (rule.v == Variable.a) ? part.a
                        : (rule.v == Variable.s) ? part.s
                        : throw new InvalidOperationException();

                    if (rule.o == Operator.LessThan && v < rule.t || rule.o == Operator.GreaterThan & v > rule.t) {
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