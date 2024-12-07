using System.Linq.Expressions;
using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day7 : Solver {
    public Day7(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private static (long Value, long[] Operands)[] Parse(string input) {
        return Shared.Split(input)
            .Select(s => {
                var parts = s.Split(new[] { ':', ' ' },
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return (long.Parse(parts[0]), parts.Skip(1).Select(long.Parse).ToArray());
            })
            .ToArray();
    }

    private readonly Dictionary<int, Func<long[], long>[]> evaluatorsCache = new();

    private Func<long[], long>[] GetEvaluators(int length, params Operators[] availableOperators) {
        if (evaluatorsCache.TryGetValue(length, out var cachedEvaluators)) 
            return cachedEvaluators;

        var operandsParameter = Expression.Parameter(typeof(long[]));
        
        var operators = availableOperators.Select(ao => new[] { ao }).ToArray();
        var combinations = operators.ToArray();
        for (var i = 1; i < length - 1; i++) {
            combinations = combinations
                .SelectMany(c => operators.Select(o => c.Concat(o).ToArray()))
                .ToArray();
        }
        
        var evaluators = new Func<long[], long>[combinations.Length];
        var evaluatorIndex = 0;

        var floor = typeof(Math).GetMethod(nameof(Math.Floor), new[] { typeof(double) })!;
        var pow = typeof(Math).GetMethod(nameof(Math.Pow))!;
        var log10 = typeof(Math).GetMethod(nameof(Math.Log10))!;

        foreach (var combination in combinations) {
            var parameterIdx = 0;
            Expression expression = Expression.ArrayAccess(operandsParameter, Expression.Constant(parameterIdx++));

            expression = combination.Aggregate(expression, (current, operation) => operation switch {
                Operators.Add => Expression.Add(current, Expression.ArrayAccess(operandsParameter, Expression.Constant(parameterIdx++))),
                
                Operators.Mul => Expression.Multiply(current, Expression.ArrayAccess(operandsParameter, Expression.Constant(parameterIdx++))),
                
                Operators.Concat =>
                    Expression.Add(
                        Expression.Multiply(
                            current,
                            Expression.Convert(
                                Expression.Call(
                                    pow,
                                    Expression.Constant(10D),
                                    Expression.Add(
                                        Expression.Constant(1D),
                                        Expression.Call(
                                            floor,
                                            Expression.Call(
                                                log10,
                                                Expression.Convert(Expression.ArrayAccess(operandsParameter, Expression.Constant(parameterIdx)), typeof(double)))))),
                                typeof(long))),
                        Expression.ArrayAccess(operandsParameter, Expression.Constant(parameterIdx++))),
                _ => throw new ArgumentOutOfRangeException()
            });

            var evaluator = Expression.Lambda<Func<long[], long>>(expression, operandsParameter);
            Trace.WriteLine(evaluator.ToString());
            
            evaluators[evaluatorIndex++] = evaluator.Compile();
        }
            
        evaluatorsCache.Add(length, evaluators);
        return evaluators;
    }

    protected override long SolvePartOne() {
        var equations = Parse(Input);

        var sum = 0L;
        foreach (var (value, operands) in equations) {
            var evaluators = GetEvaluators(operands.Length, Operators.Add, Operators.Mul);
            
            if (evaluators.Any(e => value == e(operands)))
                sum += value;
        }
        
        return sum;
    }

    protected override long SolvePartTwo() {
        var equations = Parse(Input);

        var sum = 0L;
        foreach (var (value, operands) in equations) {
            var evaluators = GetEvaluators(operands.Length, Operators.Add, Operators.Mul, Operators.Concat);
            
            if (evaluators.Any(e => value == e(operands)))
                sum += value;
        }
        
        return sum;
    }

    private enum Operators {
        Add, Mul, Concat
    }

    private const string? ExampleInput = @"
190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20
";
    
    [Theory]
    [InlineData(1, 190, new long[] { 10, 19 })]
    [InlineData(2, 3267, new long[] { 81, 40, 27 })]
    [InlineData(0, 83, new long[] { 17, 5 })]
    [InlineData(0, 156, new long[] { 15, 6 })]
    [InlineData(0, 7290, new long[] { 6, 8, 6, 15 })]
    [InlineData(0, 161011, new long[] { 16, 10, 13 })]
    [InlineData(0, 192, new long[] { 17, 8, 14 })]
    [InlineData(0, 21037, new long[] { 9, 7, 18, 13 })]
    [InlineData(1, 292, new long[] { 11, 6, 16, 20 })]
    public void SolvesIndividualPartOneLines(int expectedSolutions, long value, long[] operands) {
        var evaluators = GetEvaluators(operands.Length, Operators.Add, Operators.Mul);
        var actual = evaluators.Count(e => value == e(operands));
        Assert.Equal(expectedSolutions, actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day7(ExampleInput, Output).SolvePartOne();
        Assert.Equal(3749, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day7(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(11387, actual);
    }
}