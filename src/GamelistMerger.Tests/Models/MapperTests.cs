using GamelistMerger.DTOs;
using GamelistMerger.Models;
using GamelistMerger.Tests.TestHelpers;

namespace GamelistMerger.Tests.Models;

[TestFixture]
public class MapperTests
{
    private static GameListDTO CreateEmptyDto() => new() { Games = [], Folders = [] };

    private static ProviderDTO CreateProviderDto(
        string system = "Big Boy",
        string software = "SomeScraper",
        string database = "SomeScraper.io",
        string web = "http://www.SomeScraper.io") =>
        new() { System = system, Software = software, Database = database, Web = web };

    private static GameDTO CreateGameDto(
        string path = "./game.rom",
        string? id = null,
        string? source = null,
        string? name = null,
        string? desc = null,
        string? image = null,
        string? thumbnail = null,
        string? rating = null,
        string? releaseDate = null,
        string? developer = null,
        string? publisher = null,
        string? genre = null,
        string? players = null,
        string? playCount = null,
        string? lastPlayed = null,
        string? favorite = null,
        string? hash = null,
        string? crc32 = null,
        string? lang = null,
        string? region = null,
        string? genreId = null) =>
        new()
        {
            Path = path,
            Id = id,
            Source = source,
            Name = name,
            Desc = desc,
            Image = image,
            Thumbnail = thumbnail,
            Rating = rating,
            ReleaseDate = releaseDate,
            Developer = developer,
            Publisher = publisher,
            Genre = genre,
            Players = players,
            PlayCount = playCount,
            LastPlayed = lastPlayed,
            Favorite = favorite,
            Hash = hash,
            Crc32 = crc32,
            Lang = lang,
            Region = region,
            GenreId = genreId
        };

    private static FolderDTO CreateFolderDto(
        string path = "./folder",
        string? id = null,
        string? source = null,
        string? name = null,
        string? desc = null,
        string? image = null,
        string? thumbnail = null) =>
        new() { Path = path, Id = id, Source = source, Name = name, Desc = desc, Image = image, Thumbnail = thumbnail };

    [TestFixture]
    public class MapToGameListsTests
    {
        [Test]
        public void MapToGameLists_WithTwoEmptyDtos_ReturnsPairOfEmptyGameLists()
        {
            // Arrange
            var dto1 = CreateEmptyDto();
            var dto2 = CreateEmptyDto();

            // Act
            var result = Mapper.MapToGameLists(dto1, dto2);

            // Assert
            result.MasterGameList.Games.Length.ShouldBe(0);
            result.MasterGameList.Folders.Length.ShouldBe(0);
            result.SecondaryGameList.Games.Length.ShouldBe(0);
            result.SecondaryGameList.Folders.Length.ShouldBe(0);
        }

        [Test]
        public void MapToGameLists_WithProvider_MapsProviderCorrectly()
        {
            // Arrange
            var dto1 = new GameListDTO
            {
                Provider = CreateProviderDto(),
                Games = [CreateGameDto()],
                Folders = []
            };
            var dto2 = CreateEmptyDto();

            // Act
            var result = Mapper.MapToGameLists(dto1, dto2);

            // Assert
            result.MasterGameList.Provider.ShouldNotBeNull();
            result.MasterGameList.Provider.System.ShouldBe("Big Boy");
            result.MasterGameList.Provider.Software.ShouldBe("SomeScraper");
            result.MasterGameList.Provider.Database.ShouldBe("SomeScraper.io");
            result.MasterGameList.Provider.Web.ShouldBe("http://www.SomeScraper.io");
        }

        [Test]
        public void MapToGameLists_WithNullProvider_MapsProviderAsNull()
        {
            // Arrange
            var dto1 = new GameListDTO { Provider = null, Games = [], Folders = [] };
            var dto2 = CreateEmptyDto();

            // Act
            var result = Mapper.MapToGameLists(dto1, dto2);

            // Assert
            result.MasterGameList.Provider.ShouldBeNull();
        }

