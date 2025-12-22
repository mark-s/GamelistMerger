using System.Xml.Linq;
using GamelistMerger.Models;
using GamelistMerger.Services;
using GamelistMerger.Services.Filtering;
using GamelistMerger.Services.Parsers;
using GamelistMerger.Tests.TestHelpers;

namespace GamelistMerger.Tests.Integration;

[TestFixture]
public class GamelistMergerIntegrationTests
{
    private readonly Func<Game, Game, Game?> _noFileTypePreference = (_, _) => null;

    [Test]
    public void EndToEnd_ParseMergeAndMap_WithTwoGamelists_ProducesCorrectOutput()
    {
        // Arrange
        var masterXml = GamelistXmlBuilder.Create()
            .WithProvider("Big Boy", "SomeScraper", "SomeScraper.io", "https://www.SomeScraper.io")
            .WithFolder(f => f.WithPath("./rpg").WithName("RPG Games").WithImage("./rpg.png"))
            .WithGame(g => g.WithPath("./game1.rom").WithName("Game 1").WithId("1").WithRating(0.8))
            .WithGame(g => g.WithPath("./game2.rom").WithName("Game 2 [BIOS]").WithId("2"))
            .WithGame(g => g.WithPath("./game3.rom").WithName("Game 3").WithId("3").WithRegion("us"))
            .Build();

        var secondaryXml = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithPath("./game1.rom").WithId("1").WithImage("./game1.png").WithDeveloper("Dev Co"))
            .WithGame(g => g.WithPath("./game4.rom").WithName("Game 4").WithId("4").WithLanguage("en"))
            .Build();

        var masterDoc = XDocument.Parse(masterXml);
        var secondaryDoc = XDocument.Parse(secondaryXml);

