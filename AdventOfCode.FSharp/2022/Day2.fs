namespace AdventOfCode.FSharp._2022

open AdventOfCode.Core
open AdventOfCode.FSharp
open Xunit

module Day2 =
    type Outcome = Win | Lose | Draw

    type Shape = Rock | Paper | Scissors

    type Round = Shape * Shape

    let Decode = fun s -> 
        match s with
        | 'A' -> Rock
        | 'B' -> Paper
        | 'C' -> Scissors
        | 'X' -> Rock
        | 'Y' -> Paper
        | 'Z' -> Scissors
        | _ -> raise (System.ArgumentOutOfRangeException $"{s} is not a supported hand")

    let ScoreShape = fun s ->
        match s with
        | Rock     -> 1
        | Paper    -> 2
        | Scissors -> 3

    let ScorePlay = fun p ->
        match p with
        | Win  -> 6
        | Lose -> 0
        | Draw -> 3

    let Play = fun (r : Round) ->
        match r with
        | Rock, Scissors   -> Lose
        | Rock, Paper      -> Win
        | Paper, Rock      -> Lose
        | Paper, Scissors  -> Win
        | Scissors, Rock   -> Win
        | Scissors, Paper  -> Lose
        | _                -> Draw

    let Score = fun (r : Round) ->
        ScoreShape (snd r) + ScorePlay (Play r)

    let Solve = 
        Shared.Split 
        >> Seq.map (fun x -> Decode x[0], Decode x[2]) 
        >> Seq.map Score
        >> Seq.sum 

    let exampleInput =
        @"A Y
B X
C Z"

    [<Fact>]
    let ``Scores first example round correctly`` () =
        let actual = Score (Rock, Paper)
        Assert.Equal(8, actual)

    [<Fact>]
    let ``Scores second example round correctly`` () =
        let actual = Score (Paper, Rock)
        Assert.Equal(1, actual)

    [<Fact>]
    let ``Scores third example round correctly`` () =
        let actual = Score (Scissors, Scissors)
        Assert.Equal(6, actual)

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(15, actual)

type Day2(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day2.Solve input |> string
        member this.SolvePartTwo() = failwith "Solve part 1 first"
