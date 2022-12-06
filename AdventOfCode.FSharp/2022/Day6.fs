namespace AdventOfCode.FSharp._2022

open AdventOfCode.Core
open AdventOfCode.FSharp
open Xunit

module Day6 =
    let Solve n = 
        Seq.windowed n
        >> Seq.mapi (fun i x -> n + i, x)
        >> Seq.find (snd >> Seq.distinct >> Seq.length >> (=) n)
        >> fst

    let exampleInput1 = "mjqjpqmgbljsphdztnvjfqwrcgsmlb"    // first marker after character 7
    let exampleInput2 = "bvwbjplbgvbhsrlpgdmjqwftvncz"      // first marker after character 5
    let exampleInput3 = "nppdvjthqldpwncqszvftbrmjlhg"      // first marker after character 6
    let exampleInput4 = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg" // first marker after character 10
    let exampleInput5 = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"  // first marker after character 11

    [<Fact>]
    let ``Solves Part One Example`` () =
        Assert.Equal( 7, Solve 4 exampleInput1)
        Assert.Equal( 5, Solve 4 exampleInput2)
        Assert.Equal( 6, Solve 4 exampleInput3)
        Assert.Equal(10, Solve 4 exampleInput4)
        Assert.Equal(11, Solve 4 exampleInput5)

    let exampleInput6  = "mjqjpqmgbljsphdztnvjfqwrcgsmlb"    // first marker after character 19
    let exampleInput7  = "bvwbjplbgvbhsrlpgdmjqwftvncz"      // first marker after character 23
    let exampleInput8  = "nppdvjthqldpwncqszvftbrmjlhg"      // first marker after character 23
    let exampleInput9  = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg" // first marker after character 29
    let exampleInput10 = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"  // first marker after character 26

    [<Fact>]
    let ``Solves Part Two Example`` () =
        Assert.Equal(19, Solve 14  exampleInput6)
        Assert.Equal(23, Solve 14  exampleInput7)
        Assert.Equal(23, Solve 14  exampleInput8)
        Assert.Equal(29, Solve 14  exampleInput9)
        Assert.Equal(26, Solve 14 exampleInput10)

type Day6(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day6.Solve 4 input |> string
        member this.SolvePartTwo() = Day6.Solve 14 input |> string
