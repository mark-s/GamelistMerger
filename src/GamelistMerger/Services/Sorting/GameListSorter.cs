using GamelistMerger.Models;

namespace GamelistMerger.Services.Sorting;

public static class GameListSorter
{
    public static GameList SortByName(GameList gameList)
    {
        var sortedGames = gameList.Games
            .OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new GameList(
            gameList.Provider,
            gameList.Folders.ToList(),
            sortedGames);
    }
}