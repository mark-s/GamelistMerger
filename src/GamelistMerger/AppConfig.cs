namespace GamelistMerger;

public sealed record AppConfig(
    FileInfo GameListA,
    FileInfo GameListB,
    FileInfo OutputFile,
    bool ExcludeBios,
    string[] ExcludeNameContains,
    string[] ExcludeRegions,
    string[] IncludeLanguages,
    bool Verbose,
    bool Overwrite,
    bool PreferArchives,
    bool SortOutput);