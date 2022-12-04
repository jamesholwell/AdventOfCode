namespace AdventOfCode.FSharp._2022

open AdventOfCode.Shared

module Day4OneLiner =
    type Day4OneLiner(input: string) =
        let input = Input.Read input
        let Solve (input : string) = input.Trim().Replace("-", ",").Split("\n") |> Array.map ((fun s -> s.Split(",")) >> Array.map int) |> Array.filter (function [| a; b; c; d |] -> a <= c && d <= b || c <= a && b <= d | _ -> false) |> Seq.length 
        let Solve2 (input : string) = input.Trim().Replace("-", ",").Split("\n") |> Array.map ((fun s -> s.Split(",")) >> Array.map int) |> Array.filter (function [| a; b; c; d |] -> a <= c && c <= b || c <= a && a <= d | _ -> false) |> Seq.length 

        interface ISolver with
            member this.SolvePartOne() = Solve input |> string
            member this.SolvePartTwo() = Solve2 input |> string