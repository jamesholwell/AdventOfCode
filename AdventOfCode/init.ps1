function global:aoc {
    Param([parameter(ValueFromRemainingArguments = $true)][string[]]$args)
    dotnet .\bin\Release\net6.0\AdventOfCode.dll @args
}

function global:rebuild {
    dotnet build -c Release
}

rebuild