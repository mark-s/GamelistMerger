using GamelistMerger.Models;
using GamelistMerger.Services;
using GamelistMerger.Services.Filtering;
using GamelistMerger.Tests.TestHelpers;

namespace GamelistMerger.Tests.Services;

[TestFixture]
public class GameMergerTests
{
    private readonly IEqualityComparer<Game> _gameComparer = GameEqualityComparers.Default;
    private readonly IEqualityComparer<Game> _gameComparerById = new CompositeComparerBuilder().ById().ByPath().Build();
    private readonly IEqualityComparer<Folder> _folderComparer = FolderEqualityComparers.ByPath;
    private readonly Func<Game, Game, Game?> _noFileTypePreference = (_, _) => null;

    [Test]
    public void MergeGameLists_WithEmptyLists_ReturnsEmptyGameList()
    {
        // Arrange
        var master = new GameList(null, [], []);
        var secondary = new GameList(null, [], []);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(0);
        result.MergedGameList.Folders.Length.ShouldBe(0);
    }

    [Test]
    public void MergeGameLists_WithOnlyMasterGames_ReturnsMasterGames()
    {
        // Arrange
        var game1 = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Game 1")
            .WithPath("./game1.rom")
            .Build();
        var master = new GameList(null, [], [game1]);
        var secondary = new GameList(null, [], []);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        result.MergedGameList.Games[0].Name.ShouldBe("Game 1");
        result.Statistics.MasterIncludedCount.ShouldBe(1);
        result.Statistics.SecondaryIncludedCount.ShouldBe(0);
    }

    [Test]
    public void MergeGameLists_WithOnlySecondaryGames_ReturnsSecondaryGames()
    {
        // Arrange
        var game1 = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Game 1")
            .WithPath("./game1.rom")
            .Build();
        var master = new GameList(null, [], []);
        var secondary = new GameList(null, [], [game1]);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        result.MergedGameList.Games[0].Name.ShouldBe("Game 1");
        result.Statistics.MasterIncludedCount.ShouldBe(0);
        result.Statistics.SecondaryIncludedCount.ShouldBe(1);
    }

    [Test]
    public void MergeGameLists_WithNonOverlappingGames_CombinesBothLists()
    {
        // Arrange
        var masterGame = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Master Game")
            .WithPath("./master.rom")
            .Build();
        var secondaryGame = TestGameBuilder.Create()
            .WithId("2")
            .WithName("Secondary Game")
            .WithPath("./secondary.rom")
            .Build();
        var master = new GameList(null, [], [masterGame]);
        var secondary = new GameList(null, [], [secondaryGame]);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(2);
        result.MergedGameList.Games.ShouldContain(g => g.Name == "Master Game");
        result.MergedGameList.Games.ShouldContain(g => g.Name == "Secondary Game");
        result.Statistics.MergedGameCount.ShouldBe(2);
    }

    [Test]
    public void MergeGameLists_WithOverlappingGames_MergesPropertiesFromSecondary()
    {
        // Arrange
        var masterGame = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Game 1")
            .WithPath("./game1.rom")
            .WithRating("0.5")
            .Build();
        var secondaryGame = TestGameBuilder.Create()
            .WithId("1")
            .WithPath("./game1.rom")
            .WithDescription("Complete description")
            .WithImage("./image.png")
            .WithReleaseDate("19950101T000000")
            .WithDeveloper("Test Dev")
            .Build();
        var master = new GameList(null, [], [masterGame]);
        var secondary = new GameList(null, [], [secondaryGame]);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        var merged = result.MergedGameList.Games[0];
        merged.Name.ShouldBe("Game 1");
        merged.Desc.ShouldBe("Complete description");
        merged.Image.ShouldBe("./image.png");
        merged.Rating.ShouldBe("0.5");
        merged.ReleaseDate.ShouldBe("19950101T000000");
        merged.Developer.ShouldBe("Test Dev");
    }

    [Test]
    public void MergeGameLists_WithOverlappingGames_PrefersMasterNonNullValues()
    {
        // Arrange
        var masterGame = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Master Name")
            .WithDescription("Master Desc")
            .WithImage("./master.png")
            .WithPath("./game1.rom")
            .WithRating("0.8")
            .Build();
        var secondaryGame = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Secondary Name")
            .WithDescription("Secondary Desc")
            .WithImage("./secondary.png")
            .WithPath("./game1.rom")
            .WithRating("0.5")
            .Build();
        var master = new GameList(null, [], [masterGame]);
        var secondary = new GameList(null, [], [secondaryGame]);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        var merged = result.MergedGameList.Games[0];
        merged.Name.ShouldBe("Master Name");
        merged.Desc.ShouldBe("Master Desc");
        merged.Image.ShouldBe("./master.png");
        merged.Rating.ShouldBe("0.8");
    }

