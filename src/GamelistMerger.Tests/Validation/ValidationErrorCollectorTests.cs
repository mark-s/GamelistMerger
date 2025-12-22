using GamelistMerger.Services.Validation;

namespace GamelistMerger.Tests.Validation;

[TestFixture]
public class ValidationErrorCollectorTests
{
    [Test]
    public void Constructor_InitializesEmptyCollection()
    {
        // Arrange & Act
        var collector = new ValidationErrorCollector();

        // Assert
        collector.Errors.ShouldBeEmpty();
        collector.HasErrors.ShouldBeFalse();
    }

    [Test]
    public void Add_WithMessageOnly_AddsErrorToCollection()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        collector.Add("Test error");

        // Assert
        collector.HasErrors.ShouldBeTrue();
        collector.Errors.Count.ShouldBe(1);
        collector.Errors[0].Message.ShouldBe("Test error");
        collector.Errors[0].LineNumber.ShouldBeNull();
        collector.Errors[0].Element.ShouldBeNull();
    }

    [Test]
    public void Add_WithMessageAndLineNumber_AddsErrorWithLineNumber()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        collector.Add("Test error", 42);

        // Assert
        collector.HasErrors.ShouldBeTrue();
        collector.Errors.Count.ShouldBe(1);
        collector.Errors[0].Message.ShouldBe("Test error");
        collector.Errors[0].LineNumber.ShouldBe(42);
        collector.Errors[0].Element.ShouldBeNull();
    }

    [Test]
    public void Add_WithAllParameters_AddsCompleteError()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        collector.Add("Missing path", 15, "game");

        // Assert
        collector.HasErrors.ShouldBeTrue();
        collector.Errors.Count.ShouldBe(1);
        collector.Errors[0].Message.ShouldBe("Missing path");
        collector.Errors[0].LineNumber.ShouldBe(15);
        collector.Errors[0].Element.ShouldBe("game");
    }

    [Test]
    public void Add_MultipleErrors_AddsAllErrorsInOrder()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        collector.Add("First error", 10, "game");
        collector.Add("Second error", 20, "folder");
        collector.Add("Third error", 30, "unknown");

        // Assert
        collector.HasErrors.ShouldBeTrue();
        collector.Errors.Count.ShouldBe(3);
        collector.Errors[0].Message.ShouldBe("First error");
        collector.Errors[0].LineNumber.ShouldBe(10);
        collector.Errors[0].Element.ShouldBe("game");
        collector.Errors[1].Message.ShouldBe("Second error");
        collector.Errors[1].LineNumber.ShouldBe(20);
        collector.Errors[1].Element.ShouldBe("folder");
        collector.Errors[2].Message.ShouldBe("Third error");
        collector.Errors[2].LineNumber.ShouldBe(30);
        collector.Errors[2].Element.ShouldBe("unknown");
    }

    [Test]
    public void HasErrors_WhenNoErrors_ReturnsFalse()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        var hasErrors = collector.HasErrors;

        // Assert
        hasErrors.ShouldBeFalse();
    }

    [Test]
    public void HasErrors_AfterAddingError_ReturnsTrue()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        collector.Add("Test error");

        // Assert
        collector.HasErrors.ShouldBeTrue();
    }

    [Test]
    public void Errors_ReturnsReadOnlyCollection()
    {
        // Arrange
        var collector = new ValidationErrorCollector();
        collector.Add("Test error");

        // Act
        var errors = collector.Errors;

        // Assert
        errors.ShouldBeAssignableTo<IReadOnlyList<ValidationError>>();
    }

    [Test]
    public void Add_WithNullLineNumber_AddsErrorWithNullLineNumber()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        collector.Add("Error without line", null, "game");

        // Assert
        collector.Errors[0].LineNumber.ShouldBeNull();
    }

    [Test]
    public void Add_WithNullElement_AddsErrorWithNullElement()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        collector.Add("Error without element", 10, null);

        // Assert
        collector.Errors[0].Element.ShouldBeNull();
    }

    [Test]
    public void Add_ManyErrors_MaintainsCorrectCount()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act
        for (var i = 0; i < 100; i++)
        {
            collector.Add($"Error {i}", i, "element");
        }

        // Assert
        collector.Errors.Count.ShouldBe(100);
        collector.HasErrors.ShouldBeTrue();
    }

    [Test]
    public void Errors_ReflectsCurrentState()
    {
        // Arrange
        var collector = new ValidationErrorCollector();

        // Act & Assert - Initially empty
        collector.Errors.Count.ShouldBe(0);

        // Act & Assert - After adding one
        collector.Add("First error");
        collector.Errors.Count.ShouldBe(1);

        // Act & Assert - After adding another
        collector.Add("Second error");
        collector.Errors.Count.ShouldBe(2);
    }
}
