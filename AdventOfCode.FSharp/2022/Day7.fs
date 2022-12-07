namespace AdventOfCode.FSharp._2022

open AdventOfCode.Core
open AdventOfCode.FSharp
open System;
open Xunit

module Day7 =
    let PathFor cwd = "/" + (Seq.rev >> String.concat "/") cwd

    let Read (cwd, items) = 
        Shared.SplitBy " " >> function 
        | [| "$"; "cd"; "/" |] -> []            , items
        | [| "$"; "cd"; ".."|] -> List.tail cwd , items
        | [| "$"; "cd"; dir |] -> dir :: cwd    , items
        | [| "$"; "ls" |]      -> cwd           , items
        | [| "dir"; folder |]  -> cwd           , (PathFor cwd, 0, folder) :: items
        | [| size; file |]     -> cwd           , (PathFor cwd, int size, file) :: items
        | _                    -> cwd           , items

    let Parse =
        Shared.Split
        >> Seq.fold Read ([], [])
        >> snd

    let Path (path, _, _)= path

    let Size (_, size, _) = size

    let PathMatches (cwd : string) (path : string, _, _) = path.StartsWith(cwd)

    let DirectorySizes files = 
        files 
        |> Seq.map Path 
        |> Seq.distinct
        |> Seq.map (fun path -> path, files |> Seq.filter (PathMatches path) |> Seq.sumBy Size)

    let Solve input = 
        let files = Parse input
        DirectorySizes files 
         |> Seq.map snd
         |> Seq.filter ((>) 100000)
         |> Seq.sum


    let SolvePartTwo = fun _ -> failwith "Solve part 1 first"

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
        let items = Parse exampleInput |> Seq.filter (fun (p, s, f) -> s > 0) |> Seq.rev |> Seq.toArray
        Assert.Equal(("/"   , 14848514, "b.txt"), items[0])
        Assert.Equal(("/"   ,  8504156, "c.dat"), items[1])
        Assert.Equal(("/a"  ,    29116, "f"    ), items[2])
        Assert.Equal(("/a"  ,     2557, "g"    ), items[3])
        Assert.Equal(("/a"  ,    62596, "h.lst"), items[4])
        Assert.Equal(("/a/e",      584, "i"    ), items[5])
        Assert.Equal(("/d"  ,  4060174, "j"    ), items[6])
        Assert.Equal(("/d"  ,  8033020, "d.log"), items[7])
        Assert.Equal(("/d"  ,  5626152, "d.ext"), items[8])
        Assert.Equal(("/d"  ,  7214296, "k"    ), items[9])

    [<Fact>]
    let ``Calculates sizes for example input`` () =
        let items = Parse exampleInput |> DirectorySizes |> Seq.sortBy fst |> Seq.toArray
        Assert.Equal(("/"   , 48381165), items[0])
        Assert.Equal(("/a"  ,    94853), items[1])
        Assert.Equal(("/a/e",      584), items[2])
        Assert.Equal(("/d"  , 24933642), items[3])


    [<Fact>]
    let ``Solves Part One Example`` () =
        let actual = Solve exampleInput
        Assert.Equal(95437, actual)

type Day7(input: string) =
    interface ISolver with
        member this.SolvePartOne() = Day7.Solve input |> string
        member this.SolvePartTwo() = Day7.SolvePartTwo input |> string
