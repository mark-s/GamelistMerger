using CSharpFunctionalExtensions;
using GamelistMerger.Services.Filtering;

namespace GamelistMerger.Workflows;

public static class FilterConfigProvider
{
    public static Result<FilterConfig> BuildFromCliArgs(AppConfig config)
    {
        var cliConfig = CliFilterArgsParser.GetFilterConfig(
            config.ExcludeBios,
            config.ExcludeNameContains,
            config.ExcludeRegions,
            config.IncludeLanguages);

        return Result.Success(cliConfig);
    }
}
