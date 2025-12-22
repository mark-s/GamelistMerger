using GamelistMerger.Services.Validation;

namespace GamelistMerger.Tests.Validation;

[TestFixture]
public class ValidationSuccessTests
{
    [Test]
    public void Constructor_WithZeroCounts_CreatesSuccessWithZeroCounts()
    {
        // Arrange & Act
        var success = new ValidationSuccess(0, 0);

        // Assert
        success.GameCount.ShouldBe(0);
        success.FolderCount.ShouldBe(0);
    }

    [Test]
    public void Constructor_WithPositiveCounts_CreatesSuccessWithCounts()
    {
        // Arrange & Act
        var success = new ValidationSuccess(42, 7);

        // Assert
        success.GameCount.ShouldBe(42);
        success.FolderCount.ShouldBe(7);
    }

    [Test]
    public void ToString_WithZeroCounts_ReturnsCorrectFormat()
    {
        // Arrange
        var success = new ValidationSuccess(0, 0);

        // Act
        var result = success.ToString();

        // Assert
        result.ShouldBe("0 games, 0 folders");
    }

    [Test]
    public void ToString_WithSingleGame_ReturnsCorrectFormat()
    {
        // Arrange
        var success = new ValidationSuccess(1, 0);

        // Act
        var result = success.ToString();

        // Assert
        result.ShouldBe("1 games, 0 folders");
    }

    [Test]
    public void ToString_WithMultipleGamesAndFolders_ReturnsCorrectFormat()
    {
        // Arrange
        var success = new ValidationSuccess(150, 25);

        // Act
        var result = success.ToString();

        // Assert
        result.ShouldBe("150 games, 25 folders");
    }

    [Test]
    public void Equality_WithSameCounts_AreEqual()
    {
        // Arrange
        var success1 = new ValidationSuccess(10, 5);
        var success2 = new ValidationSuccess(10, 5);

        // Act & Assert
        success1.ShouldBe(success2);
        (success1 == success2).ShouldBeTrue();
    }

    [Test]
    public void Equality_WithDifferentGameCounts_AreNotEqual()
    {
        // Arrange
        var success1 = new ValidationSuccess(10, 5);
        var success2 = new ValidationSuccess(20, 5);

        // Act & Assert
        success1.ShouldNotBe(success2);
        (success1 != success2).ShouldBeTrue();
    }

    [Test]
    public void Equality_WithDifferentFolderCounts_AreNotEqual()
    {
        // Arrange
        var success1 = new ValidationSuccess(10, 5);
        var success2 = new ValidationSuccess(10, 15);

        // Act & Assert
        success1.ShouldNotBe(success2);
    }

    [Test]
    public void GetHashCode_WithSameCounts_ProducesSameHashCode()
    {
        // Arrange
        var success1 = new ValidationSuccess(10, 5);
        var success2 = new ValidationSuccess(10, 5);

        // Act
        var hash1 = success1.GetHashCode();
        var hash2 = success2.GetHashCode();

        // Assert
        hash1.ShouldBe(hash2);
    }

    [Test]
    public void Deconstruction_ReturnsCorrectValues()
    {
        // Arrange
        var success = new ValidationSuccess(42, 7);

        // Act
        var (gameCount, folderCount) = success;

        // Assert
        gameCount.ShouldBe(42);
        folderCount.ShouldBe(7);
    }
}
