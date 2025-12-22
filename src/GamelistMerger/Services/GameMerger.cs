using GamelistMerger.Models;
using GamelistMerger.Services.Filtering;

namespace GamelistMerger.Services;

public static class GameMerger
{
    public static MergeResult MergeGameLists(
        GameList master,
        GameList secondary,
        IEqualityComparer<Game> gameComparer,
        IEqualityComparer<Folder> folderComparer,
        Func<Game, bool> gameFilter,
        Func<Game, Game, Game?> fileTypePreferenceFunc)
    {
        var (masterIncluded, masterFiltered) = PartitionGames(master.Games, gameFilter);
        var (secondaryIncluded, secondaryFiltered) = PartitionGames(secondary.Games, gameFilter);

        var mergedGames = MergeItems(
            masterItems: masterIncluded,
            secondaryItems: secondaryIncluded,
            comparer: gameComparer,
            merge: (game1, game2) => GameFieldMerger.Merge(game1, game2, fileTypePreferenceFunc));

        var mergedFolders = MergeItems(
            masterItems: master.Folders,
            secondaryItems: secondary.Folders,
            comparer: folderComparer,
            merge: FolderMerger.Merge);

        var gameList = new GameList(master.Provider, mergedFolders, mergedGames);

        var statistics = new FilterStatistics(
            MasterFilteredGames: masterFiltered.ToList(),
            SecondaryFilteredGames: secondaryFiltered.ToList(),
            MasterIncludedCount: masterIncluded.Count,
            SecondaryIncludedCount: secondaryIncluded.Count,
            MergedGameCount: mergedGames.Count);

        return new MergeResult(gameList, statistics);
    }

    private static (List<Game> included, List<Game> filtered) PartitionGames(IEnumerable<Game> games, Func<Game, bool> filter)
    {
        var included = new List<Game>();
        var filtered = new List<Game>();

        foreach (var game in games)
        {
            if (filter(game))
                included.Add(game);
            else
                filtered.Add(game);
        }

        return (included, filtered);
    }

    private static IReadOnlyList<T> MergeItems<T>(IEnumerable<T> masterItems, IEnumerable<T> secondaryItems, IEqualityComparer<T> comparer, Func<T, T, T> merge) where T : notnull
    {
        var merged = new Dictionary<T, T>(comparer);

        foreach (var item in masterItems)
        {
            merged[item] = item;
        }

        foreach (var secondaryItem in secondaryItems)
        {
            if (merged.TryGetValue(secondaryItem, out var masterItem))
                merged[secondaryItem] = merge(masterItem, secondaryItem);
            else
                merged[secondaryItem] = secondaryItem;
        }

        return [.. merged.Values];
    }
}
