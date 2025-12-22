using CSharpFunctionalExtensions;
using GamelistMerger.Services.Filtering;
using GamelistMerger.Services.Validation;
using GamelistMerger.Workflows.Models;

namespace GamelistMerger.Workflows;

public static class GamelistMergeWorkflow
{
    public static async Task<Result<MergeWorkflowResult, IReadOnlyList<ValidationError>>> ExecuteAsync(string[] args)
    {
        var configResult = AppConfigParser.ParseCommandLine(args);
        if (configResult.IsFailure)
            return Result.Failure<MergeWorkflowResult, IReadOnlyList<ValidationError>>(WrapError(configResult.Error));

        var config = configResult.Value;

        if (config.OutputFile.Exists && !config.Overwrite)
            return Result.Failure<MergeWorkflowResult, IReadOnlyList<ValidationError>>(
                WrapError($"Output file [{config.OutputFile.FullName}] already exists. Use --overwrite to replace it."));

        var filterResult = FilterConfigProvider.BuildFromCliArgs(config);

        return await ProcessGameListsAsync((config, filterResult.Value));
    }

    private static async Task<Result<MergeWorkflowResult, IReadOnlyList<ValidationError>>> ProcessGameListsAsync((AppConfig Config, FilterConfig Filters) context)
        => await GamelistProcessor.ProcessAsync(context.Config, context.Filters);

    private static IReadOnlyList<ValidationError> WrapError(string error)
        => [new(error)];
}
