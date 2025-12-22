using System.CommandLine;
using CSharpFunctionalExtensions;

namespace GamelistMerger;

public static class AppConfigParser
{
    public const string ShowUsageIndicator = "__SHOW_USAGE__";

    public static Result<AppConfig> ParseCommandLine(string[] args)
    {
        if (args.Length == 0 || args.Any(a => a is "-h" or "--help" or "-?"))
            return Result.Failure<AppConfig>(ShowUsageIndicator);
        Option<FileInfo> gameListA = new("--gamelistA", "-a") { Description = "The path to the Gamelist.xml file." };
        Option<FileInfo> gamelistB = new("--gamelistB", "-b") { Description = "The path to an other Gamelist.xml file." };
        Option<FileInfo> outputGamelist = new("--output", "-o") { Description = "The file to save the merged game list to." };

        Option<bool> preferArchives = new("--prefer-archives") { Description = "Prefer the .zip/.7z file if available." };
        Option<bool> sortOutput = new("--sortOutput") { Description = "Sort games by name in the output file." };

        Option<bool> excludeBios = new("--exclude-bios") { Description = "Exclude BIOS files (games with 'BIOS' in name)." };
        Option<string[]?> excludeNameContains = new("--exclude-name-contains")
        {
            Description = "Exclude games where name contains any of these values (comma-separated).",
            AllowMultipleArgumentsPerToken = true
        };
        Option<string[]?> excludeRegions = new("--exclude-region")
        {
            Description = "Exclude games from these regions (comma-separated, e.g., jp,eu).",
            AllowMultipleArgumentsPerToken = true
        };
        Option<string[]?> includeLanguages = new("--include-lang")
        {
            Description = "Only include games with these languages (comma-separated, e.g., en,us).",
            AllowMultipleArgumentsPerToken = true
        };

        Option<bool> verbose = new("--verbose", "-v") { Description = "Show paths of filtered games." };
        Option<bool> overwrite = new("--overwrite") { Description = "Overwrite output file if it exists." };

        RootCommand rootCommand = new("Gamelist Merger")
        {
            gameListA,
            gamelistB,
            outputGamelist,
            excludeBios,
            excludeNameContains,
            excludeRegions,
            includeLanguages,
            verbose,
            overwrite,
            preferArchives,
            sortOutput
        };

        var parseResult = rootCommand.Parse(args);

        if (parseResult.Errors.Any())
        {
            var errorMessage = string.Join(Environment.NewLine, parseResult.Errors.Select(e => e.Message));
            return Result.Failure<AppConfig>(errorMessage);
        }

        try
        {
            var config = new AppConfig(
                parseResult.GetRequiredValue(gameListA),
                parseResult.GetRequiredValue(gamelistB),
                parseResult.GetRequiredValue(outputGamelist),
                parseResult.GetValue(excludeBios),
                ParseCommaSeparated(parseResult.GetValue(excludeNameContains)) ?? [],
                ParseCommaSeparated(parseResult.GetValue(excludeRegions)) ?? [],
                ParseCommaSeparated(parseResult.GetValue(includeLanguages)) ?? [],
                Verbose: parseResult.GetValue(verbose),
                PreferArchives: parseResult.GetValue(preferArchives),
                Overwrite: parseResult.GetValue(overwrite),
                SortOutput: parseResult.GetValue(sortOutput));
            return Result.Success(config);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppConfig>($"Failed to parse command line arguments: {ex.Message}");
        }
    }

    private static string[]? ParseCommaSeparated(string[]? values)
    {
        if (values is null || values.Length == 0)
            return null;

        // Handle both --option a,b,c and --option a --option b --option c
        return values
            .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToArray();
    }
}