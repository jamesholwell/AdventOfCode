function global:aoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$args)
    dotnet run --property WarningLevel=0 --project .\AdventOfCode @args
}

function global:buildaoc {
    dotnet build AdventOfCode -c Release --property WarningLevel=0
}

function global:runaoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$args)
    dotnet .\AdventOfCode\bin\Release\net6.0\AdventOfCode.dll @args
}