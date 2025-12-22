using GamelistMerger.Services.Filtering;
using GamelistMerger.Services.Validation;

namespace GamelistMerger.Services.IO;

public static class ConsoleOutput
{
    public static void ShowLogo(Version? version)
    {
        Console.WriteLine(Logo.SINGLE_LINE);
        if (version is not null)
            Console.WriteLine($"Version: {version.Major}.{version.Minor}.{version.Build}".PadLeft(100));
    }

    public static void ShowSuccess(ValidationSuccess validationA, ValidationSuccess validationB, FileInfo outputFile)
    {
        WriteSuccess($"A file: {validationA.GameCount} games, {validationA.FolderCount} folders");
        WriteSuccess($"B file: {validationB.GameCount} games, {validationB.FolderCount} folders");
        WriteSuccess($"Wrote merged gamelist to [{outputFile.FullName}]");
    }

    public static void ShowErrors(IReadOnlyList<ValidationError> errors)
    {
        foreach (var error in errors)
            WriteError(error.ToString());
    }

    public static void ShowFilterSummary(FilterStatistics stats, TimeSpan duration, bool verbose)
    {
        if (verbose && stats.TotalFilteredCount > 0)
        {
            Console.WriteLine();
            WriteWarning($"Filtered {stats.TotalFilteredCount} game(s):");

            if (stats.MasterFilteredGames.Count > 0)
            {
                Console.WriteLine("  From A file:");
                foreach (var game in stats.MasterFilteredGames)
                {
                    Console.Write($"    - {game.Name}");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"      {game.Path}");
                    Console.ResetColor();
                }
            }

            if (stats.SecondaryFilteredGames.Count > 0)
            {
                Console.WriteLine("  From B file:");
                foreach (var game in stats.SecondaryFilteredGames)
                {
                    Console.Write($"    - {game.Name}");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"      {game.Path}");
                    Console.ResetColor();
                }
            }
        }

        Console.WriteLine($"Summary: {stats.MergedGameCount} games merged, {stats.TotalFilteredCount} filtered, completed in {duration.TotalSeconds:F2}s");
    }

    private static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("OK ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    private static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("! ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    private static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("X ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    public static void ShowUsage(string exeName)
    {
        Console.WriteLine("Merge, de-duplicate and optionally filter gamelist.xml files.");
        Console.WriteLine();

        WriteHeader("USAGE");
        Console.WriteLine($"  {exeName} -a <file> -b <file> -o <output> [options]");
        Console.WriteLine();

        WriteHeader("REQUIRED ARGUMENTS");
        WriteOption("-a, --gamelistA <file>", "Path to the first gamelist.xml file (master)");
        WriteOption("-b, --gamelistB <file>", "Path to the second gamelist.xml file");
        WriteOption("-o, --output <file>", "Path for the merged output file");
        Console.WriteLine();

        WriteHeader("FILTER OPTIONS");
        WriteOption("--exclude-bios", "Exclude BIOS files (games with 'BIOS' in name)");
        WriteOption("--exclude-name-contains <values>", "Exclude games where name contains any of these (comma-separated)");
        WriteOption("--exclude-region <regions>", "Exclude games from these regions (e.g., jp,eu)");
        WriteOption("--include-lang <languages>", "Only include games with these languages (e.g., en,jp)");
        Console.WriteLine();

        WriteHeader("OTHER OPTIONS");
        WriteOption("-v, --verbose", "Show details of the filtered games");
        WriteOption("--overwrite", "Overwrite output file if it exists");
        WriteOption("--prefer-archives", "Prefer the .zip/.7z file if available.");
        WriteOption("--sortOutput", "Sort games by name in the output file.");
        Console.WriteLine();

        WriteHeader("EXAMPLES");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  Just merge the two gamelists remove duplicates and output to merged.xml:");
        Console.ResetColor();
        Console.WriteLine($"     {exeName} -a gamelistA.xml -b gamelistB.xml -o merged.xml");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  Merge, filter out BIOS files and any non-en languages:");
        Console.ResetColor();
        Console.WriteLine($"     {exeName} -a gamelistA.xml -b gamelistB.xml -o merged.xml --exclude-bios --include-lang en");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  Merge, exclude JP & EU region games, and show the list of games filtered out:");
        Console.ResetColor();
        Console.WriteLine($"     {exeName} -a gamelistA.xml -b gamelistB.xml -o merged.xml --exclude-region jp,eu --verbose");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  Merge, prefer .zip/7z files and sort the output gamelist file:");
        Console.ResetColor();
        Console.WriteLine($"     {exeName} -a gamelistA.xml -b gamelistB.xml -o merged.xml --prefer-archives --sortOutput");
        Console.WriteLine();
    }

    private static void WriteHeader(string header)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(header);
        Console.ResetColor();
    }

    private static void WriteOption(string option, string description)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"  {option,-35}");
        Console.ResetColor();
        Console.WriteLine(description);
    }
}
