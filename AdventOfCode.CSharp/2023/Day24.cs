using System.Diagnostics.CodeAnalysis;
using Microsoft.Z3;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2023;

public class Day24 : Solver {
    public Day24(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    public override long SolvePartOne() => SolvePartOne(Parse(Input), 200000000000000L, 400000000000000L, Trace);

    private long SolvePartOne(Hailstone[] stones, long areaMin, long areaMax, ITestOutputHelper trace) {
        var accumulator = 0;

        var pairs = stones.Combinations();
        foreach (var (left, right) in pairs) {
            trace.WriteLine();
            trace.WriteLine($"Hailstone A: {left}");
            trace.WriteLine($"Hailstone B: {right}");

            decimal x1 = left.px;
            decimal x2 = left.px + left.vx;
            decimal y1 = left.py;
            decimal y2 = left.py + left.vy;

            decimal x3 = right.px;
            decimal x4 = right.px + right.vx;
            decimal y3 = right.py;
            decimal y4 = right.py + right.vy;

            var det = ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

            if (det == 0) {
                trace.WriteLine("Hailstones' paths are parallel; they never intersect.");
                continue;
            }

            var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / det;
            var u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / det;

            if (t < 0 && u < 0) {
                trace.WriteLine("Hailstones' paths crossed in the past for both hailstones.");
                continue;
            }
            else if (t < 0) {
                trace.WriteLine("Hailstones' paths crossed in the past for hailstone A.");
                continue;
            }
            else if (u < 0) {
                trace.WriteLine("Hailstones' paths crossed in the past for hailstone B.");
                continue;
            }

            var ix = decimal.Round(left.px + t * left.vx, 3);
            var iy = decimal.Round(left.py + t * left.vy, 3);

            if (areaMin <= ix && ix <= areaMax && areaMin <= iy && iy <= areaMax) {
                trace.WriteLine($"Hailstones' paths will cross inside the test area (at x={ix}, y={iy}).");
                accumulator++;
            }
            else {
                trace.WriteLine($"Hailstones' paths will cross outside the test area (at x={ix}, y={iy}).");
            }
        }
        
        return accumulator;
    }
    
    public override long SolvePartTwo() {
        var stones = Parse(Input);

        /*

         Position of rock and left/right particle are the same at t1/(t1+n) for n in Z
         (1) left.px + left.vx*t1       = rock.px + rock.vx*t1
         (2) right.px + right.vx*(t1+n) = rock.px + rock.vx*(t1+n)

         Subtract (1) from (2):
         (right.px - left.px) + (right.vx - left.vx)*t1 + right.vx*n = rock.vx*n

         Since right.vx == left.vx:
         (right.px - left.px) = (rock.vx - left.vx)*n

         Then:
         (right.px - left.px) - (rock.vx - left.vx)*n = 0

         So:
         (right.px - left.px) = 0 [mod (rock.vx - left.vx)]

         */
        
        // upon inspection, this is a "good enough" possibility set
        var possibleVx = Enumerable.Range(-1000, 2000).ToHashSet();
        var possibleVy = Enumerable.Range(-1000, 2000).ToHashSet();
        var possibleVz = Enumerable.Range(-1000, 2000).ToHashSet();
        
        // search x velocity constraints
        foreach (var pair in stones.GroupBy(s => s.vx).Where(g => g.Count() > 1).SelectMany(g => g.Combinations())) {
            var vx = pair.left.vx;
            var dx = pair.right.px - pair.left.px;
            
            for (var i = -1000; i <= 1000; ++i)
                if (i != vx && dx % (i - vx) != 0)
                    possibleVx.Remove(i);
        }

        Output.WriteLine(possibleVx.Count == 1
            ? $"Found vx = {possibleVx.Single()}"
            : $"Found {possibleVz.Count} possible vxs {string.Join(", ", possibleVx)}");

        // search y velocity constraints
        foreach (var pair in stones.GroupBy(s => s.vy).Where(g => g.Count() > 1).SelectMany(g => g.Combinations())) {
            var vy = pair.left.vy;
            var dy = pair.right.py - pair.left.py;
            
            for (var i = -1000; i <= 1000; ++i)
                if (i != vy && dy % (i - vy) != 0)
                    possibleVy.Remove(i);
        }

        Output.WriteLine(possibleVy.Count == 1
            ? $"Found vy = {possibleVy.Single()}"
            : $"Found {possibleVy.Count} possible vys {string.Join(", ", possibleVy)}");

        // search z velocity constraints
        foreach (var pair in stones.GroupBy(s => s.vz).Where(g => g.Count() > 1).SelectMany(g => g.Combinations())) {
            var vz = pair.left.vz;
            var dz = pair.right.pz - pair.left.pz;
            
            for (var i = -1000; i <= 1000; ++i)
                if (i != vz && dz % (i - vz) != 0)
                    possibleVz.Remove(i);
        }

        Output.WriteLine(possibleVz.Count == 1
            ? $"Found vz = {possibleVz.Single()}"
            : $"Found {possibleVz.Count} possible vzs {string.Join(", ", possibleVz)}");

        // attempt to find the time of first impact in a small range
        foreach (var ttfi in Enumerable.Range(0, 10000)) {
            foreach (var vx in possibleVx.OrderByDescending(x => x)) {
                foreach (var vy in possibleVy.OrderByDescending(y => y)) {
                    foreach (var vz in possibleVz.OrderByDescending(z => z)) {
                        foreach (var fs in stones.OrderBy(s => vx > 0 ? s.px : -s.px)) {
                            var px = fs.px + ttfi * fs.vx - ttfi * vx;
                            var py = fs.py + ttfi * fs.vy - ttfi * vy;
                            var pz = fs.pz + ttfi * fs.vz - ttfi * vz;

                            var isGood = true;

                            foreach (var stone in stones) {
                                if (stone == fs)
                                    continue;

                                // calculate the intersection time - it must be integer!

                                // first part: we can't catch it if it's velocity is the same as ours
                                // sx + sv*t = px + vx*t
                                // (sx - px) + (sv-vx)*t = 0
                                // sx - px = 0 [mod (sv -vx)]

                                if (stone.vx == vx) {
                                    // if the velocity matches, we have to have the same start position or we will never catch it
                                    if (stone.px != px) {
                                        isGood = false;
                                        break;
                                    }
                                }
                                else if ((stone.px - px) % (stone.vx - vx) != 0) {
                                    // we have to catch it at an integer time
                                    isGood = false;
                                    break;
                                }

                                if (stone.vy == vy) {
                                    if (stone.py != py) {
                                        isGood = false;
                                        break;
                                    }
                                }
                                else if ((stone.py - py) % (stone.vy - vy) != 0) {
                                    isGood = false;
                                    break;
                                }

                                if (stone.vz == vz) {
                                    if (stone.pz != pz) {
                                        isGood = false;
                                        break;
                                    }
                                }
                                else if ((stone.pz - pz) % (stone.vz - vz) != 0) {
                                    isGood = false;
                                    break;
                                }

                                // sx + sv*t = px + vx*t
                                // (sx - px) = (vx - sv)*t
                                // t = (sx - px) / (vx - sv)
                                var tx = vx == stone.vx ? 0 : (stone.px - px) / (vx - stone.vx);
                                var ty = vy == stone.vy ? tx : (stone.py - py) / (vy - stone.vy);
                                var tz = vz == stone.vz ? tx : (stone.pz - pz) / (vz - stone.vz);

                                if (tx < 0 || tx != ty || tx != tz) {
                                    isGood = false;
                                    break;
                                }
                            }

                            if (isGood) {
                                Output.WriteLine($"{px}, {py}, {pz} @ {vx}, {vy}, {vz}");
                                return px + py + pz;
                            }
                        }
                    }
                }
            }
        }

        // oof, time to first impact is big: let a solver do the rest of the work
        var ctx = new Context();
        var solver = ctx.MkSolver();
        
        // set up the rock's position variables 
        var mpx = ctx.MkIntConst("px");
        var mpy = ctx.MkIntConst("py");
        var mpz = ctx.MkIntConst("pz");
        
        // assume we found single solutions to the potential velocities
        var rockvx = possibleVx.Single();
        var rockvy = possibleVy.Single();
        var rockvz = possibleVz.Single();
        
        // and set them up as constants in the model
        var cvx = ctx.MkInt(rockvx);
        var cvy = ctx.MkInt(rockvy);
        var cvz = ctx.MkInt(rockvz);
     
        // For each iteration, we will add 3 new equations and one new condition to the solver.
        // We want to find 9 variables (x, y, z, t0, t1, t2) that satisfy all the equations, so a system of 9 equations is enough.
        for (var i = 0; i < 2; i++) {
            var stone = stones[i];
            
            // time to impact variable
            var mt = ctx.MkIntConst($"t{i}");

            // set up the stone position constants
            var mspx = ctx.MkInt(stone.px);
            var mspy = ctx.MkInt(stone.py);
            var mspz = ctx.MkInt(stone.pz);
            
            // set up the stone velocity constants
            var msvx = ctx.MkInt(stone.vx);
            var msvy = ctx.MkInt(stone.vy);
            var msvz = ctx.MkInt(stone.vz);
            
            // set up the rock/time equation
            var mRockPositionEqX = ctx.MkAdd(mpx, ctx.MkMul(mt, cvx)); // rock.px + t * rock.vx
            var mRockPositionEqY = ctx.MkAdd(mpy, ctx.MkMul(mt, cvy)); // rock.py + t * rock.vy
            var mRockPositionEqZ = ctx.MkAdd(mpz, ctx.MkMul(mt, cvz)); // rock.pz + t * rock.vz
     
            // set up the stone/time equation
            var mStonePositionEqX = ctx.MkAdd(mspx, ctx.MkMul(mt, msvx)); // stone.px + t * stone.vx
            var mStonePositionEqY = ctx.MkAdd(mspy, ctx.MkMul(mt, msvy)); // stone.py + t * stone.vy
            var mStonePositionEqZ = ctx.MkAdd(mspz, ctx.MkMul(mt, msvz)); // stone.pz + t * stone.vz
     
            // constrain to positive time
            solver.Add(mt >= 0);
            
            // constrain that positions are equal at the time of impact
            solver.Add(ctx.MkEq(mRockPositionEqX, mStonePositionEqX)); // x + t * vx = px + t * pvx
            solver.Add(ctx.MkEq(mRockPositionEqY, mStonePositionEqY)); // y + t * vy = py + t * pvy
            solver.Add(ctx.MkEq(mRockPositionEqZ, mStonePositionEqZ)); // z + t * vz = pz + t * pvz
        }
     
        solver.Check();
        var model = solver.Model;
     
        var rockpx = Convert.ToInt64(model.Eval(mpx).ToString());
        var rockpy = Convert.ToInt64(model.Eval(mpy).ToString());
        var rockpz = Convert.ToInt64(model.Eval(mpz).ToString());
     
        Output.WriteLine($"p = {rockpx}, {rockpy}, {rockpz}");
        Output.WriteLine($"v = {rockvx}, {rockvy}, {rockvz}");
        
        return rockpx + rockpy + rockpz;
    }

    private Hailstone[] Parse(string input) {
        var lines = Shared.Split(input);
        var stones = new Hailstone[lines.Length];

        for (var i = 0; i < lines.Length; ++i) {
            var parts = lines[i].Replace('@', ',').Split(',', StringSplitOptions.TrimEntries);
            stones[i] = new Hailstone {
                px = long.Parse(parts[0]),
                py = long.Parse(parts[1]),
                pz = long.Parse(parts[2]),
                vx = int.Parse(parts[3]),
                vy = int.Parse(parts[4]),
                vz = int.Parse(parts[5]),
            };
        }

        return stones;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")] // these are their puzzle names
    private record Hailstone {
        public long px;
        public long py;
        public long pz;
        public int vx;
        public int vy;
        public int vz;

        public override string ToString() {
            return $"{px}, {py}, {pz} @ {vx}, {vy}, {vz}";
        }
    }

    private const string? ExampleInput = @"
19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3
";

    private const string ExampleTrace = @"
Hailstone A: 19, 13, 30 @ -2, 1, -2
Hailstone B: 18, 19, 22 @ -1, -1, -2
Hailstones' paths will cross inside the test area (at x=14.333, y=15.333).

Hailstone A: 19, 13, 30 @ -2, 1, -2
Hailstone B: 20, 25, 34 @ -2, -2, -4
Hailstones' paths will cross inside the test area (at x=11.667, y=16.667).

Hailstone A: 19, 13, 30 @ -2, 1, -2
Hailstone B: 12, 31, 28 @ -1, -2, -1
Hailstones' paths will cross outside the test area (at x=6.2, y=19.4).

Hailstone A: 19, 13, 30 @ -2, 1, -2
Hailstone B: 20, 19, 15 @ 1, -5, -3
Hailstones' paths crossed in the past for hailstone A.

Hailstone A: 18, 19, 22 @ -1, -1, -2
Hailstone B: 20, 25, 34 @ -2, -2, -4
Hailstones' paths are parallel; they never intersect.

Hailstone A: 18, 19, 22 @ -1, -1, -2
Hailstone B: 12, 31, 28 @ -1, -2, -1
Hailstones' paths will cross outside the test area (at x=-6, y=-5).

Hailstone A: 18, 19, 22 @ -1, -1, -2
Hailstone B: 20, 19, 15 @ 1, -5, -3
Hailstones' paths crossed in the past for both hailstones.

Hailstone A: 20, 25, 34 @ -2, -2, -4
Hailstone B: 12, 31, 28 @ -1, -2, -1
Hailstones' paths will cross outside the test area (at x=-2, y=3).

Hailstone A: 20, 25, 34 @ -2, -2, -4
Hailstone B: 20, 19, 15 @ 1, -5, -3
Hailstones' paths crossed in the past for hailstone B.

Hailstone A: 12, 31, 28 @ -1, -2, -1
Hailstone B: 20, 19, 15 @ 1, -5, -3
Hailstones' paths crossed in the past for both hailstones.
";
    
    [Fact]
    public void ParsesExampleCorrectly() {
        var actual = Parse(ExampleInput!);

        Assert.Equal(5, actual.Length);
        Assert.Equal(19, actual[0].px);
        Assert.Equal(13, actual[0].py);
        Assert.Equal(30, actual[0].pz);
        Assert.Equal(-2, actual[0].vx);
        Assert.Equal(1, actual[0].vy);
        Assert.Equal(-2, actual[0].vz);
    }

    [Fact]
    public void TracesPartOneExampleCorrectly() {
        var buffer = new StringBuilderOutputHelper();
        SolvePartOne(Parse(ExampleInput!), 7, 27, buffer);
        Assert.Equal(ExampleTrace, buffer.ToString());
    }
    
    [Fact]
    public void SolvesPartOneExample() {
        var actual = SolvePartOne(Parse(ExampleInput!), 7, 27, Trace);
        Assert.Equal(2, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day24(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(47, actual);
    }
}