    [Test]
    public void MergeGameLists_WithFilter_ExcludesFilteredGames()
    {
        // Arrange
        var game1 = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Game 1")
            .WithPath("./game1.rom")
            .WithRegion("us")
            .Build();
        var game2 = TestGameBuilder.Create()
            .WithId("2")
            .WithName("Game 2")
            .WithPath("./game2.rom")
            .WithRegion("jp")
            .Build();
        var master = new GameList(null, [], [game1, game2]);
        var secondary = new GameList(null, [], []);
        var filter = (Game g) => g.Region != "jp";

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        result.MergedGameList.Games[0].Region.ShouldBe("us");
        result.Statistics.MasterFilteredGames.Count.ShouldBe(1);
        result.Statistics.MasterFilteredGames[0].Region.ShouldBe("jp");
    }

    [Test]
    public void MergeGameLists_WithFilteredGames_RecordsStatistics()
    {
        // Arrange
        var masterGame1 = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Game 1")
            .WithPath("./game1.rom")
            .WithRegion("us")
            .Build();
        var masterGame2 = TestGameBuilder.Create()
            .WithId("2")
            .WithName("Game 2 [BIOS]")
            .WithPath("./bios.rom")
            .Build();
        var secondaryGame1 = TestGameBuilder.Create()
            .WithId("3")
            .WithName("Game 3")
            .WithPath("./game3.rom")
            .WithRegion("eu")
            .Build();
        var secondaryGame2 = TestGameBuilder.Create()
            .WithId("4")
            .WithName("Game 4 [BIOS]")
            .WithPath("./bios2.rom")
            .Build();
        var master = new GameList(null, [], [masterGame1, masterGame2]);
        var secondary = new GameList(null, [], [secondaryGame1, secondaryGame2]);
        var filter = (Game g) => g.Name?.Contains("[BIOS]") != true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.Statistics.MasterIncludedCount.ShouldBe(1);
        result.Statistics.SecondaryIncludedCount.ShouldBe(1);
        result.Statistics.MasterFilteredGames.Count.ShouldBe(1);
        result.Statistics.SecondaryFilteredGames.Count.ShouldBe(1);
        result.Statistics.MergedGameCount.ShouldBe(2);
    }

    [Test]
    public void MergeGameLists_WithFolders_MergesFolders()
    {
        // Arrange
        var folder1 = TestFolderBuilder.Create()
            .WithId("1")
            .WithName("Folder 1")
            .WithPath("./folder1")
            .Build();
        var folder2 = TestFolderBuilder.Create()
            .WithId("2")
            .WithName("Folder 2")
            .WithPath("./folder2")
            .Build();
        var master = new GameList(null, [folder1], []);
        var secondary = new GameList(null, [folder2], []);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Folders.Length.ShouldBe(2);
        result.MergedGameList.Folders.ShouldContain(f => f.Name == "Folder 1");
        result.MergedGameList.Folders.ShouldContain(f => f.Name == "Folder 2");
    }

    [Test]
    public void MergeGameLists_WithOverlappingFolders_MergesFolderProperties()
    {
        // Arrange
        var masterFolder = TestFolderBuilder.Create()
            .WithId("1")
            .WithName("Folder")
            .WithImage("./master-image.png")
            .WithPath("./folder")
            .Build();
        var secondaryFolder = TestFolderBuilder.Create()
            .WithId("2")
            .WithSource("ScreenScraper")
            .WithDescription("Folder description")
            .WithThumbnail("./thumb.png")
            .WithPath("./folder")
            .Build();
        var master = new GameList(null, [masterFolder], []);
        var secondary = new GameList(null, [secondaryFolder], []);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Folders.Length.ShouldBe(1);
        var merged = result.MergedGameList.Folders[0];
        merged.Id.ShouldBe("1");
        merged.Name.ShouldBe("Folder");
        merged.Desc.ShouldBe("Folder description");
        merged.Image.ShouldBe("./master-image.png");
        merged.Thumbnail.ShouldBe("./thumb.png");
        merged.Source.ShouldBe("ScreenScraper");
    }

