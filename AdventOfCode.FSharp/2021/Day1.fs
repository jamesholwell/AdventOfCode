namespace AdventOfCode.FSharp._2021

open AdventOfCode.Core
open AdventOfCode.FSharp
open Xunit

module Day1 =
    let CountIncreases =
        Seq.pairwise
        >> Seq.fold (fun acc (l, r) -> if r > l then acc + 1 else acc) 0

    let Solve = Shared.SplitInt >> CountIncreases

    let SolveWindowed =
        Shared.SplitInt
        >> Seq.windowed 3
        >> Seq.map Seq.sum
        >> CountIncreases

    let exampleInput =
        @"199
200
208
210
200
207
240
269
260
263
"

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(7, actual)

    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolveWindowed exampleInput
        Assert.Equal(5, actual)

type Day1(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day1.Solve input
        member this.SolvePartTwo() = Day1.SolveWindowed input