        var excludeBios = new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["[BIOS]"]);
        var filterConfig = new FilterConfig(ExcludeRules: [excludeBios], IncludeRules: []);

        // Act
        var masterParsed = GamelistParser.Parse(masterDoc);
        var secondaryParsed = GamelistParser.Parse(secondaryDoc);

        masterParsed.IsSuccess.ShouldBeTrue();
        secondaryParsed.IsSuccess.ShouldBeTrue();

        var gameListPair = Mapper.MapToGameLists(masterParsed.Value, secondaryParsed.Value);

        var mergeResult = GameMerger.MergeGameLists(
            master: gameListPair.MasterGameList,
            secondary: gameListPair.SecondaryGameList,
            gameComparer: GameEqualityComparers.Default,
            folderComparer: FolderEqualityComparers.ByPath,
            gameFilter: FilterRuleCompiler.Compile(filterConfig),
            fileTypePreferenceFunc: _noFileTypePreference);

        var outputDto = Mapper.MapToDto(mergeResult.MergedGameList);

        // Assert - Verify merged game count (3 from master minus 1 BIOS + 1 new from secondary = 3)
        mergeResult.MergedGameList.Games.Length.ShouldBe(3);
        outputDto.Games.Count.ShouldBe(3);

        // Verify provider from master is preserved
        outputDto.Provider.ShouldNotBeNull();
        outputDto.Provider.System.ShouldBe("Big Boy");

        // Verify folder is preserved
        outputDto.Folders.Count.ShouldBe(1);
        outputDto.Folders[0].Name.ShouldBe("RPG Games");

        // Verify BIOS was filtered out
        outputDto.Games.ShouldNotContain(g => g.Name == "Game 2 [BIOS]");

        // Verify Game 1 has merged properties (master name + secondary image/developer)
        var game1 = outputDto.Games.FirstOrDefault(g => g.Path == "./game1.rom");
        game1.ShouldNotBeNull();
        game1.Name.ShouldBe("Game 1");
        game1.Image.ShouldBe("./game1.png");
        game1.Developer.ShouldBe("Dev Co");
        game1.Rating.ShouldBe("0.80");

        // Verify secondary-only game was added
        outputDto.Games.ShouldContain(g => g.Path == "./game4.rom");

        // Verify statistics
        mergeResult.Statistics.MasterFilteredGames.Count.ShouldBe(1);
        mergeResult.Statistics.MasterFilteredGames[0].Name.ShouldBe("Game 2 [BIOS]");
    }

    [Test]
    public void EndToEnd_WithComplexFiltering_FiltersCorrectly()
    {
        // Arrange
        var masterXml = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithPath("./game1.rom").WithName("USA Game").WithRegion("us").WithLanguage("en"))
            .WithGame(g => g.WithPath("./game2.rom").WithName("Japan Game").WithRegion("jp").WithLanguage("jp"))
            .WithGame(g => g.WithPath("./game3.rom").WithName("Europe Game").WithRegion("eu").WithLanguage("en"))
            .WithGame(g => g.WithPath("./game4.rom").WithName("French Game").WithRegion("fr").WithLanguage("fr"))
            .WithGame(g => g.WithPath("./bios.rom").WithName("System [BIOS]").WithRegion("us").WithLanguage("en"))
            .WithGame(g => g.WithPath("./demo.rom").WithName("Demo Game").WithRegion("us").WithLanguage("en"))
            .Build();

        var secondaryXml = GamelistXmlBuilder.Create().Build();
        var masterDoc = XDocument.Parse(masterXml);
        var secondaryDoc = XDocument.Parse(secondaryXml);

        var excludeBios = new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["[BIOS]", "Demo"]);
        var excludeRegions = new FilterRule(FilterProperty.Region, FilterOperation.In, ["jp", "eu", "fr"]);
        var filterConfig = new FilterConfig(
            ExcludeRules: [excludeBios, excludeRegions],
            IncludeRules: []);

        // Act
        var masterParsed = GamelistParser.Parse(masterDoc);
        var secondaryParsed = GamelistParser.Parse(secondaryDoc);
        var gameListPair = Mapper.MapToGameLists(masterParsed.Value, secondaryParsed.Value);

        var mergeResult = GameMerger.MergeGameLists(
            master: gameListPair.MasterGameList,
            secondary: gameListPair.SecondaryGameList,
            gameComparer: GameEqualityComparers.Default,
            folderComparer: FolderEqualityComparers.ByPath,
            gameFilter: FilterRuleCompiler.Compile(filterConfig),
            fileTypePreferenceFunc: _noFileTypePreference);

        // Assert - Only USA Game should remain (BIOS, Demo, and non-US regions filtered)
        mergeResult.MergedGameList.Games.Length.ShouldBe(1);
        mergeResult.MergedGameList.Games[0].Name.ShouldBe("USA Game");

        mergeResult.Statistics.MasterFilteredGames.Count.ShouldBe(5);
    }

    [Test]
    public void EndToEnd_WithNonOverlappingFolders_CombinesFolders()
    {
        // Arrange - Non-overlapping folders (different names)
        var masterXml = GamelistXmlBuilder.Create()
            .WithFolder(f => f.WithPath("./rpg").WithName("RPG Games").WithId("1").WithImage("./rpg.png"))
            .WithFolder(f => f.WithPath("./action").WithName("Action Games").WithId("2").WithImage("./action.png"))
            .Build();

        var secondaryXml = GamelistXmlBuilder.Create()
            .WithFolder(f => f.WithPath("./sports").WithName("Sports Games").WithId("3").WithDescription("Sports folder"))
            .Build();

        var masterDoc = XDocument.Parse(masterXml);
        var secondaryDoc = XDocument.Parse(secondaryXml);

        // Act
        var masterParsed = GamelistParser.Parse(masterDoc);
        var secondaryParsed = GamelistParser.Parse(secondaryDoc);
        var gameListPair = Mapper.MapToGameLists(masterParsed.Value, secondaryParsed.Value);

        // Use ByName comparer like the real workflow (GamelistProcessor)
        var mergeResult = GameMerger.MergeGameLists(
            master: gameListPair.MasterGameList,
            secondary: gameListPair.SecondaryGameList,
            gameComparer: GameEqualityComparers.Default,
            folderComparer: FolderEqualityComparers.ByName,
            gameFilter: _ => true,
            fileTypePreferenceFunc: _noFileTypePreference);

        // Assert - All 3 folders should be in the result (no overlap, so all combined)
        mergeResult.MergedGameList.Folders.Length.ShouldBe(3);
        mergeResult.MergedGameList.Folders.ShouldContain(f => f.Name == "RPG Games");
        mergeResult.MergedGameList.Folders.ShouldContain(f => f.Name == "Action Games");
        mergeResult.MergedGameList.Folders.ShouldContain(f => f.Name == "Sports Games");

        // Verify master folders retain their properties
        var rpgFolder = mergeResult.MergedGameList.Folders.First(f => f.Name == "RPG Games");
        rpgFolder.Path.ShouldBe("./rpg");
        rpgFolder.Image.ShouldBe("./rpg.png");

        // Verify secondary folder is included
        var sportsFolder = mergeResult.MergedGameList.Folders.First(f => f.Name == "Sports Games");
        sportsFolder.Desc.ShouldBe("Sports folder");
    }

    [Test]
    public void EndToEnd_CliFilterArgsToCompilation_WorksCorrectly()
    {
        // Arrange - Simulate CLI arguments being converted to filter config
        var filterConfig = CliFilterArgsParser.GetFilterConfig(
            excludeBios: true,
            excludeNameContains: ["Beta", "Proto"],
            excludeRegions: ["jp"],
            includeLanguages: ["en", "us"]);

        var masterXml = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithPath("./game1.rom").WithName("Good Game").WithLanguage("en").WithRegion("us"))
            .WithGame(g => g.WithPath("./game2.rom").WithName("System [BIOS]").WithLanguage("en").WithRegion("us"))
            .WithGame(g => g.WithPath("./game3.rom").WithName("Beta Game").WithLanguage("en").WithRegion("us"))
            .WithGame(g => g.WithPath("./game4.rom").WithName("Proto Version").WithLanguage("en").WithRegion("us"))
            .WithGame(g => g.WithPath("./game5.rom").WithName("Japan Game").WithLanguage("jp").WithRegion("jp"))
            .WithGame(g => g.WithPath("./game6.rom").WithName("French Game").WithLanguage("fr").WithRegion("fr"))
            .Build();

        var masterDoc = XDocument.Parse(masterXml);
        var secondaryDoc = XDocument.Parse(GamelistXmlBuilder.Create().Build());

        // Act
        var masterParsed = GamelistParser.Parse(masterDoc);
        var secondaryParsed = GamelistParser.Parse(secondaryDoc);
        var gameListPair = Mapper.MapToGameLists(masterParsed.Value, secondaryParsed.Value);

        var mergeResult = GameMerger.MergeGameLists(
            master: gameListPair.MasterGameList,
            secondary: gameListPair.SecondaryGameList,
            gameComparer: GameEqualityComparers.Default,
            folderComparer: FolderEqualityComparers.ByPath,
            gameFilter: FilterRuleCompiler.Compile(filterConfig),
            fileTypePreferenceFunc: _noFileTypePreference);

        // Assert - Only "Good Game" should remain
        // Excluded: BIOS (name contains [BIOS]), Beta, Proto, Japan (region), French (not en/us lang)
        mergeResult.MergedGameList.Games.Length.ShouldBe(1);
        mergeResult.MergedGameList.Games[0].Name.ShouldBe("Good Game");
    }

    [Test]
    public void EndToEnd_WithRealWorldXml_ParsesAndMergesCorrectly()
    {
        // Arrange - Use real-world-like XML structure with demo data
        var masterXml = @"<?xml version=""1.0""?>
<gameList>
    <provider>
        <System>Demo Console</System>
        <software>SomeScraper</software>
        <database>SomeScraper.io</database>
        <web>http://www.SomeScraper.io</web>
    </provider>
    <folder id=""0"" source=""SomeScraper.io"">
        <path>./Best Games Collection</path>
        <name>Best Games Collection (512)</name>
        <image>./media/folders/Best Games Collection.png</image>
    </folder>
    <game id=""12345"" source=""SomeScraper.io"">
        <path>./Demo Game One (Europe).rom</path>
        <name>Demo Game One</name>
        <rating>0.65</rating>
        <releasedate>19930101T000000</releasedate>
        <developer>Demo Developer</developer>
        <publisher>Demo Publisher</publisher>
        <genre>Role Playing Game</genre>
        <players>1</players>
        <image>./images/Demo Game One.png</image>
    </game>
    <game id=""67890"" source=""SomeScraper.io"">
        <path>./Demo Game Two (USA).rom</path>
        <name>Demo Game Two</name>
        <desc>An exciting demo adventure awaits!</desc>
        <image>./images/Demo Game Two.png</image>
        <rating>0.6</rating>
        <releasedate>19900928T000000</releasedate>
        <developer>Another Developer</developer>
        <publisher>Another Publisher</publisher>
        <genre>Platform</genre>
        <players>1</players>
        <lang>en</lang>
        <region>us</region>
    </game>
</gameList>";

        var secondaryXml = @"<?xml version=""1.0""?>
<gameList>
    <game id=""12345"">
        <path>./Demo Game One (Europe).rom</path>
        <thumbnail>./thumbnails/Demo Game One.png</thumbnail>
        <genreid>768</genreid>
    </game>
</gameList>";

        var masterDoc = XDocument.Parse(masterXml);
        var secondaryDoc = XDocument.Parse(secondaryXml);

        // Act
        var masterParsed = GamelistParser.Parse(masterDoc);
        var secondaryParsed = GamelistParser.Parse(secondaryDoc);

        masterParsed.IsSuccess.ShouldBeTrue();
        secondaryParsed.IsSuccess.ShouldBeTrue();

        var gameListPair = Mapper.MapToGameLists(masterParsed.Value, secondaryParsed.Value);

        var mergeResult = GameMerger.MergeGameLists(
            master: gameListPair.MasterGameList,
            secondary: gameListPair.SecondaryGameList,
            gameComparer: GameEqualityComparers.Default,
            folderComparer: FolderEqualityComparers.ByPath,
            gameFilter: _ => true,
            fileTypePreferenceFunc: _noFileTypePreference);

        // Assert
        mergeResult.MergedGameList.Games.Length.ShouldBe(2);
        mergeResult.MergedGameList.Folders.Length.ShouldBe(1);
        mergeResult.MergedGameList.Provider.ShouldNotBeNull();
        mergeResult.MergedGameList.Provider.System.ShouldBe("Demo Console");

        // Demo Game One should have merged properties
        var demoGameOne = mergeResult.MergedGameList.Games.First(g => g.Path == "./Demo Game One (Europe).rom");
        demoGameOne.Name.ShouldBe("Demo Game One");
        demoGameOne.Rating.ShouldBe("0.65");
        demoGameOne.Thumbnail.ShouldBe("./thumbnails/Demo Game One.png");
        demoGameOne.GenreId.ShouldBe("768");
    }
}
