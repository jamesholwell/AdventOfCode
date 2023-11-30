namespace AdventOfCode.FSharp._2022

open AdventOfCode.Core
open AdventOfCode.FSharp
open System
open System.Linq
open Xunit

module Day3 =
    let priorities = seq { seq { 'a' .. 'z' } ; seq { 'A' .. 'Z' } } |> Seq.concat |> Seq.mapi (fun i x -> (x, 1 + i)) |> dict

    let Solve = 
        Shared.Split 
        >> Seq.map (fun s -> Seq.find (fun (ss : char) -> s[(s.Length / 2)..(s.Length)].Contains(ss)) s[0..(s.Length / 2)])
        >> Seq.map (fun c -> priorities[c])
        >> Seq.sum

    let SolvePartTwo = 
        Shared.Split 
        >> Seq.chunkBySize 3
        >> Seq.map (Seq.reduce (fun a b -> String.Concat(a.Intersect(b))))
        >> Seq.map (fun c -> priorities[Seq.head c])
        >> Seq.sum

    let exampleInput =
        @"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw"

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(157, actual)

    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolvePartTwo exampleInput
        Assert.Equal(70, actual)
        

type Day3(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day3.Solve input |> string
        member this.SolvePartTwo() = Day3.SolvePartTwo input |> string
