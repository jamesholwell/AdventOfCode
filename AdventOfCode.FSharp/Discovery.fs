namespace AdventOfCode.FSharp

open AdventOfCode.Shared

type Discovery() =
    interface ISolverDiscovery with
        member _.Prefix = "fs"
