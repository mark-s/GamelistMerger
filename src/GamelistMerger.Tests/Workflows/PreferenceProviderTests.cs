using GamelistMerger.Tests.TestHelpers;
using GamelistMerger.Workflows;

namespace GamelistMerger.Tests.Workflows;

[TestFixture]
public class PreferenceProviderTests
{
    [Test]
    public void PreferCompressedFile_WhenPreferArchivesIsFalse_ReturnsNull()
    {
        // Arrange
        var config = CreateConfig(preferArchives: false);
        var game1 = TestGameBuilder.Create().WithPath("./game1.zip").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.rom").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public void PreferCompressedFile_WhenPreferArchivesIsTrueAndGame1IsZip_ReturnsGame1()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("./game1.zip").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.rom").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game1);
    }

    [Test]
    public void PreferCompressedFile_WhenPreferArchivesIsTrueAndGame1Is7z_ReturnsGame1()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("./game1.7z").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.rom").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game1);
    }

    [Test]
    public void PreferCompressedFile_WhenPreferArchivesIsTrueAndGame2IsZip_ReturnsGame2()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("./game1.rom").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.zip").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game2);
    }

    [Test]
    public void PreferCompressedFile_WhenPreferArchivesIsTrueAndGame2Is7z_ReturnsGame2()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("./game1.rom").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.7z").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game2);
    }

    [Test]
    public void PreferCompressedFile_WhenPreferArchivesIsTrueAndNeitherIsCompressed_ReturnsGame1()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("./game1.rom").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.gb").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game1);
    }

    [Test]
    public void PreferCompressedFile_WhenPreferArchivesIsTrueAndBothAreCompressed_ReturnsGame1()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("./game1.zip").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.7z").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game1);
    }

    [Test]
    public void PreferCompressedFile_WhenGame1PathIsNull_ReturnsGame2IfCompressed()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath(null).Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.zip").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game2);
    }

    [Test]
    public void PreferCompressedFile_WhenGame1PathIsEmpty_ReturnsGame2IfCompressed()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.7z").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game2);
    }

    [Test]
    public void PreferCompressedFile_WhenGame1PathIsWhitespace_ReturnsGame2IfCompressed()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("   ").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.zip").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game2);
    }

    [Test]
    public void PreferCompressedFile_WhenBothPathsAreNull_ReturnsGame1()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath(null).Build();
        var game2 = TestGameBuilder.Create().WithPath(null).Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game1);
    }

    [Test]
    public void PreferCompressedFile_WhenExtensionIsMixedCase_RecognisesCompressedFile()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("./game1.ZIP").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.rom").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game1);
    }

    [Test]
    public void PreferCompressedFile_WhenExtensionIsUpperCase_RecognisesCompressedFile()
    {
        // Arrange
        var config = CreateConfig(preferArchives: true);
        var game1 = TestGameBuilder.Create().WithPath("./game1.rom").Build();
        var game2 = TestGameBuilder.Create().WithPath("./game2.7Z").Build();
        var preferenceFunc = PreferenceProvider.PreferCompressedFile(config);

        // Act
        var result = preferenceFunc(game1, game2);

        // Assert
        result.ShouldBe(game2);
    }

    private static AppConfig CreateConfig(bool preferArchives)
        => new AppConfig(
            GameListA: new FileInfo("gamelist-a.xml"),
            GameListB: new FileInfo("gamelist-b.xml"),
            OutputFile: new FileInfo("output.xml"),
            ExcludeBios: false,
            ExcludeNameContains: [],
            ExcludeRegions: [],
            IncludeLanguages: [],
            Verbose: false,
            Overwrite: false,
            PreferArchives: preferArchives,
            SortOutput: false);
}
