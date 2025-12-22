using GamelistMerger.Models;

namespace GamelistMerger.Services.Filtering;

public sealed record FilterStatistics(
    IReadOnlyList<Game> MasterFilteredGames,
    IReadOnlyList<Game> SecondaryFilteredGames,
    int MasterIncludedCount,
    int SecondaryIncludedCount,
    int MergedGameCount)
{
    public int TotalFilteredCount => MasterFilteredGames.Count + SecondaryFilteredGames.Count;
}
