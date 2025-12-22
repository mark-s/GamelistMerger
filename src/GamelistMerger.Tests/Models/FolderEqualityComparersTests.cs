using GamelistMerger.Models;
using GamelistMerger.Tests.TestHelpers;

namespace GamelistMerger.Tests.Models;

[TestFixture]
public class FolderEqualityComparersTests
{
    [TestFixture]
    public class ByPathTests
    {
        [Test]
        public void Equals_WithSamePath_ReturnsTrue()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;
            var folder1 = TestFolderBuilder.Create().WithId("1").WithName("Folder A").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("2").WithName("Folder B").WithPath("./rpg").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void Equals_WithDifferentPath_ReturnsFalse()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;
            var folder1 = TestFolderBuilder.Create().WithId("1").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("1").WithPath("./action").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public void Equals_IsCaseInsensitive()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;
            var folder1 = TestFolderBuilder.Create().WithPath("./RPG").Build();
            var folder2 = TestFolderBuilder.Create().WithPath("./rpg").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void Equals_WithSameReference_ReturnsTrue()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;
            var folder = TestFolderBuilder.Create().WithPath("./rpg").Build();

            // Act
            var result = comparer.Equals(folder, folder);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void Equals_WithBothNull_ReturnsTrue()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;

            // Act
            var result = comparer.Equals(null, null);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void Equals_WithOneNull_ReturnsFalse()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;
            var folder = TestFolderBuilder.Create().WithPath("./rpg").Build();

            // Act
            var result1 = comparer.Equals(folder, null);
            var result2 = comparer.Equals(null, folder);

            // Assert
            result1.ShouldBeFalse();
            result2.ShouldBeFalse();
        }

        [Test]
        public void GetHashCode_WithSamePath_ReturnsSameHashCode()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;
            var folder1 = TestFolderBuilder.Create().WithId("1").WithName("A").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("2").WithName("B").WithPath("./RPG").Build();

            // Act
            var hash1 = comparer.GetHashCode(folder1);
            var hash2 = comparer.GetHashCode(folder2);

