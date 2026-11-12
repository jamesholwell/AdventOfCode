using AdventOfCode.Infrastructure;
using AdventOfCode.Shared;

namespace AdventOfCode.Services;

internal static class Workflow {
    /// <summary>
    ///     Initialise a new solver in the today project
    /// </summary>
    public static bool TryInitialiseSolver(int @event, int day, string? suffix, bool isForced, out string targetPath) {
        if (@event is < 2015 or > 2099) throw new ArgumentOutOfRangeException(nameof(@event));
        if (day is < 1 or > 25) throw new ArgumentOutOfRangeException(nameof(day));
        if (suffix != null && !Puzzle.SuffixPattern.IsMatch(suffix))
            throw new ArgumentOutOfRangeException(nameof(suffix));

        var templatePath = Path.Combine(GetBasePath("AdventOfCode.CSharp"), "TemplateSolver.cs");
        if (!File.Exists(templatePath))
            throw new InvalidOperationException($"Template path not found (tried: {templatePath}");

        var todayBasePath = GetBasePath("AdventOfCode.Today");
        var eventBasePath = Path.Combine(todayBasePath, @event.ToString());
        if (!Directory.Exists(eventBasePath))
            Directory.CreateDirectory(eventBasePath);

        targetPath = Path.Combine(eventBasePath, $"Day{day}{suffix}.cs");
        if (File.Exists(targetPath)) {
            if (!isForced)
                return false;

            CreateBackupFile(targetPath);
        }

        // rewrite the template contents
        var source = File.ReadAllText(templatePath);
        source = source
            .Replace("namespace AdventOfCode.CSharp;", $"namespace AdventOfCode.Today._{@event};")
            .Replace("TemplateSolver", $"Day{day}{suffix}");

        // write the solver
        File.WriteAllText(targetPath, source);
        return true;
    }

    private static void CreateBackupFile(string targetPath) {
        var backupPath = Path.Combine(Path.GetDirectoryName(targetPath)!,
            $"{Path.GetFileNameWithoutExtension(targetPath)}_{Guid.NewGuid().ToString()[..8]}.txt");
        File.Move(targetPath, backupPath);
    }

    /// <summary>
    ///     Try to locate the first candidate solver source file in the today project
    /// </summary>
    public static bool TryLocateSolver(out string? path) {
        var basePath = GetBasePath("AdventOfCode.Today");
        var candidates = new List<string>();

        foreach (var directory in Directory.EnumerateDirectories(basePath)) {
            var directoryName = Path.GetFileName(directory);
            if (directoryName.Length != 4 || !int.TryParse(directoryName, out var @event) || @event is < 2015 or > 2099)
                continue;

            var files = Directory.EnumerateFiles(directory, "*.cs");

            candidates.AddRange(files.Where(f => Puzzle.DayPattern.IsMatch(Path.GetFileNameWithoutExtension(f))));
        }

        switch (candidates.Count) {
            case > 1:
                throw new AmbiguousSolverException([.. candidates]);

            case 1:
                path = candidates[0];
                return true;

            default:
                path = null;
                return false;
        }
    }

    /// <summary>
    ///     Copy a solver to the library, rewriting the namespace in flight
    /// </summary>
    public static bool TryStageFile(string sourcePath, bool isForced, out string targetPath) {
        // ensure the base directory was found
        var basePath = GetBasePath("AdventOfCode.CSharp");
        if (!Directory.Exists(basePath))
            throw new InvalidOperationException($"Project base directory not found (tried: {basePath})");

        // initialise the target directory if not found
        var targetDirectory = Path.Combine(basePath, Path.GetFileName(Path.GetDirectoryName(sourcePath))!);
        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        // check the target path for existing files
        targetPath = Path.Combine(targetDirectory, Path.GetFileName(sourcePath));
        if (File.Exists(targetPath)) {
            if (!isForced)
                return false;

            CreateBackupFile(targetPath);
        }

        // rewrite the contents
        var contents = File.ReadAllText(sourcePath);
        contents = contents.Replace("namespace AdventOfCode.Today", "namespace AdventOfCode.CSharp");

        // write out the file
        File.WriteAllText(targetPath, contents);

        return true;
    }

    private static string GetBasePath(string projectName) {
        // use the currently executing image to reliably traverse to the project root folders 
        // app context is typically ./AdventOfCode/bin/Debug/netX.0/
        var basePath = Path.GetRelativePath(
            Environment.CurrentDirectory,
            Path.Combine(Path.Combine(AppContext.BaseDirectory, "../../../../"), projectName));

        if (!Directory.Exists(basePath))
            throw new InvalidOperationException($"Could not locate base path (tried: {basePath})");

        return basePath;
    }
}