using AdventOfCode.Core;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.CSharp._2024;

public class Day9 : Solver {
    public Day9(string? input = null, ITestOutputHelper? outputHelper = null) : base(input, outputHelper) { }

    private static int?[] BuildDisk(string? input) {
        var chars = input!.Trim().ToCharArray().Select(c => c - '0').ToArray();
        var length = chars.Sum(c => c);
        var disk = new int?[length];
        var pos = 0;
        var id = 0;
        
        for (var i = 0; i < chars.Length; ++i) {
            for (int j = 0, c = chars[i]; j < c; ++j)
                disk[pos++] = id;

            if (++i < chars.Length)
                pos += chars[i];

            ++id;
        }

        return disk;
    }

    private static long Checksum(int?[] disk) {
        return disk.Select((value, index) => value.HasValue ? (long)value.Value * index : 0L).Sum();
    }

    protected override long SolvePartOne() {
        var disk = BuildDisk(Input);

        var dataPointer = disk.Length - 1;
        var freeSpacePointer = 0;
        while (freeSpacePointer < dataPointer) {
            if (!disk[dataPointer].HasValue) {
                dataPointer--;
                continue;
            }

            while (disk[freeSpacePointer].HasValue)
                freeSpacePointer++;

            if (freeSpacePointer >= dataPointer) 
                break;
            
            disk[freeSpacePointer] = disk[dataPointer];
            disk[dataPointer] = null;
            dataPointer--;
        }
        
        return Checksum(disk);
    }

    protected override long SolvePartTwo()  {
        var disk = BuildDisk(Input);

        var dataPointer = (start: disk.Length - 1, end: -1);
        var seenIds = new HashSet<int>();
        
        while (0 < dataPointer.start) {
            // seek to the next extent
            while (dataPointer.start > 0 && !disk[dataPointer.start].HasValue)
                dataPointer.start--;

            // don't run off the end
            if (dataPointer.start == 0)
                break;

            // don't try to move the same file twice
            var fileId = disk[dataPointer.start]!.Value;
            if (!seenIds.Add(fileId)) {
                dataPointer.start--;
                continue;
            }

            // seek to the beginning of the extent
            dataPointer.end = dataPointer.start;
            while (dataPointer.start > 0 && disk[dataPointer.start - 1] == fileId)
                dataPointer.start--;

            // search for free space
            var requiredSpace = dataPointer.end - dataPointer.start;
            var freeSpacePointer = 0;
            for (var i = 0; i < dataPointer.start; ++i) {
                if (disk[i].HasValue) {
                    freeSpacePointer = i + 1;
                    continue;
                }

                if (i - freeSpacePointer < requiredSpace) 
                    continue;
                
                for (var j = 0; j <= requiredSpace; ++j) {
                    disk[freeSpacePointer + j] = disk[dataPointer.start + j];
                    disk[dataPointer.start + j] = null;
                }
                    
                break;
            }
            
            // reset the extent
            dataPointer.start--;
        }
        
        return Checksum(disk);
    }

    private const string? ExampleInput = @"
2333133121414131402
";

    [Fact]
    public void SolvesSimpleExample() {
        var actual = new Day9("12345", Output).SolvePartOne();
        Assert.Equal(60, actual);
    }

    [Fact]
    public void SolvesPartOneExample() {
        var actual = new Day9(ExampleInput, Output).SolvePartOne();
        Assert.Equal(1928, actual);
    }
    
    [Fact]
    public void SolvesPartTwoExample() {
        var actual = new Day9(ExampleInput, Output).SolvePartTwo();
        Assert.Equal(2858, actual);
    }
}