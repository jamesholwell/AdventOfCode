namespace AdventOfCode.FSharp._2022

open AdventOfCode.Shared
open Xunit

module Day2OneLiner = 
    let Solve = 
        Input.Lines
        >> Seq.map (fun (s: string) -> 6 - 3 * ((4 + ((((int)'Y' - (int)s[2]) - ((int)'B' - (int)s[0])) % 3 + 3) % 3) % 3) + (int)s[2] - (int)'W')
        >> Seq.sum 
        >> int64

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve Day2.exampleInput
        Assert.Equal(15L, actual)

type Day2OneLiner(input: string) =
    let input = Input.Read input
    interface ISolver with
        member this.SolvePartOne() = Day2OneLiner.Solve input |> string
        member this.SolvePartTwo() = failwith "Solve part 1 first"