// ReSharper disable MemberCanBePrivate.Global - helpers are used downstream
// ReSharper disable UnusedMember.Global - helpers are used downstream
// ReSharper disable UnusedType.Global - helpers are used downstream
namespace AdventOfCode.Core;

public static class Mathematics {
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

    // ReSharper disable once InconsistentNaming - this is the mathematical name
    public static int mod(int a, int n) => wrap(a, n);

    // ReSharper disable once InconsistentNaming - this is the mathematical name
    public static int wrap(int a, int n) => ((a % n) + n) % n;
}