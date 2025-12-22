using System.Collections.Immutable;

namespace GamelistMerger.Models;

public sealed class GameList(Provider? provider, IReadOnlyList<Folder> folders, IReadOnlyList<Game> games)
{
    public int TotalGames => Games.Length;

    public Provider? Provider { get; } = provider;

    public ImmutableArray<Folder> Folders { get; } = folders?.ToImmutableArray() ?? [];

    public ImmutableArray<Game> Games { get; } = games?.ToImmutableArray() ?? [];
}