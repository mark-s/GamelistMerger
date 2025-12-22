using GamelistMerger.Models;
using GamelistMerger.Services.Filtering;

namespace GamelistMerger.Tests.Services.Filtering;

[TestFixture]
public class CompositeComparerBuilderTests
{
    private Game CreateGame(string? id = null, string? path = null, string? name = null)
    {
        return new Game(
            Id: id,
            Source: null,
            Name: name,
            Desc: null,
            Image: null,
            Thumbnail: null,
            Path: path,
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

    [Test]
    public void Build_WithNoSelectors_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new CompositeComparerBuilder();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => builder.Build())
            .Message.ShouldBe("At least one property must be specified.");
    }

    [Test]
    public void Build_WithByPath_ComparesGamesByPath()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(path: "./game.rom");
        var game2 = CreateGame(path: "./game.rom");
        var game3 = CreateGame(path: "./different.rom");

        // Act
        var equals12 = comparer.Equals(game1, game2);
        var equals13 = comparer.Equals(game1, game3);

        // Assert
        equals12.ShouldBeTrue();
        equals13.ShouldBeFalse();
    }

    [Test]
    public void Build_WithByPath_IsCaseInsensitive()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(path: "./GAME.ROM");
        var game2 = CreateGame(path: "./game.rom");

        // Act
        var result = comparer.Equals(game1, game2);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void Build_WithById_ComparesGamesById()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById();
        var comparer = builder.Build();
        var game1 = CreateGame(id: "123");
        var game2 = CreateGame(id: "123");
        var game3 = CreateGame(id: "456");

        // Act
        var equals12 = comparer.Equals(game1, game2);
        var equals13 = comparer.Equals(game1, game3);

        // Assert
        equals12.ShouldBeTrue();
        equals13.ShouldBeFalse();
    }

    [Test]
    public void Build_WithById_IsCaseInsensitive()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById();
        var comparer = builder.Build();
        var game1 = CreateGame(id: "ABC123");
        var game2 = CreateGame(id: "abc123");

        // Act
        var result = comparer.Equals(game1, game2);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void Build_WithByIdThenByPath_PrioritisesId()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(id: "123", path: "./game1.rom");
        var game2 = CreateGame(id: "123", path: "./game2.rom");

        // Act
        var result = comparer.Equals(game1, game2);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void Build_WithByIdThenByPath_FallsBackToPathWhenIdIsNull()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(id: null, path: "./game.rom");
        var game2 = CreateGame(id: null, path: "./game.rom");
        var game3 = CreateGame(id: null, path: "./different.rom");

        // Act
        var equals12 = comparer.Equals(game1, game2);
        var equals13 = comparer.Equals(game1, game3);

        // Assert
        equals12.ShouldBeTrue();
        equals13.ShouldBeFalse();
    }

    [Test]
    public void Build_WithByIdThenByPath_FallsBackToPathWhenIdIsEmpty()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(id: "", path: "./game.rom");
        var game2 = CreateGame(id: "", path: "./game.rom");
        var game3 = CreateGame(id: "", path: "./different.rom");

        // Act
        var equals12 = comparer.Equals(game1, game2);
        var equals13 = comparer.Equals(game1, game3);

        // Assert
        equals12.ShouldBeTrue();
        equals13.ShouldBeFalse();
    }

    [Test]
    public void Build_WithBothPropertiesNull_ReturnsFalse()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(id: null, path: null);
        var game2 = CreateGame(id: null, path: null);

        // Act
        var result = comparer.Equals(game1, game2);

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void Build_WithNullGames_HandlesNull()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ByPath();
        var comparer = builder.Build();
        var game = CreateGame(path: "./game.rom");

        // Act
        var equalsNull1 = comparer.Equals(game, null);
        var equalsNull2 = comparer.Equals(null, game);
        var bothNull = comparer.Equals(null, null);

        // Assert
        equalsNull1.ShouldBeFalse();
        equalsNull2.ShouldBeFalse();
        bothNull.ShouldBeTrue();
    }

    [Test]
    public void Build_WithSameReference_ReturnsTrue()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ByPath();
        var comparer = builder.Build();
        var game = CreateGame(path: "./game.rom");

        // Act
        var result = comparer.Equals(game, game);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void GetHashCode_WithSamePath_ReturnsSameHashCode()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(path: "./game.rom");
        var game2 = CreateGame(path: "./GAME.ROM");

        // Act
        var hash1 = comparer.GetHashCode(game1);
        var hash2 = comparer.GetHashCode(game2);

        // Assert
        hash1.ShouldBe(hash2);
    }

    [Test]
    public void GetHashCode_WithSameId_ReturnsSameHashCode()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById();
        var comparer = builder.Build();
        var game1 = CreateGame(id: "123");
        var game2 = CreateGame(id: "123");

        // Act
        var hash1 = comparer.GetHashCode(game1);
        var hash2 = comparer.GetHashCode(game2);

        // Assert
        hash1.ShouldBe(hash2);
    }

    [Test]
    public void GetHashCode_WithByIdThenByPath_UsesIdForHash()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(id: "123", path: "./game1.rom");
        var game2 = CreateGame(id: "123", path: "./game2.rom");

        // Act
        var hash1 = comparer.GetHashCode(game1);
        var hash2 = comparer.GetHashCode(game2);

        // Assert
        hash1.ShouldBe(hash2);
    }

    [Test]
    public void GetHashCode_WithByIdThenByPath_FallsBackToPathWhenIdIsNull()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(id: null, path: "./game.rom");
        var game2 = CreateGame(id: null, path: "./GAME.ROM");

        // Act
        var hash1 = comparer.GetHashCode(game1);
        var hash2 = comparer.GetHashCode(game2);

        // Assert
        hash1.ShouldBe(hash2);
    }

    [Test]
    public void GetHashCode_WithAllPropertiesNull_ReturnsZero()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game = CreateGame(id: null, path: null);

        // Act
        var hash = comparer.GetHashCode(game);

        // Assert
        hash.ShouldBe(0);
    }

    [Test]
    public void Build_UsedInDictionary_WorksCorrectly()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var dict = new Dictionary<Game, string>(comparer);

        var game1 = CreateGame(id: "123", path: "./game.rom");
        var game2 = CreateGame(id: "123", path: "./different.rom");
        var game3 = CreateGame(id: null, path: "./game.rom");

        // Act
        dict[game1] = "First";
        dict[game2] = "Second";
        dict[game3] = "Third";

        // Assert
        dict.Count.ShouldBe(2);
        dict[game1].ShouldBe("Second");
        dict[game3].ShouldBe("Third");
    }

    [Test]
    public void Build_DefaultComparer_ComparesByHash_ThenByCrc32_ThenByPath()
    {
        // Arrange
        var comparer = GameEqualityComparers.Default;
        var game1 = CreateGame(id: "123", path: "./game.rom");
        var game2 = CreateGame(id: "123", path: "./different.rom");
        var game3 = CreateGame(id: null, path: "./game.rom");
        var game4 = CreateGame(id: null, path: "./game.rom");

        // Act
        var equals12 = comparer.Equals(game1, game2);
        var equals13 = comparer.Equals(game1, game3);
        var equals34 = comparer.Equals(game3, game4);

        // Assert
        // Default comparer uses Hash -> Crc32 -> Path, NOT Id
        equals12.ShouldBeFalse(); // Different paths, no hash/crc32
        equals13.ShouldBeTrue(); // Same paths
        equals34.ShouldBeTrue(); // Same paths
    }

    [Test]
    public void Build_WithOneGameHavingIdOtherHavingPath_ComparesCorrectly()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(id: "123", path: null);
        var game2 = CreateGame(id: null, path: "./game.rom");

        // Act
        var result = comparer.Equals(game1, game2);

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void Build_WithOneGameHavingBothProperties_ComparesById()
    {
        // Arrange
        var builder = new CompositeComparerBuilder().ById().ByPath();
        var comparer = builder.Build();
        var game1 = CreateGame(id: "123", path: "./game1.rom");
        var game2 = CreateGame(id: "456", path: "./game1.rom");

        // Act
        var result = comparer.Equals(game1, game2);

        // Assert
        result.ShouldBeFalse();
    }
}
