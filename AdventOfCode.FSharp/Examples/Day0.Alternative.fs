namespace AdventOfCode.FSharp.Examples

open AdventOfCode.Core
open Xunit

module Day0Alternative =
    let Solve (s: string) =
        let rec inner l =
            match l with
            | head :: tail -> head * inner tail
            | [] -> 1L

        s |> Seq.map int64 |> Seq.toList |> inner

    [<Fact>]
    let ``Solves Example`` () =
        let exampleInput = @"foo"

        let actual = Solve exampleInput
        Assert.Equal(1256742L, actual)

[<Solver("day0", "sprintf")>]
type Day0Alternative(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day0.Solve input |> string
        member this.SolvePartTwo() = failwith "NotImplemented"