        [Test]
        public void MapToGameLists_WithGames_MapsGamesCorrectly()
        {
            // Arrange
            var dto1 = new GameListDTO
            {
                Games = [CreateGameDto(
                    path: "./game1.rom",
                    id: "123",
                    source: "SomeScraper.io",
                    name: "Test Game",
                    desc: "Description",
                    image: "./image.png",
                    thumbnail: "./thumb.png",
                    rating: "0.85",
                    releaseDate: "19950101T000000",
                    developer: "Dev Studio",
                    publisher: "Pub Co",
                    genre: "RPG",
                    players: "2",
                    playCount: "5",
                    lastPlayed: "20240115T103000",
                    favorite: "true",
                    hash: "ABC123",
                    crc32: "12345678",
                    lang: "en",
                    region: "us",
                    genreId: "768")],
                Folders = []
            };
            var dto2 = CreateEmptyDto();

            // Act
            var result = Mapper.MapToGameLists(dto1, dto2);

            // Assert
            result.MasterGameList.Games.Length.ShouldBe(1);
            var game = result.MasterGameList.Games[0];
            game.Id.ShouldBe("123");
            game.Source.ShouldBe("SomeScraper.io");
            game.Path.ShouldBe("./game1.rom");
            game.Name.ShouldBe("Test Game");
            game.Desc.ShouldBe("Description");
            game.Image.ShouldBe("./image.png");
            game.Thumbnail.ShouldBe("./thumb.png");
            game.Rating.ShouldBe("0.85");
            game.ReleaseDate.ShouldBe("19950101T000000");
            game.Developer.ShouldBe("Dev Studio");
            game.Publisher.ShouldBe("Pub Co");
            game.Genre.ShouldBe("RPG");
            game.Players.ShouldBe("2");
            game.PlayCount.ShouldBe("5");
            game.LastPlayed.ShouldBe("20240115T103000");
            game.Favorite.ShouldBe("true");
            game.Hash.ShouldBe("ABC123");
            game.Crc32.ShouldBe("12345678");
            game.Lang.ShouldBe("en");
            game.Region.ShouldBe("us");
            game.GenreId.ShouldBe("768");
        }

        [Test]
        public void MapToGameLists_WithMultipleGames_MapsAllGames()
        {
            // Arrange
            var dto1 = new GameListDTO
            {
                Games =
                [
                    CreateGameDto(path: "./game1.rom", name: "Game 1"),
                    CreateGameDto(path: "./game2.rom", name: "Game 2"),
                    CreateGameDto(path: "./game3.rom", name: "Game 3")
                ],
                Folders = []
            };
            var dto2 = CreateEmptyDto();

            // Act
            var result = Mapper.MapToGameLists(dto1, dto2);

            // Assert
            result.MasterGameList.Games.Length.ShouldBe(3);
            result.MasterGameList.Games[0].Name.ShouldBe("Game 1");
            result.MasterGameList.Games[1].Name.ShouldBe("Game 2");
            result.MasterGameList.Games[2].Name.ShouldBe("Game 3");
        }

        [Test]
        public void MapToGameLists_WithFolders_MapsFoldersCorrectly()
        {
            // Arrange
            var dto1 = new GameListDTO
            {
                Games = [CreateGameDto()],
                Folders = [CreateFolderDto(
                    path: "./rpg-folder",
                    id: "0",
                    source: "SomeScraper.io",
                    name: "RPG Games",
                    desc: "Collection of RPG games",
                    image: "./folder.png",
                    thumbnail: "./folder-thumb.png")]
            };
            var dto2 = CreateEmptyDto();

            // Act
            var result = Mapper.MapToGameLists(dto1, dto2);

            // Assert
            result.MasterGameList.Folders.Length.ShouldBe(1);
            var folder = result.MasterGameList.Folders[0];
            folder.Id.ShouldBe("0");
            folder.Source.ShouldBe("SomeScraper.io");
            folder.Path.ShouldBe("./rpg-folder");
            folder.Name.ShouldBe("RPG Games");
            folder.Desc.ShouldBe("Collection of RPG games");
            folder.Image.ShouldBe("./folder.png");
            folder.Thumbnail.ShouldBe("./folder-thumb.png");
        }
    }

    [TestFixture]
    public class MapToDtoTests
    {
        [Test]
        public void MapToDto_WithEmptyGameList_ReturnsEmptyDto()
        {
            // Arrange
            var gameList = new GameList(provider: null, folders: [], games: []);

            // Act
            var result = Mapper.MapToDto(gameList);

            // Assert
            result.Games.Count.ShouldBe(0);
            result.Folders.Count.ShouldBe(0);
            result.Provider.ShouldBeNull();
        }

