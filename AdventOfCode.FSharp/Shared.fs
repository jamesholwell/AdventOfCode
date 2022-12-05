module AdventOfCode.FSharp.Shared

let SplitBy (b: string) (s: string) =
    s.Replace("\r\n", "\n").Trim('\n').Split(b)

let Split = SplitBy "\n"

let SplitInt = Split >> Seq.map int
