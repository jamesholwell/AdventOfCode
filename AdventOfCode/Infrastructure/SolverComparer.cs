namespace AdventOfCode.Infrastructure;

internal class SolverComparer : IComparer<string> {
    public int Compare(string? x, string? y) {
        if (x == null || y == null) return 0;

        var defaultComparison = string.CompareOrdinal(x, y);

        var xParts = x.Split('-', 3);
        var yParts = y.Split('-', 3);
        if (xParts.Length < 2 || yParts.Length < 2 || xParts[1][..3] != "day" || yParts[1][..3] != "day")
            return defaultComparison;

        if (!int.TryParse(xParts[0], out var xEvent) || !int.TryParse(yParts[0], out var yEvent))
            return defaultComparison;

        if (xEvent > yEvent) return 1;
        if (xEvent < yEvent) return -1;

        if (!int.TryParse(xParts[1].AsSpan(3), out var xDay) || !int.TryParse(yParts[1].AsSpan(3), out var yDay))
            return defaultComparison;

        if (xDay > yDay) return 1;
        if (xDay < yDay) return -1;

        var xSolver = xParts.Length < 3 ? string.Empty : xParts[2];
        var ySolver = yParts.Length < 3 ? string.Empty : yParts[2];
        
        return string.CompareOrdinal(xSolver, ySolver);
    }
}