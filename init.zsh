#!/usr/bin/env zsh
# aoc               - runs the current puzzle solver (supports pt2 and --trace arguments)
# aoc build         - asks dotnet to release-build the aoc binary
# aoc fast [args]   - asks dotnet to run the release-build binary
# aoc [args]        - all other commands (e.g. start, stage, finish) are passed to the main project

buildaoc() {
  dotnet build AdventOfCode -c Release --property WarningLevel=0 -v n
}

runaoc() {
  local tfm="net10.0"
  dotnet "./AdventOfCode/bin/Release/$tfm/AdventOfCode.dll" "$@"
}

aoc() {
  local args=("$@")

  # first check for default usage by testing for unsupported arguments
  local today_allowed=("part2" "pt2" "--trace")
  local disallowedArgs=(${@:|today_allowed})
  if (( ${#disallowedArgs} == 0 )); then
    dotnet build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly ./AdventOfCode.Today && dotnet run --no-build --project ./AdventOfCode.Today -- "${args[@]}"
    return
  fi

  # now check for `aoc build` (handled directly in zsh)
  if [[ ${#args[@]} -eq 1 && ${args[1]} == "build" ]]; then
    buildaoc
    return
  fi

  # now check for `aoc fast` (handled directly in zsh)
  if [[ ${#args[@]} -ge 1 && ${args[1]} == "fast" ]]; then
    runaoc "${args[@]:1}"
    return
  fi

  # all other commands are delegated to the main project
  dotnet build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly ./AdventOfCode && dotnet run --no-build --project ./AdventOfCode -- "${args[@]}"
}