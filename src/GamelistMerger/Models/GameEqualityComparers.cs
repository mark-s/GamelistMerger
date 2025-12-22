using GamelistMerger.Services.Filtering;

namespace GamelistMerger.Models;

public static class GameEqualityComparers
{
    public static readonly IEqualityComparer<Game> Default = Composite()
        .ByHash()
        .ByCrc32()
        .ByPath()
        .Build();

    private static CompositeComparerBuilder Composite() => new();

}