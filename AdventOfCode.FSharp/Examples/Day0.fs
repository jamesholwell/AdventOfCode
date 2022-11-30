namespace AdventOfCode.FSharp.Examples

open AdventOfCode.Core
open AdventOfCode.FSharp.Shared
open Xunit

module Day0 =
    let Solve =
        Seq.fold (fun acc item -> acc * int64 item) 1L

    [<Fact>]
    let ``Solves Example`` () =
        let exampleInput = @"foo"

        let actual = Solve exampleInput
        Assert.Equal(1256742L, actual)

type Day0(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day0.Solve input
        member this.SolvePartTwo() = failwith "NotImplemented"
