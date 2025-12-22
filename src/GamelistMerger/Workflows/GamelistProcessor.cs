using System.Diagnostics;
using CSharpFunctionalExtensions;
using GamelistMerger.Services.Filtering;
using GamelistMerger.Services.Validation;
using GamelistMerger.Workflows.Models;
using GamelistMerger.Workflows.Steps;

namespace GamelistMerger.Workflows;

public static class GamelistProcessor
{
    public static async Task<Result<MergeWorkflowResult, IReadOnlyList<ValidationError>>> ProcessAsync(AppConfig config, FilterConfig filters)
    {
        var stopwatch = Stopwatch.StartNew();

        return await LoadStep.ExecuteAsync(config)
            .Bind(ParseStep.Execute)
            .Map(parsed => MergeStep.Execute(parsed, filters, config))
            .Bind(data => SaveStep.Execute(data.Parsed, data.MergeResult, config.OutputFile, stopwatch.Elapsed, config.Verbose, config.SortOutput));
    }
}
