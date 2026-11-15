# aoc               - runs the current puzzle solver (supports pt2 and --trace arguments)
# aoc [args]        - all other commands (e.g. start, stage, finish) are passed to the main project
# aoc buildself     - asks dotnet to rebuild the main aoc binary

function global:aoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$arguments)

    # first check for default usage by testing for unsupported arguments
    $disallowedArgs = $arguments | where { -not (@("part2", "pt2", "--trace") -contains $_) }
    if ($disallowedArgs.Count -eq 0) {
        # no unsupported arguments, run the current solver project only
        dotnet build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly ./AdventOfCode.Today && dotnet run --no-build --project ./AdventOfCode.Today -- @arguments
        return;
    }

    # now check for `aoc buildself` (handled directly in powershell)
    if ($arguments.Count -eq 1 -and $arguments[0] -eq "buildself") {
        dotnet build AdventOfCode -c Release --property WarningLevel=0 -v n
        return;
    }
    
    # ensure the binary is available, or build (and quit on error)
    $tfm = "net10.0"
    $dll = "./AdventOfCode/bin/Release/$tfm/AdventOfCode.dll"
    if (-not (test-path $dll -type leaf)) {
        dotnet build AdventOfCode -c Release --property WarningLevel=0 -v q
        if ($LASTEXITCODE -ne 0) { return }
    }
     
    # delegate all other commands to the main project
    dotnet $dll @arguments
}