            // Assert
            hash1.ShouldBe(hash2);
        }

        [Test]
        public void GetHashCode_WithNullPath_ReturnsZero()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;
            var folder = TestFolderBuilder.Create().Build();

            // Act
            var hash = comparer.GetHashCode(folder);

            // Assert
            hash.ShouldBe(0);
        }
    }

    [TestFixture]
    public class ByNameTests
    {
        [Test]
        public void Equals_WithSameName_ReturnsTrue()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByName;
            var folder1 = TestFolderBuilder.Create().WithId("1").WithName("RPG Games").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("2").WithName("RPG Games").WithPath("./action").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void Equals_WithDifferentName_ReturnsFalse()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByName;
            var folder1 = TestFolderBuilder.Create().WithName("RPG Games").Build();
            var folder2 = TestFolderBuilder.Create().WithName("Action Games").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public void Equals_IsCaseInsensitive()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByName;
            var folder1 = TestFolderBuilder.Create().WithName("RPG GAMES").Build();
            var folder2 = TestFolderBuilder.Create().WithName("rpg games").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void GetHashCode_WithSameName_ReturnsSameHashCode()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByName;
            var folder1 = TestFolderBuilder.Create().WithId("1").WithName("RPG Games").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("2").WithName("rpg games").WithPath("./action").Build();

            // Act
            var hash1 = comparer.GetHashCode(folder1);
            var hash2 = comparer.GetHashCode(folder2);

            // Assert
            hash1.ShouldBe(hash2);
        }
    }

    [TestFixture]
    public class ByIdTests
    {
        [Test]
        public void Equals_WithSameId_ReturnsTrue()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ById;
            var folder1 = TestFolderBuilder.Create().WithId("123").WithName("Folder A").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("123").WithName("Folder B").WithPath("./action").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void Equals_WithDifferentId_ReturnsFalse()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ById;
            var folder1 = TestFolderBuilder.Create().WithId("123").Build();
            var folder2 = TestFolderBuilder.Create().WithId("456").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public void Equals_IsCaseSensitive()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ById;
            var folder1 = TestFolderBuilder.Create().WithId("ABC").Build();
            var folder2 = TestFolderBuilder.Create().WithId("abc").Build();

            // Act
            var result = comparer.Equals(folder1, folder2);

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public void GetHashCode_WithSameId_ReturnsSameHashCode()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ById;
            var folder1 = TestFolderBuilder.Create().WithId("123").WithName("A").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("123").WithName("B").WithPath("./action").Build();

            // Act
            var hash1 = comparer.GetHashCode(folder1);
            var hash2 = comparer.GetHashCode(folder2);

            // Assert
            hash1.ShouldBe(hash2);
        }

        [Test]
        public void GetHashCode_WithDifferentCase_ReturnsDifferentHashCode()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ById;
            var folder1 = TestFolderBuilder.Create().WithId("ABC").Build();
            var folder2 = TestFolderBuilder.Create().WithId("abc").Build();

            // Act
            var hash1 = comparer.GetHashCode(folder1);
            var hash2 = comparer.GetHashCode(folder2);

            // Assert
            hash1.ShouldNotBe(hash2);
        }

        [Test]
        public void GetHashCode_WithNullId_ReturnsZero()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ById;
            var folder = TestFolderBuilder.Create().Build();

            // Act
            var hash = comparer.GetHashCode(folder);

            // Assert
            hash.ShouldBe(0);
        }
    }

    [TestFixture]
    public class UsageInCollectionsTests
    {
        [Test]
        public void ByPath_UsedInDictionary_WorksCorrectly()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByPath;
            var dict = new Dictionary<Folder, string>(comparer);

            var folder1 = TestFolderBuilder.Create().WithId("1").WithName("Folder A").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("2").WithName("Folder B").WithPath("./RPG").Build();
            var folder3 = TestFolderBuilder.Create().WithId("3").WithName("Folder C").WithPath("./action").Build();

            // Act
            dict[folder1] = "First";
            dict[folder2] = "Second";
            dict[folder3] = "Third";

            // Assert
            dict.Count.ShouldBe(2);
            dict[folder1].ShouldBe("Second");
            dict[folder3].ShouldBe("Third");
        }

        [Test]
        public void ByName_UsedInDictionary_WorksCorrectly()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ByName;
            var dict = new Dictionary<Folder, string>(comparer);

            var folder1 = TestFolderBuilder.Create().WithName("RPG Games").WithPath("./rpg1").Build();
            var folder2 = TestFolderBuilder.Create().WithName("rpg games").WithPath("./rpg2").Build();
            var folder3 = TestFolderBuilder.Create().WithName("Action Games").WithPath("./action").Build();

            // Act
            dict[folder1] = "First";
            dict[folder2] = "Second";
            dict[folder3] = "Third";

            // Assert
            dict.Count.ShouldBe(2);
            dict[folder1].ShouldBe("Second");
            dict[folder3].ShouldBe("Third");
        }

        [Test]
        public void ById_UsedInDictionary_WorksCorrectly()
        {
            // Arrange
            var comparer = FolderEqualityComparers.ById;
            var dict = new Dictionary<Folder, string>(comparer);

            var folder1 = TestFolderBuilder.Create().WithId("123").WithName("A").WithPath("./rpg").Build();
            var folder2 = TestFolderBuilder.Create().WithId("123").WithName("B").WithPath("./action").Build();
            var folder3 = TestFolderBuilder.Create().WithId("456").WithName("C").WithPath("./sports").Build();

            // Act
            dict[folder1] = "First";
            dict[folder2] = "Second";
            dict[folder3] = "Third";

            // Assert
            dict.Count.ShouldBe(2);
            dict[folder1].ShouldBe("Second");
            dict[folder3].ShouldBe("Third");
        }
    }
}
