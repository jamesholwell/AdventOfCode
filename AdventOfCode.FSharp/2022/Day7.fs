namespace AdventOfCode.FSharp._2022

open AdventOfCode.Core
open AdventOfCode.FSharp
open Xunit

module Day7 =
    let PathFor cwd = (Seq.rev >> String.concat "/") cwd

    (*

      cd /       []                       [/]          [0]
      cd foo     []                       [/; foo]     [0; 0]
      a.txt 100  []                       [/; foo]     [100; 100]
      b.txt  50  []                       [/; foo]     [150; 150]
      cd ..      [foo=150]                [/]          [150]
      cd bar     [foo=150]                [/; bar]     [150; 0]
      c.txt 25   [foo=150]                [/; bar]     [175; 25]
      **         [foo=150,bar=25]         [/]          [175]
      **         [foo=150,bar=25,/=175]   []           []

    *)

    let Read (output, cwd, sizes) = 
        Shared.SplitBy " " >> function 
        | [| "$"; "cd"; ".."|] -> (PathFor cwd, List.head sizes) :: output, List.tail cwd, List.tail sizes
        | [| "$"; "cd"; dir |] -> output, dir :: cwd, 0 :: sizes
        | [| "$"; _ |]         -> output, cwd, sizes
        | [| "dir"; _ |]       -> output, cwd, sizes
        | [| size; file |]     -> output, cwd, List.map ((+) (int size)) sizes
        | _                    -> output, cwd, sizes

    let rec FinalizeRead = function
        | (output, cwd, size :: sizes) -> FinalizeRead ((PathFor cwd, size) :: output, List.tail cwd, sizes)
        | (output, cwd, []) -> output

    let Parse =
        Shared.Split
        >> Array.fold Read ([], [], [])
        >> FinalizeRead

    let Solve input =
        Parse input
        |> Seq.map snd
        |> Seq.filter ((>) 100000)
        |> Seq.sum

    let SolvePartTwo input =
        let sizes = Parse input
        let freespace = 70000000 - (dict sizes)["/"]

        sizes
        |> Seq.map snd
        |> Seq.filter ((<=) (30000000 - freespace))
        |> Seq.sort
        |> Seq.head

    let exampleInput = @"
$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k
"
    [<Fact>]
    let ``Parses example input`` () =
        let items = Parse exampleInput |> Seq.sortBy fst |> Seq.toArray
        Assert.Equal(("/"   , 48381165), items[0])
        Assert.Equal(("//a"  ,    94853), items[1])
        Assert.Equal(("//a/e",      584), items[2])
        Assert.Equal(("//d"  , 24933642), items[3])


    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(95437, actual)

    [<Fact>]
    let ``Solves Part Two Example`` () =
        let actual = SolvePartTwo exampleInput
        Assert.Equal(24933642, actual)

type Day7(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day7.Solve input |> string
        member this.SolvePartTwo() = Day7.SolvePartTwo input |> string
