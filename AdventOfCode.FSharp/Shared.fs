module AdventOfCode.FSharp.Shared

open System

let Split (s: string) =
    s
        .Replace("\r\n", "\n")
        .Split("\n", StringSplitOptions.RemoveEmptyEntries)

let SplitInt = Split >> Seq.map int
