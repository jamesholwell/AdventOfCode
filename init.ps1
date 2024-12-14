function global:aoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$args)

    if ($args.count -eq 0 -or $args[0] -eq "pt2") {
        dotnet run --property WarningLevel=0 --project .\AdventOfCode.Today @args        
        return;
    }
    
    dotnet run --property WarningLevel=0 --project .\AdventOfCode @args
}

function global:buildaoc {
    dotnet build AdventOfCode -c Release --property WarningLevel=0
}

function global:runaoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$args)
    dotnet .\AdventOfCode\bin\Release\net8.0\AdventOfCode.dll @args
}