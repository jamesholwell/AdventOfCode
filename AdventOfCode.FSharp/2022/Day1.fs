namespace AdventOfCode.FSharp._2022

open AdventOfCode.Shared
open Xunit

module Day1 =
    let Parse = 
        Input.Chunks >> Seq.map Input.Ints

    let Solve = Parse >> Seq.map Seq.sum >> Seq.max

    let SolvePartTwo = Parse >> Seq.map Seq.sum >> Seq.sortByDescending (fun i -> i) >> Seq.take 3 >> Seq.sum

    let exampleInput =
        @"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000"

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(24000, actual)

    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolvePartTwo exampleInput
        Assert.Equal(45000, actual)

type Day1(input: string) =
    let input = Input.Read input
    interface ISolver with
        member this.SolvePartOne() = Day1.Solve input |> string
        member this.SolvePartTwo() = Day1.SolvePartTwo input |> string
