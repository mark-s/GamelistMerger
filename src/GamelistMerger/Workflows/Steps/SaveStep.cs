using CSharpFunctionalExtensions;
using GamelistMerger.Models;
using GamelistMerger.Services.IO;
using GamelistMerger.Services.Sorting;
using GamelistMerger.Services.Validation;
using GamelistMerger.Workflows.Models;

namespace GamelistMerger.Workflows.Steps;

public static class SaveStep
{
    public static Result<MergeWorkflowResult, IReadOnlyList<ValidationError>> Execute(
        ParsedGameListPair parsed,
        MergeResult mergeResult,
        FileInfo outputFile,
        TimeSpan duration,
        bool verbose,
        bool sortOutput)
    {
        var gameListToSave = sortOutput
            ? GameListSorter.SortByName(mergeResult.MergedGameList)
            : mergeResult.MergedGameList;

        var saveResult = XmlWriter.SaveAsXml(Mapper.MapToDto(gameListToSave), outputFile);

        if (saveResult.IsSuccess)
            return new MergeWorkflowResult(parsed.ValidationA, parsed.ValidationB, outputFile, mergeResult.Statistics, duration, verbose);
        else
            return Result.Failure<MergeWorkflowResult, IReadOnlyList<ValidationError>>([new ValidationError(saveResult.Error)]);
    }
}
