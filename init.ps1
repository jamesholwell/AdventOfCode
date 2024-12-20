function global:aoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$args)

    $disallowedargs = $args | where { -not (@("pt2", "--trace") -contains $_) }
    if ($disallowedargs.length -eq 0) {
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