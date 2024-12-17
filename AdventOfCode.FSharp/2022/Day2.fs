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

    let DecodeOutcome = fun s -> 
        match s with
        | 'X' -> Lose
        | 'Y' -> Draw
        | 'Z' -> Win
        | _ -> raise (System.ArgumentOutOfRangeException $"{s} is not a supported outcome")

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

    let FigureOut = fun (r : Shape * Outcome) ->
        match r with
        | Rock, Win      -> Paper
        | Rock, Lose     -> Scissors
        | Paper, Win     -> Scissors
        | Paper, Lose    -> Rock
        | Scissors, Win  -> Rock
        | Scissors, Lose -> Paper
        | _              -> fst r

    let Score = fun (r : Round) ->
        ScoreShape (snd r) + ScorePlay (Play r)

    let Solve = 
        Shared.Split 
        >> Seq.map (fun x -> Decode x[0], Decode x[2]) 
        >> Seq.map Score
        >> Seq.sum 

    let SolvePartTwo = 
        Shared.Split 
        >> Seq.map (fun x -> Decode x[0], DecodeOutcome x[2]) 
        >> Seq.map (fun x -> fst x, FigureOut x)
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
        
    [<Fact>]
    let ``Figures out first example round correctly`` () =
        let actual = FigureOut (Rock, Draw)
        Assert.Equal(Rock, actual)

    [<Fact>]
    let ``Figures out second example round correctly`` () =
        let actual = FigureOut (Paper, Lose)
        Assert.Equal(Rock, actual)

    [<Fact>]
    let ``Figures out third example round correctly`` () =
        let actual = FigureOut (Scissors, Win)
        Assert.Equal(Rock, actual)

    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolvePartTwo exampleInput
        Assert.Equal(12, actual)

module Day2OneLiner = 
    let Solve = 
        Shared.Split
        >> Seq.map (fun (s: string) -> 6 - 3 * ((4 + ((((int)'Y' - (int)s[2]) - ((int)'B' - (int)s[0])) % 3 + 3) % 3) % 3) + (int)s[2] - (int)'W')
        >> Seq.sum 
        >> int64

    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve Day2.exampleInput
        Assert.Equal(15L, actual)
        

type Day2(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day2.Solve input |> string
        member this.SolvePartTwo() = Day2.SolvePartTwo input |> string

type Day2OneLiner(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day2OneLiner.Solve input |> string
        member this.SolvePartTwo() = failwith "Solve part 1 first"
