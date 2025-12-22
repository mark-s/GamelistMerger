using GamelistMerger.Models;
using GamelistMerger.Services;
using GamelistMerger.Services.Filtering;
using GamelistMerger.Workflows.Models;

namespace GamelistMerger.Workflows.Steps;

public static class MergeStep
{
    public static (ParsedGameListPair Parsed, MergeResult MergeResult) Execute(
        ParsedGameListPair parsed,
        FilterConfig filters,
        AppConfig config)
    {
        var gamelistPair = Mapper.MapToGameLists(parsed.ParsedA, parsed.ParsedB);

        var mergeResult = GameMerger.MergeGameLists(
            master: gamelistPair.MasterGameList,
            secondary: gamelistPair.SecondaryGameList,
            gameComparer: GameEqualityComparers.Default,
            folderComparer: FolderEqualityComparers.ByName,
            FilterRuleCompiler.Compile(filters),
            PreferenceProvider.PreferCompressedFile(config));

        return (parsed, mergeResult);
    }
}
