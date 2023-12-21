namespace AdventOfCode.CSharp;

public class PointGrid<T> where T : struct {
    private readonly char[,] grid;
    private readonly int height;
    private readonly T[] points;
    private readonly int width;
    private readonly Func<T, int> xf;
    private readonly int xOffset;
    private readonly Func<T, int> yf;
    private readonly int yOffset;

    public PointGrid(IEnumerable<T> enumerable, Func<T, int> xAccessor, Func<T, int> yAccessor,
        Func<T, char>? valueAccessor = null, char defaultValue = ' ') {
        points = enumerable as T[] ?? enumerable.ToArray();
        xf = xAccessor;
        yf = yAccessor;

        xOffset = points.Min(xAccessor);
        width = 1 + points.Max(xAccessor) - xOffset;

        yOffset = points.Min(yAccessor);
        height = 1 + points.Max(yAccessor) - yOffset;

        grid = new char[height, width];

        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            grid[y, x] = defaultValue;

        if (valueAccessor != null)
            foreach (var point in points)
                grid[yf(point), xf(point)] = valueAccessor(point);
    }

    public void Initialize(char c) => grid.Initialize(c);
    
    public void Fill(Func<int, int, char> selector) {
        for (var y = 0; y < height; ++y)
        for (var x = 0; x < width; ++x)
            grid[y, x] = selector(x + xOffset, y + yOffset);
    }
    
    public void Set(T point, char c) => grid[yf(point) - yOffset, xf(point) - xOffset] = c;

    public void Set(int pointDomainX, int pointDomainY, char c) => grid[pointDomainY - yOffset, pointDomainX - xOffset] = c;

    public string Render() => grid.Render();
}