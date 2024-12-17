namespace AdventOfCode.FSharp._2022

open AdventOfCode.Core
open AdventOfCode.FSharp
open System.Collections.Generic
open Xunit

module Day5 =
    type Crate = {
        Height : int;
        Stack : int;
        Label : char;
    }

    type Instruction = {
        Number : int;
        From : int;
        To : int;
    }

    let ParseCrates = 
        Shared.Split
        >> Seq.rev
        >> Seq.skip 1
        >> Seq.mapi (fun h x -> Seq.chunkBySize 4 x |> Seq.mapi (fun s y -> { Height = h; Stack = s + 1; Label = y[1] }))
        >> Seq.concat
        >> Seq.filter (fun c -> c.Label <> ' ')
        >> Seq.sortBy (fun c -> c.Stack, c.Height)

    let ParseInstruction input = 
        match (Shared.SplitBy " " input) with
        | [| "move"; number; "from"; moveFrom; "to"; moveTo |] -> { Number = int number; From = int moveFrom; To = int moveTo }
        | _ -> failwith $"Instruction `{input}` could not be decoded"

    let ParseInstructions = Shared.Split >> Seq.map ParseInstruction

    let Parse input = 
        match (Shared.SplitBy "\n\n" input) with
        | [| crates; instructions |] -> ParseCrates crates, ParseInstructions instructions
        | _ -> failwith $"Input not in correct format"

    let Init input =
        let crates, instructions = Parse input
        let mutable stacks = [| for i in 0..9 -> new Stack<char>() |]
        
        for crate in crates do 
            stacks[crate.Stack].Push(crate.Label)

        stacks, instructions

    let TopOfStacks =
        Array.filter (fun (s : Stack<char>) -> s.Count > 0) 
        >> Array.map (fun (s : Stack<char>) -> s.Peek()) 
        >> System.String

    let Solve input = 
        let stacks, instructions = Init input

        for instruction in instructions do
            for i in 1..instruction.Number do
                stacks[instruction.To].Push(stacks[instruction.From].Pop())

        TopOfStacks stacks

    let SolvePartTwo input = 
        let stacks, instructions = Init input
        let buffer = new Stack<char>()

        for instruction in instructions do
            for i in 1..instruction.Number do
                buffer.Push(stacks[instruction.From].Pop())
            while (buffer.Count > 0) do
                stacks[instruction.To].Push(buffer.Pop())

        TopOfStacks stacks

    let exampleInput = @"
    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2
"
    [<Fact>]
    let ``Decodes example crates as expected`` () =
        let actual = ParseCrates ((Shared.SplitBy "\n\n" exampleInput)[0]) |> Seq.toArray
        Assert.Equal( 6 , actual.Length)
        Assert.Equal( 1 , actual[0].Stack)
        Assert.Equal('Z', actual[0].Label)
        Assert.Equal( 1 , actual[1].Stack)
        Assert.Equal('N', actual[1].Label)
        Assert.Equal( 2 , actual[2].Stack)
        Assert.Equal('M', actual[2].Label)
        Assert.Equal( 2 , actual[3].Stack)
        Assert.Equal('C', actual[3].Label)
        Assert.Equal( 2 , actual[4].Stack)
        Assert.Equal('D', actual[4].Label)
        Assert.Equal( 3 , actual[5].Stack)
        Assert.Equal('P', actual[5].Label)

    [<Fact>]
    let ``Decodes example instructions as expected`` () =
        let actual = ParseInstructions ((Shared.SplitBy "\n\n" exampleInput)[1]) |> Seq.toArray
        Assert.Equal(1, actual[0].Number)
        Assert.Equal(2, actual[0].From)
        Assert.Equal(1, actual[0].To)
        Assert.Equal(3, actual[1].Number)
        Assert.Equal(1, actual[1].From)
        Assert.Equal(3, actual[1].To)
        Assert.Equal(2, actual[2].Number)
        Assert.Equal(2, actual[2].From)
        Assert.Equal(1, actual[2].To)
        Assert.Equal(1, actual[3].Number)
        Assert.Equal(1, actual[3].From)
        Assert.Equal(2, actual[3].To)


    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal("CMZ", actual)

    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolvePartTwo exampleInput
        Assert.Equal("MCD", actual)

type Day5(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day5.Solve input 
        member this.SolvePartTwo() = Day5.SolvePartTwo input