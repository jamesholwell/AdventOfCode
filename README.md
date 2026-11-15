# Advent Of Code

I love puzzles, and although I first learned about [Advent of Code](https://adventofcode.com/) in 2017*
I didn't pay it much attention until much later when [George Seeger](https://github.com/GeorgeSeeger) 
recommended some of the more involved challenges (like [2016 Day 11](https://adventofcode.com/2016/day/11)),
and we started playing along as a team at work.

After solving my first few puzzles, I quickly realised that I wanted a common workflow and some helpers
to reduce the boilerplate required each day (and to keep [Pete Forrest](https://github.com/Itzalive)
from running away with all the stars). With this as my 'good excuse' I set about assembling something 
that would: 
* support writing solutions in either C# or F#; 
* have testing as a first-class part of the development loop;
* make it easy to switch from the debugger to real inputs on the console.

After completing 2022 and 2023 using the framework, I realised I missed the immediacy of working on 
just a single solver at a time: the rapid iteration of a small project felt delightful, and I missed
having the full library of past solutions and algorithms available in my current branch. I introduced 
the 'today' project and built some automations to manage the source code as it moved around. 


\* *I am often reminded of the premise of the very first puzzle that year 
([Inverse Captcha](https://adventofcode.com/2017/day/1)) in which the test proves
you are **not** human: feels somewhat prescient today.*

### Getting Started

Run `./init.ps1` or `source init.zsh` to install the `aoc` alias.

#### Basic Usage

* `aoc` runs the current solver  
  > input files are resolved by convention from `inputs/{event}/day{n}.txt`
* use `aoc pt2` for part 2, and add `--trace` to enable debugging output

#### Workflow

* `aoc start day1 2015` initialises a new solver.  
  The `event` and `day` arguments may be given in any order.  
  During December, these are set to the current day and year by default.  
  Specify a suffix to create variants of a solver e.g. `aoc start day1 2015 linq`

  > Don't forget to download your puzzle input (no automation here, yet).

* `aoc stage` copies the current solver to the library.  
  Do this once you've completed part 1, or whenever you want to make a version control checkpoint.  
  Will not overwrite an existing file by default, use `--force` to override.


* `aoc finish` moves the current solver to the library and deletes the original.


* `aoc next` finishes the current puzzle and starts the next day.

#### Solver Library

* `aoc build` rebuilds the library of solvers.  
   Normally this happens automatically as part of `finish` or `next`.


* `aoc list` lists all the currently discovered solvers


* `aoc 2015 day1` runs a solver from the archive.  
  Use `--bench` to compare different solvers, or specify the suffix to pick one in particular.



