namespace AdventOfCode.Infrastructure;

internal static class ConsoleExtensions {
    extension(Console) {
        internal static void WriteMessage(string message) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        internal static void WriteError(string errorMessage) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(errorMessage);
            Console.ResetColor();

            Console.WriteLine(string.Empty);
        }
    }
}