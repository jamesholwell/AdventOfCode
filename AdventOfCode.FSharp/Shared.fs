module AdventOfCode.FSharp.Shared

open System

let NormalizeLineEndings (s: string) =
    s.Replace("\r\n", "\n")

let Split (s: string) =
    NormalizeLineEndings(s).Split("\n", StringSplitOptions.RemoveEmptyEntries)

let SplitBy (b: string) (s: string) =
    NormalizeLineEndings(s).Split(b, StringSplitOptions.RemoveEmptyEntries)

let SplitInt = Split >> Seq.map int
