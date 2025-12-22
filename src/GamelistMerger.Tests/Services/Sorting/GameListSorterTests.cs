using GamelistMerger.Models;
using GamelistMerger.Services.Sorting;

namespace GamelistMerger.Tests.Services.Sorting;

[TestFixture]
public class GameListSorterTests
{
    [Test]
    public void ByName_SortsGamesAlphabeticallyByName()
    {
        // Arrange
        var games = new List<Game>
        {
            CreateGame("Zelda"),
            CreateGame("Asteroids"),
            CreateGame("Mario")
        };
        var gameList = new GameList(null, [], games);

        // Act
        var result = GameListSorter.SortByName(gameList);

        // Assert
        result.Games.Length.ShouldBe(3);
        result.Games[0].Name.ShouldBe("Asteroids");
        result.Games[1].Name.ShouldBe("Mario");
        result.Games[2].Name.ShouldBe("Zelda");
    }

    [Test]
    public void ByName_SortsCaseInsensitive()
    {
        // Arrange
        var games = new List<Game>
        {
            CreateGame("zelda"),
            CreateGame("ASTEROIDS"),
            CreateGame("Mario")
        };
        var gameList = new GameList(null, [], games);

        // Act
        var result = GameListSorter.SortByName(gameList);

        // Assert
        result.Games.Length.ShouldBe(3);
        result.Games[0].Name.ShouldBe("ASTEROIDS");
        result.Games[1].Name.ShouldBe("Mario");
        result.Games[2].Name.ShouldBe("zelda");
    }

    [Test]
    public void ByName_HandlesNullNames()
    {
        // Arrange
        var games = new List<Game>
        {
            CreateGame("Zelda"),
            CreateGame(null),
            CreateGame("Asteroids")
        };
        var gameList = new GameList(null, [], games);

        // Act
        var result = GameListSorter.SortByName(gameList);

        // Assert
        result.Games.Length.ShouldBe(3);
        result.Games[0].Name.ShouldBeNull();
        result.Games[1].Name.ShouldBe("Asteroids");
        result.Games[2].Name.ShouldBe("Zelda");
    }

    [Test]
    public void ByName_HandlesEmptyNames()
    {
        // Arrange
        var games = new List<Game>
        {
            CreateGame("Zelda"),
            CreateGame(""),
            CreateGame("Asteroids")
        };
        var gameList = new GameList(null, [], games);

        // Act
        var result = GameListSorter.SortByName(gameList);

        // Assert
        result.Games.Length.ShouldBe(3);
        result.Games[0].Name.ShouldBe("");
        result.Games[1].Name.ShouldBe("Asteroids");
        result.Games[2].Name.ShouldBe("Zelda");
    }

    [Test]
    public void ByName_PreservesProvider()
    {
        // Arrange
        var provider = new Provider("System1", "Software1", "Database1", "Web1");
        var games = new List<Game> { CreateGame("Game1") };
        var gameList = new GameList(provider, [], games);

        // Act
        var result = GameListSorter.SortByName(gameList);

        // Assert
        result.Provider.ShouldNotBeNull();
        result.Provider.System.ShouldBe("System1");
        result.Provider.Software.ShouldBe("Software1");
    }

    [Test]
    public void ByName_PreservesFolders()
    {
        // Arrange
        var folders = new List<Folder>
        {
            new Folder("1", "source", "Folder1", "Desc", null, null, "./path1"),
            new Folder("2", "source", "Folder2", "Desc", null, null, "./path2")
        };
        var games = new List<Game> { CreateGame("Game1") };
        var gameList = new GameList(null, folders, games);

        // Act
        var result = GameListSorter.SortByName(gameList);

        // Assert
        result.Folders.Length.ShouldBe(2);
        result.Folders[0].Name.ShouldBe("Folder1");
        result.Folders[1].Name.ShouldBe("Folder2");
    }

    [Test]
    public void ByName_HandlesEmptyGameList()
    {
        // Arrange
        var gameList = new GameList(null, [], []);

        // Act
        var result = GameListSorter.SortByName(gameList);

        // Assert
        result.Games.Length.ShouldBe(0);
    }

    [Test]
    public void ByName_HandlesSingleGame()
    {
        // Arrange
        var games = new List<Game> { CreateGame("OnlyGame") };
        var gameList = new GameList(null, [], games);

        // Act
        var result = GameListSorter.SortByName(gameList);

        // Assert
        result.Games.Length.ShouldBe(1);
        result.Games[0].Name.ShouldBe("OnlyGame");
    }

    private static Game CreateGame(string? name)
        => new Game(
            Id: null,
            Source: null,
            Name: name,
            Desc: null,
            Image: null,
            Thumbnail: null,
            Path: $"./{name ?? "unknown"}.rom",
            Rating: null,
            ReleaseDate: null,
            Developer: null,
            Publisher: null,
            Genre: null,
            Players: null,
            PlayCount: null,
            LastPlayed: null,
            Favorite: null,
            Hash: null,
            Crc32: null,
            Lang: null,
            Region: null,
            GenreId: null);
}
