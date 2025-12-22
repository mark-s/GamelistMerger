namespace GamelistMerger.Tests.TestHelpers;

[TestFixture]
public class GamelistXmlBuilderTests
{
    [Test]
    public void Build_WithEmptyGamelist_GeneratesValidXml()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create();

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<?xml version=\"1.0\"?>");
        xml.ShouldContain("<gameList>");
        xml.ShouldContain("</gameList>");
    }

    [Test]
    public void Build_WithoutXmlDeclaration_OmitsDeclaration()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithoutXmlDeclaration();

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldNotContain("<?xml version=\"1.0\"?>");
        xml.ShouldContain("<gameList>");
    }

    [Test]
    public void Build_WithProvider_IncludesProviderElement()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithProvider("Big Boy", "SomeScraper", "SomeScraper.io", "https://www.SomeScraper.io");

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<provider>");
        xml.ShouldContain("<System>Big Boy</System>");
        xml.ShouldContain("<software>SomeScraper</software>");
        xml.ShouldContain("<database>SomeScraper.io</database>");
        xml.ShouldContain("<web>https://www.SomeScraper.io</web>");
        xml.ShouldContain("</provider>");
    }

    [Test]
    public void Build_WithSimpleGame_IncludesGameElement()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithGame(g => g
                .WithPath("./game1.rom")
                .WithName("Test Game"));

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<game>");
        xml.ShouldContain("<path>./game1.rom</path>");
        xml.ShouldContain("<name>Test Game</name>");
        xml.ShouldContain("</game>");
    }

    [Test]
    public void Build_WithCompleteGameMetadata_IncludesAllFields()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithGame(g => g
                .WithId("12345")
                .WithSource("SomeScraper.io")
                .WithPath("./game.rom")
                .WithName("Complete Game")
                .WithDescription("A game with all metadata")
                .WithImage("./images/game.png")
                .WithThumbnail("./images/game-thumb.png")
                .WithRating(0.85)
                .WithReleaseDate("19950101T000000")
                .WithDeveloper("Test Dev")
                .WithPublisher("Test Pub")
                .WithGenre("RPG")
                .WithPlayers(2)
                .WithHash("ABCD1234")
                .WithCrc32("12345678")
                .WithLanguage("en")
                .WithRegion("us")
                .WithGenreId("768")
                .WithPlayCount(10)
                .WithLastPlayed("20240115T103000"));

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("id=\"12345\"");
        xml.ShouldContain("source=\"SomeScraper.io\"");
        xml.ShouldContain("<path>./game.rom</path>");
        xml.ShouldContain("<name>Complete Game</name>");
        xml.ShouldContain("<desc>A game with all metadata</desc>");
        xml.ShouldContain("<image>./images/game.png</image>");
        xml.ShouldContain("<thumbnail>./images/game-thumb.png</thumbnail>");
        xml.ShouldContain("<rating>0.85</rating>");
        xml.ShouldContain("<releasedate>19950101T000000</releasedate>");
        xml.ShouldContain("<developer>Test Dev</developer>");
        xml.ShouldContain("<publisher>Test Pub</publisher>");
        xml.ShouldContain("<genre>RPG</genre>");
        xml.ShouldContain("<players>2</players>");
        xml.ShouldContain("<hash>ABCD1234</hash>");
        xml.ShouldContain("<crc32>12345678</crc32>");
        xml.ShouldContain("<lang>en</lang>");
        xml.ShouldContain("<region>us</region>");
        xml.ShouldContain("<genreid>768</genreid>");
        xml.ShouldContain("<playcount>10</playcount>");
        xml.ShouldContain("<lastplayed>20240115T103000</lastplayed>");
    }

    [Test]
    public void Build_WithGameWithoutPath_OmitsPathElement()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithName("Game Without Path"));

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<game>");
        xml.ShouldContain("<name>Game Without Path</name>");
        xml.ShouldNotContain("<path>");
    }

    [Test]
    public void Build_WithFolder_IncludesFolderElement()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithFolder(f => f
                .WithPath("./rpg-folder")
                .WithName("RPG Games"));

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<folder>");
        xml.ShouldContain("<path>./rpg-folder</path>");
        xml.ShouldContain("<name>RPG Games</name>");
        xml.ShouldContain("</folder>");
    }

    [Test]
    public void Build_WithCompleteFolderMetadata_IncludesAllFields()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithFolder(f => f
                .WithId("0")
                .WithSource("SomeScraper.io")
                .WithPath("./folder")
                .WithName("Test Folder")
                .WithDescription("Folder description")
                .WithImage("./images/folder.png")
                .WithThumbnail("./images/folder-thumb.png"));

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("id=\"0\"");
        xml.ShouldContain("source=\"SomeScraper.io\"");
        xml.ShouldContain("<path>./folder</path>");
        xml.ShouldContain("<name>Test Folder</name>");
        xml.ShouldContain("<desc>Folder description</desc>");
        xml.ShouldContain("<image>./images/folder.png</image>");
        xml.ShouldContain("<thumbnail>./images/folder-thumb.png</thumbnail>");
    }

    [Test]
    public void Build_WithMultipleGames_IncludesAllGames()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithPath("./game1.rom").WithName("Game 1"))
            .WithGame(g => g.WithPath("./game2.rom").WithName("Game 2"))
            .WithGame(g => g.WithPath("./game3.rom").WithName("Game 3"));

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<name>Game 1</name>");
        xml.ShouldContain("<name>Game 2</name>");
        xml.ShouldContain("<name>Game 3</name>");
    }

    [Test]
    public void Build_WithMixedElements_IncludesAllInOrder()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithProvider("Big Boy", "SomeScraper", "SomeScraper.io", "https://www.SomeScraper.io")
            .WithFolder(f => f.WithPath("./folder").WithName("Test Folder"))
            .WithGame(g => g.WithPath("./game1.rom").WithName("Game 1"))
            .WithGame(g => g.WithPath("./game2.rom").WithName("Game 2"));

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<provider>");
        xml.ShouldContain("<folder>");
        xml.ShouldContain("<game>");

        // Check order
        var providerIndex = xml.IndexOf("<provider>", StringComparison.Ordinal);
        var folderIndex = xml.IndexOf("<folder>", StringComparison.Ordinal);
        var gameIndex = xml.IndexOf("<game>", StringComparison.Ordinal);

        providerIndex.ShouldBeLessThan(folderIndex);
        folderIndex.ShouldBeLessThan(gameIndex);
    }

    [Test]
    public void Build_WithUnknownElement_IncludesUnknownElement()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithPath("./game.rom").WithName("Test Game"))
            .WithUnknownElement("custom", "Custom content");

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<custom>");
        xml.ShouldContain("<content>Custom content</content>");
        xml.ShouldContain("</custom>");
    }

    [Test]
    public void Build_WithUnknownElementNoContent_IncludesEmptyElement()
    {
        // Arrange
        var builder = GamelistXmlBuilder.Create()
            .WithUnknownElement("empty");

        // Act
        var xml = builder.Build();

        // Assert
        xml.ShouldContain("<empty>");
        xml.ShouldContain("</empty>");
    }
}
