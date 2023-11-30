namespace AdventOfCode.Core;

class SolverComparer : IComparer<string> {
    public int Compare(string? x, string? y) {
        if (x == null || y == null) return 0;
        
        var defaultComparison = string.Compare(x, y);

        var xparts = x.Split('-', 3);
        var yparts = y.Split('-', 3);
        if (xparts.Length < 2 || yparts.Length < 2 || xparts[1][..3] != "day" || yparts[1][..3] != "day")
            return defaultComparison;

        if (!int.TryParse(xparts[0], out var xevent) || !int.TryParse(yparts[0], out var yevent))
            return defaultComparison;

        if (xevent > yevent) return 1;
        if (xevent < yevent) return -1;
        
        if (!int.TryParse(xparts[1].Substring(3), out var xday) || !int.TryParse(yparts[1].Substring(3), out var yday))
            return defaultComparison;

        if (xday > yday) return 1;
        if (xday < yday) return -1;
        
        return string.Compare(xparts[2], yparts[2]);
    }
}
