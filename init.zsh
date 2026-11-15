#!/usr/bin/env zsh
# aoc               - runs the current puzzle solver (supports pt2 and --trace arguments)
# aoc [args]        - all other commands (e.g. start, stage, finish) are passed to the main project
# aoc buildself     - asks dotnet to rebuild the main aoc binary

aoc() {
  # first check for default usage by testing for unsupported arguments
  local today_allowed=("part2" "pt2" "--trace")
  local disallowedArgs=(${@:|today_allowed})
  if (( ${#disallowedArgs} == 0 )); then
    dotnet build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly ./AdventOfCode.Today && dotnet run --no-build --project ./AdventOfCode.Today -- "$@"
    return
  fi

  # now check for `aoc buildself` (handled directly in zsh)
  if [[ ${#@} -eq 1 && ${@[1]} == "buildself" ]]; then
    dotnet build AdventOfCode -c Release --property WarningLevel=0 -v n  
    return
  fi

  # ensure the binary is available, or build (and quit on error)
  local tfm="net10.0"
  local exe="./AdventOfCode/bin/Release/$tfm/AdventOfCode"
  if [[ ! -f "$exe" ]]; then
    dotnet build AdventOfCode -c Release --property WarningLevel=0 -v q || return $?
  fi
    
  # delegate all other commands to the main project
  "$exe" "$@"
}