    [Test]
    public void MergeGameLists_WithProvider_UsesMasterProvider()
    {
        // Arrange
        var masterProvider = new Provider(System: "Big Boy", Software: "SomeScraper", Database: "SomeScraper.io", Web: "http://www.SomeScraper.io");
        var secondaryProvider = new Provider(System: "Big Boy Advance", Software: "Other", Database: "Other.com", Web: "http://other.com");
        var master = new GameList(masterProvider, [], []);
        var secondary = new GameList(secondaryProvider, [], []);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Provider.ShouldNotBeNull();
        result.MergedGameList.Provider.System.ShouldBe("Big Boy");
        result.MergedGameList.Provider.Software.ShouldBe("SomeScraper");
    }

    [Test]
    public void MergeGameLists_WithComplexScenario_MergesCorrectly()
    {
        // Arrange
        var masterGames = new[]
        {
            TestGameBuilder.Create()
                .WithId("1")
                .WithName("Game 1")
                .WithDescription("Master desc")
                .WithPath("./game1.rom")
                .WithRating("0.8")
                .WithPlayCount("5")
                .WithRegion("us")
                .Build(),
            TestGameBuilder.Create()
                .WithId("2")
                .WithName("Game 2 [BIOS]")
                .WithPath("./bios.rom")
                .Build(),
            TestGameBuilder.Create()
                .WithId("3")
                .WithName("Game 3")
                .WithPath("./game3.rom")
                .WithRegion("us")
                .Build()
        };

        var secondaryGames = new[]
        {
            TestGameBuilder.Create()
                .WithId("1")
                .WithPath("./game1.rom")
                .WithImage("./game1.png")
                .WithReleaseDate("19950101T000000")
                .WithDeveloper("Dev 1")
                .WithRegion("us")
                .Build(),
            TestGameBuilder.Create()
                .WithId("4")
                .WithName("Game 4")
                .WithPath("./game4.rom")
                .WithRegion("eu")
                .Build()
        };

        var master = new GameList(null, [], masterGames);
        var secondary = new GameList(null, [], secondaryGames);
        var filter = (Game g) => g.Name?.Contains("[BIOS]") != true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparer, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(3);

        var game1 = result.MergedGameList.Games.FirstOrDefault(g => g.Id == "1");
        game1.ShouldNotBeNull();
        game1.Name.ShouldBe("Game 1");
        game1.Desc.ShouldBe("Master desc");
        game1.Image.ShouldBe("./game1.png");
        game1.Rating.ShouldBe("0.8");
        game1.ReleaseDate.ShouldBe("19950101T000000");
        game1.Developer.ShouldBe("Dev 1");
        game1.PlayCount.ShouldBe("5");

        result.MergedGameList.Games.ShouldContain(g => g.Id == "3");
        result.MergedGameList.Games.ShouldContain(g => g.Id == "4");
        result.MergedGameList.Games.ShouldNotContain(g => g.Id == "2");

        result.Statistics.MasterIncludedCount.ShouldBe(2);
        result.Statistics.SecondaryIncludedCount.ShouldBe(2);
        result.Statistics.MasterFilteredGames.Count.ShouldBe(1);
        result.Statistics.MergedGameCount.ShouldBe(3);
    }