        [Test]
        public void MapToDto_WithProvider_MapsProviderCorrectly()
        {
            // Arrange
            var provider = new Provider(
                System: "Big Boy",
                Software: "SomeScraper",
                Database: "SomeScraper.io",
                Web: "http://www.SomeScraper.io");
            var gameList = new GameList(provider, folders: [], games: []);

            // Act
            var result = Mapper.MapToDto(gameList);

            // Assert
            result.Provider.ShouldNotBeNull();
            result.Provider.System.ShouldBe("Big Boy");
            result.Provider.Software.ShouldBe("SomeScraper");
            result.Provider.Database.ShouldBe("SomeScraper.io");
            result.Provider.Web.ShouldBe("http://www.SomeScraper.io");
        }

        [Test]
        public void MapToDto_WithGames_MapsGamesCorrectly()
        {
            // Arrange
            var game = TestGameBuilder.Create()
                .WithId("123")
                .WithSource("SomeScraper.io")
                .WithName("Test Game")
                .WithDescription("Description")
                .WithImage("./image.png")
                .WithThumbnail("./thumb.png")
                .WithPath("./game.rom")
                .WithRating("0.85")
                .WithReleaseDate("19950101T000000")
                .WithDeveloper("Dev Studio")
                .WithPublisher("Pub Co")
                .WithGenre("RPG")
                .WithPlayers("2")
                .WithPlayCount("5")
                .WithLastPlayed("20240115T103000")
                .WithFavorite("true")
                .WithHash("ABC123")
                .WithCrc32("12345678")
                .WithLang("en")
                .WithRegion("us")
                .WithGenreId("768")
                .Build();
            var gameList = new GameList(provider: null, folders: [], games: [game]);

            // Act
            var result = Mapper.MapToDto(gameList);

            // Assert
            result.Games.Count.ShouldBe(1);
            var gameDto = result.Games[0];
            gameDto.Id.ShouldBe("123");
            gameDto.Source.ShouldBe("SomeScraper.io");
            gameDto.Path.ShouldBe("./game.rom");
            gameDto.Name.ShouldBe("Test Game");
            gameDto.Desc.ShouldBe("Description");
            gameDto.Image.ShouldBe("./image.png");
            gameDto.Thumbnail.ShouldBe("./thumb.png");
            gameDto.Rating.ShouldBe("0.85");
            gameDto.ReleaseDate.ShouldBe("19950101T000000");
            gameDto.Developer.ShouldBe("Dev Studio");
            gameDto.Publisher.ShouldBe("Pub Co");
            gameDto.Genre.ShouldBe("RPG");
            gameDto.Players.ShouldBe("2");
            gameDto.PlayCount.ShouldBe("5");
            gameDto.LastPlayed.ShouldBe("20240115T103000");
            gameDto.Favorite.ShouldBe("true");
            gameDto.Hash.ShouldBe("ABC123");
            gameDto.Crc32.ShouldBe("12345678");
            gameDto.Lang.ShouldBe("en");
            gameDto.Region.ShouldBe("us");
            gameDto.GenreId.ShouldBe("768");
        }

        [Test]
        public void MapToDto_WithGameWithNullPath_MapsPathAsEmptyString()
        {
            // Arrange
            var game = TestGameBuilder.Create()
                .WithId("123")
                .WithName("Test Game")
                .Build();
            var gameList = new GameList(provider: null, folders: [], games: [game]);

            // Act
            var result = Mapper.MapToDto(gameList);

            // Assert
            result.Games[0].Path.ShouldBe("");
        }

        [Test]
        public void MapToDto_WithFolders_MapsFoldersCorrectly()
        {
            // Arrange
            var folder = TestFolderBuilder.Create()
                .WithId("0")
                .WithSource("SomeScraper.io")
                .WithName("RPG Games")
                .WithDescription("Collection of RPG games")
                .WithImage("./folder.png")
                .WithThumbnail("./folder-thumb.png")
                .WithPath("./rpg-folder")
                .Build();
            var gameList = new GameList(provider: null, folders: [folder], games: []);

            // Act
            var result = Mapper.MapToDto(gameList);

            // Assert
            result.Folders.Count.ShouldBe(1);
            var folderDto = result.Folders[0];
            folderDto.Id.ShouldBe("0");
            folderDto.Source.ShouldBe("SomeScraper.io");
            folderDto.Path.ShouldBe("./rpg-folder");
            folderDto.Name.ShouldBe("RPG Games");
            folderDto.Desc.ShouldBe("Collection of RPG games");
            folderDto.Image.ShouldBe("./folder.png");
            folderDto.Thumbnail.ShouldBe("./folder-thumb.png");
        }

