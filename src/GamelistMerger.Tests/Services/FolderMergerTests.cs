using GamelistMerger.Models;
using GamelistMerger.Services;

namespace GamelistMerger.Tests.Services;

[TestFixture]
public class FolderMergerTests
{
    [Test]
    public void Merge_WithBothFoldersHavingAllProperties_PrefersMasterValues()
    {
        // Arrange
        var master = new Folder(
            Id: "1",
            Source: "MasterSource",
            Name: "Master Name",
            Desc: "Master Description",
            Image: "./master-image.png",
            Thumbnail: "./master-thumb.png",
            Path: "./master-path");

        var secondary = new Folder(
            Id: "2",
            Source: "SecondarySource",
            Name: "Secondary Name",
            Desc: "Secondary Description",
            Image: "./secondary-image.png",
            Thumbnail: "./secondary-thumb.png",
            Path: "./secondary-path");

        // Act
        var result = FolderMerger.Merge(master, secondary);

        // Assert
        result.Id.ShouldBe("1");
        result.Source.ShouldBe("MasterSource");
        result.Name.ShouldBe("Master Name");
        result.Desc.ShouldBe("Master Description");
        result.Image.ShouldBe("./master-image.png");
        result.Thumbnail.ShouldBe("./master-thumb.png");
        result.Path.ShouldBe("./master-path");
    }

    [Test]
    public void Merge_WithMasterMissingProperties_FillsFromSecondary()
    {
        // Arrange
        var master = new Folder(
            Id: "1",
            Source: null,
            Name: "Master Name",
            Desc: null,
            Image: null,
            Thumbnail: null,
            Path: "./master-path");

        var secondary = new Folder(
            Id: "2",
            Source: "SecondarySource",
            Name: "Secondary Name",
            Desc: "Secondary Description",
            Image: "./secondary-image.png",
            Thumbnail: "./secondary-thumb.png",
            Path: "./secondary-path");

        // Act
        var result = FolderMerger.Merge(master, secondary);

        // Assert
        result.Id.ShouldBe("1");
        result.Source.ShouldBe("SecondarySource");
        result.Name.ShouldBe("Master Name");
        result.Desc.ShouldBe("Secondary Description");
        result.Image.ShouldBe("./secondary-image.png");
        result.Thumbnail.ShouldBe("./secondary-thumb.png");
        result.Path.ShouldBe("./master-path");
    }

    [Test]
    public void Merge_WithMasterEmptyStrings_FillsFromSecondary()
    {
        // Arrange
        var master = new Folder(
            Id: "1",
            Source: "",
            Name: "",
            Desc: "",
            Image: "",
            Thumbnail: "",
            Path: "");

        var secondary = new Folder(
            Id: "2",
            Source: "SecondarySource",
            Name: "Secondary Name",
            Desc: "Secondary Description",
            Image: "./secondary-image.png",
            Thumbnail: "./secondary-thumb.png",
            Path: "./secondary-path");

        // Act
        var result = FolderMerger.Merge(master, secondary);

        // Assert
        result.Id.ShouldBe("1");
        result.Source.ShouldBe("SecondarySource");
        result.Name.ShouldBe("Secondary Name");
        result.Desc.ShouldBe("Secondary Description");
        result.Image.ShouldBe("./secondary-image.png");
        result.Thumbnail.ShouldBe("./secondary-thumb.png");
        result.Path.ShouldBe("./secondary-path");
    }

    [Test]
    public void Merge_WithSecondaryMissingProperties_KeepsMasterValues()
    {
        // Arrange
        var master = new Folder(
            Id: "1",
            Source: "MasterSource",
            Name: "Master Name",
            Desc: "Master Description",
            Image: "./master-image.png",
            Thumbnail: "./master-thumb.png",
            Path: "./master-path");

        var secondary = new Folder(
            Id: "2",
            Source: null,
            Name: null,
            Desc: null,
            Image: null,
            Thumbnail: null,
            Path: null);

        // Act
        var result = FolderMerger.Merge(master, secondary);

        // Assert
        result.Id.ShouldBe("1");
        result.Source.ShouldBe("MasterSource");
        result.Name.ShouldBe("Master Name");
        result.Desc.ShouldBe("Master Description");
        result.Image.ShouldBe("./master-image.png");
        result.Thumbnail.ShouldBe("./master-thumb.png");
        result.Path.ShouldBe("./master-path");
    }

