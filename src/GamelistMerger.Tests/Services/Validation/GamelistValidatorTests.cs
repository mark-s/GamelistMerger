using System.Xml.Linq;
using GamelistMerger.Services.Validation;
using GamelistMerger.Tests.TestHelpers;

namespace GamelistMerger.Tests.Services.Validation;

[TestFixture]
public class GamelistValidatorTests
{
    [Test]
    public void ValidateDocument_WithValidEmptyGamelist_ReturnsSuccess()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create().Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameCount.ShouldBe(0);
        result.Value.FolderCount.ShouldBe(0);
    }

    [Test]
    public void ValidateDocument_WithNoRootElement_ReturnsFailure()
    {
        // Arrange
        var document = new XDocument();

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Count.ShouldBe(1);
        result.Error[0].Message.ShouldBe("XML document has no root element");
    }

    [Test]
    public void ValidateDocument_WithInvalidRootElement_ReturnsFailure()
    {
        // Arrange
        var xml = "<invalidRoot><game><path>./game.rom</path></game></invalidRoot>";
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Count.ShouldBe(1);
        result.Error[0].Message.ShouldContain("Root element must be <gameList>");
        result.Error[0].Message.ShouldContain("found <invalidRoot>");
    }

    [Test]
    public void ValidateDocument_WithValidGames_ReturnsSuccessWithCount()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithPath("./game1.rom").WithName("Game 1"))
            .WithGame(g => g.WithPath("./game2.rom").WithName("Game 2"))
            .WithGame(g => g.WithPath("./game3.rom").WithName("Game 3"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameCount.ShouldBe(3);
        result.Value.FolderCount.ShouldBe(0);
    }

    [Test]
    public void ValidateDocument_WithValidFolders_ReturnsSuccessWithCount()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithFolder(f => f.WithPath("./folder1").WithName("Folder 1"))
            .WithFolder(f => f.WithPath("./folder2").WithName("Folder 2"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameCount.ShouldBe(0);
        result.Value.FolderCount.ShouldBe(2);
    }

    [Test]
    public void ValidateDocument_WithGameMissingPath_ReturnsFailure()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithName("Game Without Path"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Count.ShouldBe(1);
        result.Error[0].Message.ShouldContain("<game> element missing required <path> child");
    }

    [Test]
    public void ValidateDocument_WithMultipleGamesMissingPath_ReturnsMultipleErrors()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<gameList>
    <game>
        <name>Game 1 Without Path</name>
    </game>
    <game>
        <path>./game2.rom</path>
        <name>Game 2 With Path</name>
    </game>
    <game>
        <name>Game 3 Without Path</name>
    </game>
</gameList>";
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Count.ShouldBe(2);
        result.Error.ShouldAllBe(e => e.Message.Contains("<game> element missing required <path> child"));
    }

    [Test]
    public void ValidateDocument_WithFolderMissingPath_ReturnsFailure()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithFolder(f => f.WithName("Folder Without Path"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Count.ShouldBe(1);
        result.Error[0].Message.ShouldContain("<folder> element missing required <path> child");
    }

    [Test]
    public void ValidateDocument_WithUnknownElement_ReturnsFailure()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithGame(g => g.WithPath("./game.rom").WithName("Valid Game"))
            .WithUnknownElement("customElement", "Some content")
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Count.ShouldBe(1);
        result.Error[0].Message.ShouldContain("Unknown element <customElement> under <gameList>");
    }

    [Test]
    public void ValidateDocument_WithMultipleUnknownElements_ReturnsMultipleErrors()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<gameList>
    <game>
        <path>./game.rom</path>
    </game>
    <unknown1>Content</unknown1>
    <unknown2>Content</unknown2>
</gameList>";
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Count.ShouldBe(2);
        result.Error.ShouldContain(e => e.Message.Contains("Unknown element <unknown1>"));
        result.Error.ShouldContain(e => e.Message.Contains("Unknown element <unknown2>"));
    }

    [Test]
    public void ValidateDocument_WithValidProvider_ReturnsSuccess()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithProvider("Big Boy", "SomeScraper", "SomeScraper.io", "https://www.SomeScraper.io")
            .WithGame(g => g.WithPath("./game.rom").WithName("Game"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public void ValidateDocument_WithMixedValidAndInvalidElements_ReturnsFailureWithAllErrors()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<gameList>
    <provider>
        <System>Big Boy</System>
    </provider>
    <game>
        <path>./game1.rom</path>
        <name>Valid Game</name>
    </game>
    <game>
        <name>Invalid Game - Missing Path</name>
    </game>
    <unknownElement>Content</unknownElement>
    <folder>
        <name>Invalid Folder - Missing Path</name>
    </folder>
</gameList>";
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Count.ShouldBe(3);
        result.Error.ShouldContain(e => e.Message.Contains("<game> element missing required <path> child"));
        result.Error.ShouldContain(e => e.Message.Contains("Unknown element <unknownElement>"));
        result.Error.ShouldContain(e => e.Message.Contains("<folder> element missing required <path> child"));
    }

    [Test]
    public void ValidateDocument_WithRealWorldExample_ReturnsSuccess()
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
		<path>./All but the Best (Bigboy)</path>
		<name>All but the Best (Bigboy) (512)</name>
	</folder>
	<game id=""01234"" source=""SomeScraper.io"">
		<path>./Castle Quest (Europe).gb</path>
		<name>Castle Quest</name>
		<rating>0.65</rating>
	</game>
	<game>
		<path>./Momotaro.7z</path>
		<name>Momotaro Thunderbolt</name>
		<lang>en</lang>
		<region>jp</region>
	</game>
</gameList>";
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameCount.ShouldBe(2);
        result.Value.FolderCount.ShouldBe(1);
    }

    [Test]
    public void ValidateDocument_WithOnlyProvider_ReturnsSuccessWithZeroCounts()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithProvider("Big Boy", "SomeScraper", "SomeScraper.io", "https://www.SomeScraper.io")
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameCount.ShouldBe(0);
        result.Value.FolderCount.ShouldBe(0);
    }

    [Test]
    public void ValidateDocument_WithValidGameAndFolder_ReturnsSuccessWithCorrectCounts()
    {
        // Arrange
        var xml = GamelistXmlBuilder.Create()
            .WithFolder(f => f.WithPath("./folder").WithName("Test Folder"))
            .WithGame(g => g.WithPath("./game1.rom").WithName("Game 1"))
            .WithGame(g => g.WithPath("./game2.rom").WithName("Game 2"))
            .Build();
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameCount.ShouldBe(2);
        result.Value.FolderCount.ShouldBe(1);
    }

    [Test]
    public void ValidateDocument_WithGameHavingEmptyPath_ReturnsSuccess()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<gameList>
    <game>
        <path></path>
        <name>Game With Empty Path</name>
    </game>
</gameList>";
        var document = XDocument.Parse(xml);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public void ValidateDocument_ErrorIncludesLineNumber_WhenAvailable()
    {
        // Arrange
        var xml = @"<?xml version=""1.0""?>
<gameList>
    <game>
        <name>Game Without Path</name>
    </game>
</gameList>";
        var document = XDocument.Parse(xml, LoadOptions.SetLineInfo);

        // Act
        var result = GamelistValidator.ValidateDocument(document);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error[0].LineNumber.ShouldNotBeNull();
    }
}