        [Test]
        public void MapToDto_WithFolderWithNullPath_MapsPathAsEmptyString()
        {
            // Arrange
            var folder = TestFolderBuilder.Create()
                .WithId("0")
                .WithName("Test Folder")
                .Build();
            var gameList = new GameList(provider: null, folders: [folder], games: []);

            // Act
            var result = Mapper.MapToDto(gameList);

            // Assert
            result.Folders[0].Path.ShouldBe("");
        }

        [Test]
        public void MapToDto_WithCompleteGameList_MapsEverythingCorrectly()
        {
            // Arrange
            var provider = new Provider("Big Boy", "SomeScraper", "SomeScraper.io", "https://www.SomeScraper.io");
            var folder = TestFolderBuilder.Create()
                .WithId("0")
                .WithSource("SomeScraper.io")
                .WithName("RPG Games")
                .WithDescription("Folder desc")
                .WithImage("./folder.png")
                .WithPath("./rpg")
                .Build();
            var game = TestGameBuilder.Create()
                .WithId("123")
                .WithSource("SomeScraper.io")
                .WithName("Test Game")
                .WithDescription("Game desc")
                .WithImage("./game.png")
                .WithPath("./game.rom")
                .WithRating("0.85")
                .WithReleaseDate("19950101T000000")
                .WithDeveloper("Dev")
                .WithPublisher("Pub")
                .WithGenre("RPG")
                .WithPlayers("2")
                .WithPlayCount("5")
                .WithLastPlayed("20240115T103000")
                .WithHash("ABC")
                .WithCrc32("123")
                .WithLang("en")
                .WithRegion("us")
                .WithGenreId("768")
                .Build();
            var gameList = new GameList(provider, [folder], [game]);

            // Act
            var result = Mapper.MapToDto(gameList);

            // Assert
            result.Provider.ShouldNotBeNull();
            result.Folders.Count.ShouldBe(1);
            result.Games.Count.ShouldBe(1);
        }
    }

    [TestFixture]
    public class RoundTripTests
    {
        [Test]
        public void MapToDto_RoundTrip_PreservesAllData()
        {
            // Arrange
            var originalDto = new GameListDTO
            {
                Provider = CreateProviderDto(),
                Folders = [CreateFolderDto(
                    path: "./rpg",
                    id: "0",
                    source: "SomeScraper.io",
                    name: "RPG Games",
                    desc: "Folder desc",
                    image: "./folder.png",
                    thumbnail: "./folder-thumb.png")],
                Games = [CreateGameDto(
                    path: "./game.rom",
                    id: "123",
                    source: "SomeScraper.io",
                    name: "Test Game",
                    desc: "Game desc",
                    image: "./game.png",
                    thumbnail: "./game-thumb.png",
                    rating: "0.85",
                    releaseDate: "19950101T000000",
                    developer: "Dev",
                    publisher: "Pub",
                    genre: "RPG",
                    players: "2",
                    playCount: "5",
                    lastPlayed: "20240115T103000",
                    favorite: "true",
                    hash: "ABC",
                    crc32: "123",
                    lang: "en",
                    region: "us",
                    genreId: "768")]
            };

            // Act
            var gameListPair = Mapper.MapToGameLists(originalDto, originalDto);
            var roundTripDto = Mapper.MapToDto(gameListPair.MasterGameList);

            // Assert
            roundTripDto.Provider?.System.ShouldBe(originalDto.Provider.System);
            roundTripDto.Folders.Count.ShouldBe(1);
            roundTripDto.Folders[0].Name.ShouldBe("RPG Games");
            roundTripDto.Games.Count.ShouldBe(1);
            roundTripDto.Games[0].Name.ShouldBe("Test Game");
            roundTripDto.Games[0].Developer.ShouldBe("Dev");
        }
    }
}