    [Test]
    public void Merge_WithPartialMasterProperties_MergesCorrectly()
    {
        // Arrange
        var master = new Folder(
            Id: "1",
            Source: "MasterSource",
            Name: null,
            Desc: "Master Description",
            Image: null,
            Thumbnail: "./master-thumb.png",
            Path: null);

        var secondary = new Folder(
            Id: "2",
            Source: "SecondarySource",
            Name: "Secondary Name",
            Desc: "Secondary Description",
            Image: "./secondary-image.png",
            Thumbnail: "./secondary-thumb.png",
            Path: "./secondary-path");

        // Act
        var result = FolderMerger.Merge(master, secondary);

        // Assert
        result.Id.ShouldBe("1");
        result.Source.ShouldBe("MasterSource");
        result.Name.ShouldBe("Secondary Name");
        result.Desc.ShouldBe("Master Description");
        result.Image.ShouldBe("./secondary-image.png");
        result.Thumbnail.ShouldBe("./master-thumb.png");
        result.Path.ShouldBe("./secondary-path");
    }

    [Test]
    public void Merge_AlwaysPreservesMasterId()
    {
        // Arrange
        var master = new Folder(
            Id: "master-id-123",
            Source: null,
            Name: null,
            Desc: null,
            Image: null,
            Thumbnail: null,
            Path: null);

        var secondary = new Folder(
            Id: "secondary-id-456",
            Source: "Source",
            Name: "Name",
            Desc: "Desc",
            Image: "Image",
            Thumbnail: "Thumb",
            Path: "Path");

        // Act
        var result = FolderMerger.Merge(master, secondary);

        // Assert
        result.Id.ShouldBe("master-id-123");
    }

    [Test]
    public void Merge_WithBothFoldersNull_ReturnsNullProperties()
    {
        // Arrange
        var master = new Folder(
            Id: null,
            Source: null,
            Name: null,
            Desc: null,
            Image: null,
            Thumbnail: null,
            Path: null);

        var secondary = new Folder(
            Id: null,
            Source: null,
            Name: null,
            Desc: null,
            Image: null,
            Thumbnail: null,
            Path: null);

        // Act
        var result = FolderMerger.Merge(master, secondary);

        // Assert
        result.Id.ShouldBeNull();
        result.Source.ShouldBeNull();
        result.Name.ShouldBeNull();
        result.Desc.ShouldBeNull();
        result.Image.ShouldBeNull();
        result.Thumbnail.ShouldBeNull();
        result.Path.ShouldBeNull();
    }

    [Test]
    public void Merge_WithTypicalScraperScenario_MergesCorrectly()
    {
        // Arrange
        var master = new Folder(
            Id: "0",
            Source: "SomeScraper.io",
            Name: "RPG Games (120)",
            Desc: "Folder: gb/RPG Games\nFiles: 120",
            Image: "./media/folders/RPG Games.png",
            Thumbnail: null,
            Path: "./RPG Games");

        var secondary = new Folder(
            Id: "1",
            Source: "SomeScraper.io",
            Name: null,
            Desc: null,
            Image: null,
            Thumbnail: "./media/folders/RPG Games-thumb.png",
            Path: "./RPG Games");

        // Act
        var result = FolderMerger.Merge(master, secondary);

        // Assert
        result.Id.ShouldBe("0");
        result.Source.ShouldBe("SomeScraper.io");
        result.Name.ShouldBe("RPG Games (120)");
        result.Desc.ShouldBe("Folder: gb/RPG Games\nFiles: 120");
        result.Image.ShouldBe("./media/folders/RPG Games.png");
        result.Thumbnail.ShouldBe("./media/folders/RPG Games-thumb.png");
        result.Path.ShouldBe("./RPG Games");
    }
}
