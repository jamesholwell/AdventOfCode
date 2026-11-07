# Advent Of Code

I love puzzles, and although I first learned about [Advent of Code](https://adventofcode.com/) in 2017*
I didn't pay it much attention until much later when [George Seeger](https://github.com/GeorgeSeeger) 
recommended some of the more involved challenges (like [2016 Day 11](https://adventofcode.com/2016/day/11)),
and we started playing along as a team at work.

After solving my first few puzzles, I quickly realised that I wanted a common workflow and some helpers
to reduce the boilerplate required each day. With puzzle-solving as my 'good excuse' I set about assembling
some code that would: allow writing solutions in either C# or F#; offered a decent developer experience
for testing against the examples; and made it easy to switching between the console and the debugger.

After completing a couple of events, I realised that I missed the immediacy of working on a single
puzzle at a time: the (very small) extra build time was noticeable when iterating rapidly. Introducing
a separate project runner with verbs to start, stage and finish a puzzle felt quite natural. 


\* *I am often reminded of the premise of the very first puzzle that year
([Inverse Captcha](https://adventofcode.com/2017/day/1)) in which the test proves
you are **not** human: feels somewhat prescient in today in the age of agentic AI.*

### Getting Started

Run `./init.ps1` or `source init.zsh` to install the `aoc` alias.

#### Basic Usage

* `aoc` runs the current solver  
  > input files are resolved by convention from `inputs/{event}/day{n}.txt`
* use `aoc pt2` for part 2, and add `--trace` to enable debugging output

Normally the default `dotnet run` behaviour of `aoc` is sufficient, but you can use `aoc build`
to release-build the library and then `aoc fast` to run it directly (for better performance, maybe?).

#### Workflow

* `aoc start day1 2015` initialises a new solver.  
  The `event` and `day` arguments may be given in any order.  
  During December, these are set to the current day and year by default.

  > Don't forget to download your puzzle input (no automation here, yet).

* `aoc stage` copies the current solver to the library.  
  Do this once you've completed part 1, or whenever you want to make a version control checkpoint.  
  Will not overwrite an existing file by default, use `--force` to override.


* `aoc finish` moves the current solver to the library and deletes the original.


* `aoc next` finishes the current puzzle and starts the next day.
