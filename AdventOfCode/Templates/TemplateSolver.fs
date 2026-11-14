namespace AdventOfCode.FSharp

open AdventOfCode.Shared
open Xunit

module TemplateSolver =
    let Solve =
        Input.Read
        >> fun _ -> 0

    let SolvePartTwo = fun _ -> failwith "Solve part 1 first"

    let exampleInput = @"
foo
"
    
    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(0, actual)

type TemplateSolver(input: string) =
    interface ISolver with
        member this.SolvePartOne() = TemplateSolver.Solve input |> string
        member this.SolvePartTwo() = TemplateSolver.SolvePartTwo input |> string