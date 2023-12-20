namespace AdventOfCode.CSharp;

public static class Maths {
    // ReSharper disable once InconsistentNaming - this is the mathematical name
    public static long lcm(long a, long b) {
        return (a / gcf(a, b)) * b;
    }

    // ReSharper disable once InconsistentNaming - this is the mathematical name
    private static long gcf(long a, long b) {
        while (b != 0) {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }
}