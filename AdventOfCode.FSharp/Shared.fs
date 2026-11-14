module AdventOfCode.FSharp.Shared

let SplitBy (b: string) (s: string) = s.Split(b)

let Split = SplitBy "\n"

let SplitInt = Split >> Seq.map int
