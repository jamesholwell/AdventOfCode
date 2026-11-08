# aoc               - runs the current puzzle solver (supports pt2 and --trace arguments)
# aoc build         - asks dotnet to release-build the aoc binary
# aoc fast [args]   - asks dotnet to run the release-build binary
# aoc [args]        - all other commands (e.g. start, stage, finish) are passed to the main project

function global:build-aoc {
    dotnet build AdventOfCode -c Release --property WarningLevel=0 -v n
}

function global:run-aoc {
    Param([string[]]$arguments)
    $tfm = "net10.0"
    dotnet ./AdventOfCode/bin/Release/$tfm/AdventOfCode.dll @arguments
}

function global:aoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$arguments)

    # first check for default usage by testing for unsupported arguments
    $disallowedArgs = $arguments | where { -not (@("part2", "pt2", "--trace") -contains $_) }
    if ($disallowedArgs.Count -eq 0) {
        # no unsupported arguments, run the current solver project only
        dotnet build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly ./AdventOfCode.Today && dotnet run --no-build --project ./AdventOfCode.Today -- @arguments
        return;
    }

    # now check for `aoc build` (handled directly in powershell)
    if ($arguments.Count -eq 1 -and $arguments[0] -eq "build") {
        build-aoc
        return;
    }

    # now check for `aoc fast` (handled directly in powershell)
    if ($arguments.Count -ge 1 -and $arguments[0] -eq "fast") {
        $rest = $arguments.Count -gt 1 ? $arguments[1..($arguments.Count - 1)] : @()
        run-aoc $rest
        return;
    }
    
    # all other commands are delegated to the main project
    dotnet build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly ./AdventOfCode && dotnet run --no-build --project ./AdventOfCode -- @arguments
}
