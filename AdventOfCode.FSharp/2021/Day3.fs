namespace AdventOfCode.FSharp._2021

open AdventOfCode.Core
open AdventOfCode.FSharp
open Xunit

module Day3 =
    let flip = fun x -> 1 - x

    let mostCommonBit offset (entries: int[][]) : int =
        entries
        |> Array.map (fun e -> if e[offset] = 1 then 1 else -1)
        |> Array.sum
        |> fun s -> if s >= 0 then 1 else 0

    let leastCommonBit o = mostCommonBit o >> flip

    let decode s = match s with | '1' -> 1 | _ -> 0

    let decodeLines = Array.map (Seq.map decode >> Seq.toArray)

    let asDecimal = Seq.reduce (fun a b -> a * 2 + b)

    let SolvePartOne input =
        let entries = Shared.Split input |> decodeLines

        let bits =
            seq { for i in 0 .. (entries[0].Length - 1) -> mostCommonBit i entries }

        let gamma = bits |> asDecimal
        let epsilon = Seq.map flip bits |> asDecimal
        gamma * epsilon

    let SolvePartTwo input =
        let entries = Shared.Split input |> decodeLines

        let rec sieve selector offset entries =
            match entries with
            | [| entry |] -> entry |> asDecimal
            | _ ->
                let mcb = selector offset entries
                sieve selector (offset + 1) (Array.filter (fun (c: int []) -> c[offset] = mcb) entries)

        let oxyReading = sieve mostCommonBit 0 entries
        let co2Reading = sieve leastCommonBit 0 entries
        oxyReading * co2Reading

    let exampleInput = @"
00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010
"

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = SolvePartOne exampleInput
        Assert.Equal(198, actual)

    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolvePartTwo exampleInput
        Assert.Equal(230, actual)

type Day3(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day3.SolvePartOne input
        member this.SolvePartTwo() = Day3.SolvePartTwo input
