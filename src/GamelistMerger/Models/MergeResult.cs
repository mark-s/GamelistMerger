using GamelistMerger.Services.Filtering;

namespace GamelistMerger.Models;

public sealed record MergeResult(GameList MergedGameList, FilterStatistics Statistics);
