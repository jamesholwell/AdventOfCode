namespace AdventOfCode.FSharp._2022

open AdventOfCode.Core
open AdventOfCode.FSharp
open Xunit

module Day4 =
    type Range = {
        From : int;
        To : int;
    }

    (*
     *  "2-4,6-8" -> [ "2-4"; "6-8" ] -> [ [ "2"; "4" ]; [ "6"; "8" ] ] -> [ [ 2; 4 ]; [ 6; 8 ] ] -> [ 2; 4; 6; 8 ]
     *
     *  [ 2; 4; 6; 8 ] -> { From = 2; To = 4 }, { From = 6; To = 8 }
     *)
    let Parse = 
        Shared.Split
        >> Seq.map (Shared.SplitBy "," >> Seq.map (Shared.SplitBy "-" >> Array.map int) >> Array.concat)
        >> Seq.map (fun a -> { From = a[0]; To = a[1] }, { From = a[2]; To = a[3]})

    (*
     *  r1  a...........b
     *  r2      c...d     
     *)
    let FullyContains (r1 : Range) (r2 : Range) = 
        r1.From <= r2.From && r2.To <= r1.To

    (*
     *  r1  a...b
     *  r2    c...d     
     *)
    let Overlaps (r1 : Range) (r2 : Range) = 
        r1.From <= r2.From && r2.From <= r1.To

    let Solve = 
        Parse
        >> Seq.filter (fun (r1, r2) -> FullyContains r1 r2 || FullyContains r2 r1)
        >> Seq.length

    let SolvePartTwo = 
        Parse
        >> Seq.filter (fun (r1, r2) -> Overlaps r1 r2 || Overlaps r2 r1)
        >> Seq.length

    let exampleInput =
        @"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8"

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(2, actual)

    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolvePartTwo exampleInput
        Assert.Equal(4, actual)
        

type Day4(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day4.Solve input |> string
        member this.SolvePartTwo() = Day4.SolvePartTwo input |> string