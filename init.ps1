function global:aoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$args)
    dotnet run --project .\AdventOfCode @args
}

function global:buildaoc {
    dotnet build AdventOfCode -c Release
}

function global:runaoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$args)
    dotnet .\AdventOfCode\bin\Release\net6.0\AdventOfCode.dll @args
}