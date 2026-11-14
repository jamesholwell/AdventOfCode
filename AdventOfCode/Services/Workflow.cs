using System.Xml;
using System.Xml.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode.Services;

internal static class Workflow {
    /// <summary>
    ///     Initialise a new solver in the today project
    /// </summary>
    public static bool TryInitialiseSolver(PuzzleArguments puzzle, bool isForced, out string targetPath) {
        if (!puzzle.IsValid)
            throw new InvalidOperationException(nameof(puzzle));

        var (project, ext) = puzzle.Language switch {
            PuzzleArguments.Languages.FSharp => ("AdventOfCode.FSharp", "fs"),
            _ => ("AdventOfCode.Today", "cs")
        };

        var templatePath = Path.Combine(GetBasePath("AdventOfCode"), $"Templates/TemplateSolver.{ext}");
        if (!File.Exists(templatePath))
            throw new InvalidOperationException($"Template path not found (tried: {templatePath}");

        var basePath = GetBasePath(project);
        var eventPath = Path.Combine(basePath, puzzle.Event.ToString());
        if (!Directory.Exists(eventPath))
            Directory.CreateDirectory(eventPath);

        var className = $"Day{puzzle.Day}{puzzle.Suffix}";
        targetPath = Path.Combine(eventPath, $"{className}.{ext}");
        if (File.Exists(targetPath)) {
            if (!isForced)
                return false;

            CreateBackupFile(targetPath);
        }

        // rewrite the template contents
        var source = File.ReadAllText(templatePath);
        source = puzzle.Language switch {
            PuzzleArguments.Languages.FSharp => RewriteFSharpTemplate(),
            _ => RewriteCSharpTemplate()
        };

        // write the solver
        File.WriteAllText(targetPath, source);

        // add the solver to the project file
        if (puzzle.Language == PuzzleArguments.Languages.FSharp)
            AddSolverToFSharpProject();

        return true;

        string RewriteCSharpTemplate() {
            return source
                .Replace("namespace AdventOfCode.Templates;", $"namespace AdventOfCode.Today._{puzzle.Event};")
                .Replace("TemplateSolver", className);
        }

        string RewriteFSharpTemplate() {
            return source
                .Replace("namespace AdventOfCode.FSharp", $"namespace AdventOfCode.FSharp._{puzzle.Event}")
                .Replace("TemplateSolver", className);
        }

        // F# projects are ordered, so the files have to be individually listed
        void AddSolverToFSharpProject() {
            var fsprojPath = Path.Combine(basePath, $"{project}.fsproj");
            var fsproj = XDocument.Load(fsprojPath, LoadOptions.PreserveWhitespace);

            // locate the existing compile elements that load the other solvers
            var compileItems = fsproj.Descendants("Compile").ToArray();
            if (compileItems.Length == 0) throw new InvalidOperationException("Failed to add solver to .fsproj");

            // compose the compile element for the new solver
            var newElement = new XElement("Compile", new XAttribute("Include", $"{puzzle.Event}/{className}.{ext}"));

            // sort the new element into the sequence of all items
            var sortedCompileItems = compileItems
                .Union([newElement])
                .OrderBy(x => (string?)x.Attribute("Include"), new FilePathComparer())
                .ToArray();

            // insert the element into the XDocument with matching whitespace
            var neighbour = sortedCompileItems[1 + sortedCompileItems.IndexOf(newElement)];
            var newElements = neighbour.PreviousNode is XText xText
                ? new object?[] { newElement, new XText(xText.Value) }
                : [newElement];
            neighbour.AddBeforeSelf(newElements);

            // write the updated fsproj without <?xml> and preserving whitespace
            var writerSettings = new XmlWriterSettings {
                OmitXmlDeclaration = true,
                Indent = false
            };

            using var writer = XmlWriter.Create(fsprojPath, writerSettings);
            fsproj.Save(writer);
        }
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

    private class FilePathComparer : IComparer<string?> {
        private const string Shared = "Shared.fs";

        private const string Discovery = "Discovery.fs";

        public int Compare(string? x, string? y) {
            if (x == null || y == null) return 0;

            // always sort Shared to top and Discovery to bottom
            if (x == Shared || y == Shared)
                return x == Shared ? -1 : 1;
            if (x == Discovery || y == Discovery)
                return x == Discovery ? 1 : -1;

            var defaultComparison = string.CompareOrdinal(x, y);

            // attempt to decompose the path into {Event}/{Solver}.{ext}
            var xParts = x.Split(['/', '.'], 3);
            var yParts = y.Split(['/', '.'], 3);
            if (xParts.Length < 2 || yParts.Length < 2)
                return defaultComparison;

            // handle events
            if (!int.TryParse(xParts[0], out var xEvent) || !int.TryParse(yParts[0], out var yEvent))
                return defaultComparison;

            if (xEvent > yEvent) return 1;
            if (xEvent < yEvent) return -1;

            // handle days
            var xMatch = Puzzle.DayPattern.Match(xParts[1]);
            var yMatch = Puzzle.DayPattern.Match(yParts[1]);
            if (!xMatch.Success || !yMatch.Success)
                return defaultComparison;

            var xDay = Puzzle.GetDay(xMatch);
            var yDay = Puzzle.GetDay(yMatch);
            if (xDay > yDay) return 1;
            if (xDay < yDay) return -1;

            // handle suffices
            var xSuffix = Puzzle.GetSuffix(xMatch);
            var ySuffix = Puzzle.GetSuffix(yMatch);
            return string.CompareOrdinal(xSuffix, ySuffix);
        }
    }
}