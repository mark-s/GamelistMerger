using System.Xml.Linq;
using GamelistMerger.Services.Parsers;
using GamelistMerger.Tests.TestHelpers;

namespace GamelistMerger.Tests.Services.Parsers;

[TestFixture]
public class GamelistParserTests
{
    [Test]
    public void Parse_WithEmptyGamelist_ReturnsEmptyGameListDto()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create().Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Games.Count.ShouldBe(0);
        result.Value.Folders.Count.ShouldBe(0);
        result.Value.Provider.ShouldBeNull();
    }

    [Test]
    public void Parse_WithProvider_ParsesProviderCorrectly()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithProvider("Big Boy", "SomeScraper", "SomeScraper.io", "https://www.SomeScraper.io")
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Provider.ShouldNotBeNull();
        result.Value.Provider.System.ShouldBe("Big Boy");
        result.Value.Provider.Software.ShouldBe("SomeScraper");
        result.Value.Provider.Database.ShouldBe("SomeScraper.io");
        result.Value.Provider.Web.ShouldBe("https://www.SomeScraper.io");
    }

    [Test]
    public void Parse_WithSingleGame_ParsesGameCorrectly()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithGame(g => g
                .WithPath("./game1.rom")
                .WithName("Test Game"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Games.Count.ShouldBe(1);
        result.Value.Games[0].Path.ShouldBe("./game1.rom");
        result.Value.Games[0].Name.ShouldBe("Test Game");
    }

    [Test]
    public void Parse_WithGameWithAllMetadata_ParsesAllFieldsCorrectly()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithGame(g => g
                .WithId("12345")
                .WithSource("SomeScraper.io")
                .WithPath("./game.rom")
                .WithName("Complete Game")
                .WithDescription("A complete game description")
                .WithImage("./images/game.png")
                .WithThumbnail("./images/game-thumb.png")
                .WithRating(0.85)
                .WithReleaseDate("19950101T000000")
                .WithDeveloper("Test Developer")
                .WithPublisher("Test Publisher")
                .WithGenre("RPG")
                .WithPlayers(2)
                .WithHash("ABCD1234")
                .WithCrc32("12345678")
                .WithLanguage("en")
                .WithRegion("us")
                .WithGenreId("768")
                .WithPlayCount(10)
                .WithLastPlayed("20240115T103000"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Games.Count.ShouldBe(1);
        var game = result.Value.Games[0];
        game.Id.ShouldBe("12345");
        game.Source.ShouldBe("SomeScraper.io");
        game.Path.ShouldBe("./game.rom");
        game.Name.ShouldBe("Complete Game");
        game.Desc.ShouldBe("A complete game description");
        game.Image.ShouldBe("./images/game.png");
        game.Thumbnail.ShouldBe("./images/game-thumb.png");
        game.Rating.ShouldBe("0.85");
        game.ReleaseDate.ShouldBe("19950101T000000");
        game.Developer.ShouldBe("Test Developer");
        game.Publisher.ShouldBe("Test Publisher");
        game.Genre.ShouldBe("RPG");
        game.Players.ShouldBe("2");
        game.Hash.ShouldBe("ABCD1234");
        game.Crc32.ShouldBe("12345678");
        game.Lang.ShouldBe("en");
        game.Region.ShouldBe("us");
        game.GenreId.ShouldBe("768");
        game.PlayCount.ShouldBe("10");
        game.LastPlayed.ShouldBe("20240115T103000");
    }

    [Test]
    public void Parse_WithMultipleGames_ParsesAllGames()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithPath("./game1.rom").WithName("Game 1"))
            .WithGame(g => g.WithPath("./game2.rom").WithName("Game 2"))
            .WithGame(g => g.WithPath("./game3.rom").WithName("Game 3"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Games.Count.ShouldBe(3);
        result.Value.Games[0].Name.ShouldBe("Game 1");
        result.Value.Games[1].Name.ShouldBe("Game 2");
        result.Value.Games[2].Name.ShouldBe("Game 3");
    }

    [Test]
    public void Parse_WithSingleFolder_ParsesFolderCorrectly()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithFolder(f => f
                .WithPath("./rpg-folder")
                .WithName("RPG Games"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Folders.Count.ShouldBe(1);
        result.Value.Folders[0].Path.ShouldBe("./rpg-folder");
        result.Value.Folders[0].Name.ShouldBe("RPG Games");
    }

    [Test]
    public void Parse_WithFolderWithAllMetadata_ParsesAllFieldsCorrectly()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithFolder(f => f
                .WithId("0")
                .WithSource("SomeScraper.io")
                .WithPath("./folder")
                .WithName("Test Folder")
                .WithDescription("Folder description")
                .WithImage("./images/folder.png")
                .WithThumbnail("./images/folder-thumb.png"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Folders.Count.ShouldBe(1);
        var folder = result.Value.Folders[0];
        folder.Id.ShouldBe("0");
        folder.Source.ShouldBe("SomeScraper.io");
        folder.Path.ShouldBe("./folder");
        folder.Name.ShouldBe("Test Folder");
        folder.Desc.ShouldBe("Folder description");
        folder.Image.ShouldBe("./images/folder.png");
        folder.Thumbnail.ShouldBe("./images/folder-thumb.png");
    }

    [Test]
    public void Parse_WithMultipleFolders_ParsesAllFolders()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithFolder(f => f.WithPath("./folder1").WithName("Folder 1"))
            .WithFolder(f => f.WithPath("./folder2").WithName("Folder 2"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Folders.Count.ShouldBe(2);
        result.Value.Folders[0].Name.ShouldBe("Folder 1");
        result.Value.Folders[1].Name.ShouldBe("Folder 2");
    }

    [Test]
    public void Parse_WithMixedContent_ParsesAllElements()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithProvider("Big Boy", "SomeScraper", "SomeScraper.io", "https://www.SomeScraper.io")
            .WithFolder(f => f.WithPath("./folder").WithName("Test Folder"))
            .WithGame(g => g.WithPath("./game1.rom").WithName("Game 1"))
            .WithGame(g => g.WithPath("./game2.rom").WithName("Game 2"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Provider.ShouldNotBeNull();
        result.Value.Provider.System.ShouldBe("Big Boy");
        result.Value.Folders.Count.ShouldBe(1);
        result.Value.Games.Count.ShouldBe(2);
    }

    [Test]
    public void XDocumentParse_WithMalformedXml_ThrowsXmlException()
    {
        // Arrange
        var malformedXml = "<gameList><game><path>./game.rom</path><invalidElement></game></gameList>";

        // Act & Assert
        Should.Throw<System.Xml.XmlException>(() => XDocument.Parse(malformedXml));
    }

    [Test]
    public void Parse_WithUnknownElement_ParsesSuccessfully()
    {
        // Arrange
        // Unknown elements are ignored by the XML deserialiser, not rejected
        var xml = "<gameList><game><unknownElement>value</unknownElement><path>./game.rom</path></game></gameList>";
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Games.Count.ShouldBe(1);
    }

    [Test]
    public void Parse_WithRealWorldExample_ParsesCorrectly()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<gameList>
	<provider>
		<System>Big Boy</System>
		<software>SomeScraper</software>
		<database>SomeScraper.io</database>
		<web>http://www.SomeScraper.io</web>
	</provider>
	<folder id=""0"" source=""SomeScraper.io"">
		<path>./Some Random Folder (Bigboy)</path>
		<name>Some Random Folder (Bigboy) (123)</name>
		<desc>Folder: gb/Some Random Folder (Bigboy)
Files: 123</desc>
		<image>./media/folders/Some Random Folder (Bigboy).png</image>
	</folder>
	<game id=""01234"" source=""SomeScraper.io"">
		<path>./Smoochies Boochies (Europe).gb</path>
		<name>Smoochies Boochies</name>
		<desc>Smoochies Boochies is board strategy game mixed with bootie battle confrontations.</desc>
		<rating>0.67</rating>
		<releasedate>19930101T000000</releasedate>
		<developer>Headsup</developer>
		<publisher>Headsup</publisher>
		<genre>Role Playing Game</genre>
		<players>1</players>
		<hash>AAABBCCSS</hash>
		<image>./images/Some Random Folder (Bigboy)/Smoochies Boochies.png</image>
		<thumbnail>./images/Some Random Folder (Bigboy)/Smoochies Boochies.png</thumbnail>
		<genreid>768</genreid>
	</game>
	<game>
		<path>./Translations (Bigboy)/Smoochies Boochies (Japan) [T-En by A TransLetters] [n].7z</path>
		<name>Smoochies Boochies (Japan) [T-En by A TransLetters] [n]</name>
		<lang>en</lang>
		<region>jp</region>
	</game>
</gameList>";
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Provider.ShouldNotBeNull();
        result.Value.Provider.System.ShouldBe("Big Boy");
        result.Value.Folders.Count.ShouldBe(1);
        result.Value.Games.Count.ShouldBe(2);

        var folder = result.Value.Folders[0];
        folder.Id.ShouldBe("0");
        folder.Name.ShouldBe("Some Random Folder (Bigboy) (123)");

        var game1 = result.Value.Games[0];
        game1.Id.ShouldBe("01234");
        game1.Name.ShouldBe("Smoochies Boochies");
        game1.Developer.ShouldBe("Headsup");
        game1.GenreId.ShouldBe("768");

        var game2 = result.Value.Games[1];
        game2.Id.ShouldBeNull();
        game2.Lang.ShouldBe("en");
        game2.Region.ShouldBe("jp");
    }

    [Test]
    public void Parse_WithGameWithoutPath_ParsesSuccessfully()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithName("Game Without Path"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Games.Count.ShouldBe(1);
        result.Value.Games[0].Name.ShouldBe("Game Without Path");
        result.Value.Games[0].Path.ShouldBeNullOrEmpty();
    }

    [Test]
    public void Parse_WithMultilineDescription_PreservesLineBreaks()
    {
        // Arrange
        var description = "Line 1\nLine 2\nLine 3";
        var xml = GamelistXmlBuilder.Create()
            .WithGame(g => g
                .WithPath("./game.rom")
                .WithDescription(description))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistParser.Parse(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Games[0].Desc.ShouldBe(description);
    }
}
