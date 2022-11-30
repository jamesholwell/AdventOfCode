namespace AdventOfCode.FSharp._2021

open AdventOfCode.Core
open AdventOfCode.FSharp
open Xunit

module Day2 =
    let (|Prefix|_|) (p: string) (s: string) =
        if s.StartsWith p then
            Some(s.Substring(p.Length) |> int)
        else
            None

    let UpdateVector =
        fun (h, d) s ->
            match s with
            | Prefix "forward" x -> (h + x, d)
            | Prefix "down"    x -> (h, d + x)
            | Prefix "up"      x -> (h, d - x)
            | _                  -> (h, d)

    let UpdateAimedVector =
        fun (h, d, a) s ->
            match s with
            | Prefix "forward" x -> (h + x, d + x * a, a)
            | Prefix "down"    x -> (h, d, a + x)
            | Prefix "up"      x -> (h, d, a - x)
            | _                  -> (h, d, a)
        
    let GetMagnitude = fun (h, d) -> h * d

    let GetMagnitude3 (h, d, _) = GetMagnitude (h, d)

    let Solve =
        Shared.Split
        >> Seq.fold UpdateVector (0, 0)
        >> GetMagnitude

    let SolveAimed =
        Shared.Split
        >> Seq.fold UpdateAimedVector (0, 0, 0)
        >> GetMagnitude3

    let exampleInput = @"
forward 5
down 5
forward 8
up 3
down 8
forward 2
"

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(150, actual)
        
    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolveAimed exampleInput
        Assert.Equal(900, actual)
        
type Day2(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day2.Solve input
        member this.SolvePartTwo() = Day2.SolveAimed input
