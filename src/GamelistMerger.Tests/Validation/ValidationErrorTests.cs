using GamelistMerger.Services.Validation;

namespace GamelistMerger.Tests.Validation;

[TestFixture]
public class ValidationErrorTests
{
    [Test]
    public void Constructor_WithMessageOnly_CreatesErrorWithNoLineNumberOrElement()
    {
        // Arrange
        var message = "Test error message";

        // Act
        var error = new ValidationError(message);

        // Assert
        error.Message.ShouldBe(message);
        error.LineNumber.ShouldBeNull();
        error.Element.ShouldBeNull();
    }

    [Test]
    public void Constructor_WithMessageAndLineNumber_CreatesErrorWithLineNumber()
    {
        // Arrange
        var message = "Test error message";
        var lineNumber = 42;

        // Act
        var error = new ValidationError(message, lineNumber);

        // Assert
        error.Message.ShouldBe(message);
        error.LineNumber.ShouldBe(lineNumber);
        error.Element.ShouldBeNull();
    }

    [Test]
    public void Constructor_WithAllParameters_CreatesErrorWithAllProperties()
    {
        // Arrange
        var message = "Test error message";
        var lineNumber = 42;
        var element = "game";

        // Act
        var error = new ValidationError(message, lineNumber, element);

        // Assert
        error.Message.ShouldBe(message);
        error.LineNumber.ShouldBe(lineNumber);
        error.Element.ShouldBe(element);
    }

    [Test]
    public void ToString_WithLineNumber_IncludesLinePrefix()
    {
        // Arrange
        var error = new ValidationError("Missing path element", 10, "game");

        // Act
        var result = error.ToString();

        // Assert
        result.ShouldBe("Line 10: Missing path element");
    }

    [Test]
    public void ToString_WithoutLineNumber_ExcludesLinePrefix()
    {
        // Arrange
        var error = new ValidationError("File not found");

        // Act
        var result = error.ToString();

        // Assert
        result.ShouldBe("File not found");
    }

    [Test]
    public void ToString_WithNullLineNumber_ExcludesLinePrefix()
    {
        // Arrange
        var error = new ValidationError("Generic error", null, "game");

        // Act
        var result = error.ToString();

        // Assert
        result.ShouldBe("Generic error");
    }

    [Test]
    public void Equality_WithSameValues_AreEqual()
    {
        // Arrange
        var error1 = new ValidationError("Test message", 10, "game");
        var error2 = new ValidationError("Test message", 10, "game");

        // Act & Assert
        error1.ShouldBe(error2);
        (error1 == error2).ShouldBeTrue();
    }

    [Test]
    public void Equality_WithDifferentMessages_AreNotEqual()
    {
        // Arrange
        var error1 = new ValidationError("Message 1", 10, "game");
        var error2 = new ValidationError("Message 2", 10, "game");

        // Act & Assert
        error1.ShouldNotBe(error2);
        (error1 != error2).ShouldBeTrue();
    }

    [Test]
    public void Equality_WithDifferentLineNumbers_AreNotEqual()
    {
        // Arrange
        var error1 = new ValidationError("Test message", 10, "game");
        var error2 = new ValidationError("Test message", 20, "game");

        // Act & Assert
        error1.ShouldNotBe(error2);
    }

    [Test]
    public void Equality_WithDifferentElements_AreNotEqual()
    {
        // Arrange
        var error1 = new ValidationError("Test message", 10, "game");
        var error2 = new ValidationError("Test message", 10, "folder");

        // Act & Assert
        error1.ShouldNotBe(error2);
    }

    [Test]
    public void GetHashCode_WithSameValues_ProducesSameHashCode()
    {
        // Arrange
        var error1 = new ValidationError("Test message", 10, "game");
        var error2 = new ValidationError("Test message", 10, "game");

        // Act
        var hash1 = error1.GetHashCode();
        var hash2 = error2.GetHashCode();

        // Assert
        hash1.ShouldBe(hash2);
    }
}
