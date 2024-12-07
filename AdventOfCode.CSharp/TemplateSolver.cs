﻿using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp;

public class TemplateSolver : Solver {
    public TemplateSolver(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    protected override long SolvePartOne() => 0;

    protected override long SolvePartTwo() => throw new NotImplementedException("Solve part 1 first");

    private const string? ExampleInput = @"
foo
";

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new TemplateSolver(ExampleInput, Output).SolvePartOne();
        Assert.Equal(0, actual);
    }
}