    [Test]
    public void MergeGameLists_WhenFileTypePreferenceFuncReturnsGame2_UsesSGame2PathImageAndThumbnail()
    {
        // Arrange
        var masterGame = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Master Game")
            .WithPath("./game1.rom")
            .WithImage("./master-image.png")
            .WithThumbnail("./master-thumb.png")
            .WithDescription("Master description")
            .WithRating("0.8")
            .Build();
        var secondaryGame = TestGameBuilder.Create()
            .WithId("1")
            .WithPath("./game1.zip")
            .WithImage("./secondary-image.png")
            .WithThumbnail("./secondary-thumb.png")
            .WithDeveloper("Test Developer")
            .Build();
        var master = new GameList(null, [], [masterGame]);
        var secondary = new GameList(null, [], [secondaryGame]);
        var filter = (Game _) => true;
        var fileTypePreferenceFunc = (Game _, Game game2) => game2;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparerById, _folderComparer, filter, fileTypePreferenceFunc);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        var merged = result.MergedGameList.Games[0];
        merged.Path.ShouldBe("./game1.zip");
        merged.Image.ShouldBe("./secondary-image.png");
        merged.Thumbnail.ShouldBe("./secondary-thumb.png");
        merged.Name.ShouldBe("Master Game");
        merged.Desc.ShouldBe("Master description");
        merged.Rating.ShouldBe("0.8");
        merged.Developer.ShouldBe("Test Developer");
    }

    [Test]
    public void MergeGameLists_WhenFileTypePreferenceFuncReturnsGame1_UsesGame1PathImageAndThumbnail()
    {
        // Arrange
        var masterGame = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Master Game")
            .WithPath("./game1.7z")
            .WithImage("./master-image.png")
            .WithThumbnail("./master-thumb.png")
            .WithDescription("Master description")
            .WithRating("0.8")
            .Build();
        var secondaryGame = TestGameBuilder.Create()
            .WithId("1")
            .WithPath("./game1.rom")
            .WithImage("./secondary-image.png")
            .WithThumbnail("./secondary-thumb.png")
            .WithDeveloper("Test Developer")
            .Build();
        var master = new GameList(null, [], [masterGame]);
        var secondary = new GameList(null, [], [secondaryGame]);
        var filter = (Game _) => true;
        var fileTypePreferenceFunc = (Game game1, Game _) => game1;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparerById, _folderComparer, filter, fileTypePreferenceFunc);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        var merged = result.MergedGameList.Games[0];
        merged.Path.ShouldBe("./game1.7z");
        merged.Image.ShouldBe("./master-image.png");
        merged.Thumbnail.ShouldBe("./master-thumb.png");
        merged.Name.ShouldBe("Master Game");
        merged.Desc.ShouldBe("Master description");
        merged.Rating.ShouldBe("0.8");
        merged.Developer.ShouldBe("Test Developer");
    }

    [Test]
    public void MergeGameLists_WhenFileTypePreferenceFuncReturnsNull_UsesNormalMergeLogic()
    {
        // Arrange
        var masterGame = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Master Game")
            .WithPath("./game1.rom")
            .WithImage("./master-image.png")
            .WithThumbnail("./master-thumb.png")
            .WithDescription("Master description")
            .WithRating("0.8")
            .Build();
        var secondaryGame = TestGameBuilder.Create()
            .WithId("1")
            .WithPath("./game1.gb")
            .WithImage("./secondary-image.png")
            .WithThumbnail("./secondary-thumb.png")
            .WithDeveloper("Test Developer")
            .Build();
        var master = new GameList(null, [], [masterGame]);
        var secondary = new GameList(null, [], [secondaryGame]);
        var filter = (Game _) => true;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparerById, _folderComparer, filter, _noFileTypePreference);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        var merged = result.MergedGameList.Games[0];
        merged.Path.ShouldBe("./game1.rom");
        merged.Image.ShouldBe("./master-image.png");
        merged.Thumbnail.ShouldBe("./master-thumb.png");
        merged.Name.ShouldBe("Master Game");
        merged.Desc.ShouldBe("Master description");
        merged.Rating.ShouldBe("0.8");
        merged.Developer.ShouldBe("Test Developer");
    }

    [Test]
    public void MergeGameLists_WhenFileTypePreferenceFuncReturnsGame2ButMasterHasNoValues_UsesSSecondaryValues()
    {
        // Arrange
        var masterGame = TestGameBuilder.Create()
            .WithId("1")
            .WithName("Master Game")
            .WithPath(null)
            .WithImage(null)
            .WithThumbnail(null)
            .Build();
        var secondaryGame = TestGameBuilder.Create()
            .WithId("1")
            .WithPath("./game1.zip")
            .WithImage("./secondary-image.png")
            .WithThumbnail("./secondary-thumb.png")
            .WithDeveloper("Test Developer")
            .Build();
        var master = new GameList(null, [], [masterGame]);
        var secondary = new GameList(null, [], [secondaryGame]);
        var filter = (Game _) => true;
        var fileTypePreferenceFunc = (Game _, Game game2) => game2;

        // Act
        var result = GameMerger.MergeGameLists(master, secondary, _gameComparerById, _folderComparer, filter, fileTypePreferenceFunc);

        // Assert
        result.MergedGameList.Games.Length.ShouldBe(1);
        var merged = result.MergedGameList.Games[0];
        merged.Path.ShouldBe("./game1.zip");
        merged.Image.ShouldBe("./secondary-image.png");
        merged.Thumbnail.ShouldBe("./secondary-thumb.png");
        merged.Name.ShouldBe("Master Game");
        merged.Developer.ShouldBe("Test Developer");
    }
}
