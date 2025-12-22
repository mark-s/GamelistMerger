namespace GamelistMerger.Models;

public sealed class GameListPair
{
    public GameList MasterGameList { get; }
    public GameList SecondaryGameList { get; }

    public GameListPair(GameList gameListA, GameList gameListB)
    {
        if (gameListA.TotalGames > gameListB.TotalGames)
        {
            MasterGameList = gameListA;
            SecondaryGameList = gameListB;
        }
        else
        {
            MasterGameList = gameListB;
            SecondaryGameList = gameListA;
        }
    }
}