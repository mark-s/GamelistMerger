namespace GamelistMerger.Tests;

[TestFixture]
public class AppConfigParserTests
{
    [Test]
    public void ParseCommandLine_WithNoArguments_ReturnsFailureWithShowUsageIndicator()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AppConfigParser.ShowUsageIndicator);
    }

    [Test]
    public void ParseCommandLine_WithHelpFlag_ReturnsFailureWithShowUsageIndicator()
    {
        // Arrange
        var args = new[] { "-h" };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AppConfigParser.ShowUsageIndicator);
    }

    [Test]
    public void ParseCommandLine_WithLongHelpFlag_ReturnsFailureWithShowUsageIndicator()
    {
        // Arrange
        var args = new[] { "--help" };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AppConfigParser.ShowUsageIndicator);
    }

    [Test]
    public void ParseCommandLine_WithQuestionMarkFlag_ReturnsFailureWithShowUsageIndicator()
    {
        // Arrange
        var args = new[] { "-?" };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(AppConfigParser.ShowUsageIndicator);
    }

    [Test]
    public void ParseCommandLine_WithValidRequiredArguments_ReturnsSuccess()
    {
        // Arrange
        var args = new[]
        {
            "--gamelistA", "test1.xml",
            "--gamelistB", "test2.xml",
            "--output", "output.xml"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameListA.Name.ShouldBe("test1.xml");
        result.Value.GameListB.Name.ShouldBe("test2.xml");
        result.Value.OutputFile.Name.ShouldBe("output.xml");
    }

    [Test]
    public void ParseCommandLine_WithShortFlags_ReturnsSuccess()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameListA.Name.ShouldBe("test1.xml");
        result.Value.GameListB.Name.ShouldBe("test2.xml");
        result.Value.OutputFile.Name.ShouldBe("output.xml");
    }

    [Test]
    public void ParseCommandLine_WithMissingRequiredArgument_ReturnsFailure()
    {
        // Arrange
        var args = new[]
        {
            "--gamelistA", "test1.xml",
            "--output", "output.xml"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("gamelistB");
    }

    [Test]
    public void ParseCommandLine_WithExcludeBiosFlag_SetsExcludeBiosToTrue()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "--exclude-bios"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ExcludeBios.ShouldBeTrue();
    }

    [Test]
    public void ParseCommandLine_WithoutExcludeBiosFlag_SetsExcludeBiosToFalse()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ExcludeBios.ShouldBeFalse();
    }

    [Test]
    public void ParseCommandLine_WithExcludeNameContains_ParsesCommaSeparatedValues()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "--exclude-name-contains", "Beta,Alpha,Demo"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ExcludeNameContains.ShouldNotBeNull();
        result.Value.ExcludeNameContains.Length.ShouldBe(3);
        result.Value.ExcludeNameContains.ShouldContain("Beta");
        result.Value.ExcludeNameContains.ShouldContain("Alpha");
        result.Value.ExcludeNameContains.ShouldContain("Demo");
    }

    [Test]
    public void ParseCommandLine_WithExcludeNameContainsMultipleArgs_CombinesAllValues()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "--exclude-name-contains", "Beta",
            "--exclude-name-contains", "Alpha"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ExcludeNameContains.ShouldNotBeNull();
        result.Value.ExcludeNameContains.Length.ShouldBe(2);
        result.Value.ExcludeNameContains.ShouldContain("Beta");
        result.Value.ExcludeNameContains.ShouldContain("Alpha");
    }

    [Test]
    public void ParseCommandLine_WithExcludeRegion_ParsesCommaSeparatedValues()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "--exclude-region", "jp,eu,fr"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ExcludeRegions.ShouldNotBeNull();
        result.Value.ExcludeRegions.Length.ShouldBe(3);
        result.Value.ExcludeRegions.ShouldContain("jp");
        result.Value.ExcludeRegions.ShouldContain("eu");
        result.Value.ExcludeRegions.ShouldContain("fr");
    }

    [Test]
    public void ParseCommandLine_WithIncludeLang_ParsesCommaSeparatedValues()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "--include-lang", "en,us"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.IncludeLanguages.ShouldNotBeNull();
        result.Value.IncludeLanguages.Length.ShouldBe(2);
        result.Value.IncludeLanguages.ShouldContain("en");
        result.Value.IncludeLanguages.ShouldContain("us");
    }

    [Test]
    public void ParseCommandLine_WithVerboseFlag_SetsVerboseToTrue()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "--verbose"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Verbose.ShouldBeTrue();
    }

    [Test]
    public void ParseCommandLine_WithShortVerboseFlag_SetsVerboseToTrue()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "-v"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Verbose.ShouldBeTrue();
    }

    [Test]
    public void ParseCommandLine_WithAllOptions_ParsesAllCorrectly()
    {
        // Arrange
        var args = new[]
        {
            "-a", "input1.xml",
            "-b", "input2.xml",
            "-o", "merged.xml",
            "--exclude-bios",
            "--exclude-name-contains", "Beta,Demo",
            "--exclude-region", "jp",
            "--include-lang", "en",
            "-v"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameListA.Name.ShouldBe("input1.xml");
        result.Value.GameListB.Name.ShouldBe("input2.xml");
        result.Value.OutputFile.Name.ShouldBe("merged.xml");
        result.Value.ExcludeBios.ShouldBeTrue();
        result.Value.ExcludeNameContains.ShouldNotBeNull();
        result.Value.ExcludeNameContains.Length.ShouldBe(2);
        result.Value.ExcludeRegions.ShouldNotBeNull();
        result.Value.ExcludeRegions.Length.ShouldBe(1);
        result.Value.IncludeLanguages.ShouldNotBeNull();
        result.Value.IncludeLanguages.Length.ShouldBe(1);
        result.Value.Verbose.ShouldBeTrue();
    }

    [Test]
    public void ParseCommandLine_WithCommaSeparatedValuesWithSpaces_TrimsValues()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "--exclude-name-contains", "Beta , Alpha , Demo"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ExcludeNameContains.ShouldNotBeNull();
        result.Value.ExcludeNameContains.Length.ShouldBe(3);
        result.Value.ExcludeNameContains.ShouldContain("Beta");
        result.Value.ExcludeNameContains.ShouldContain("Alpha");
        result.Value.ExcludeNameContains.ShouldContain("Demo");
    }

    [Test]
    public void ParseCommandLine_WithSortOutputFlag_SetsSortOutputToTrue()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml",
            "--sortOutput"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.SortOutput.ShouldBeTrue();
    }

    [Test]
    public void ParseCommandLine_WithoutSortOutputFlag_SetsSortOutputToFalse()
    {
        // Arrange
        var args = new[]
        {
            "-a", "test1.xml",
            "-b", "test2.xml",
            "-o", "output.xml"
        };

        // Act
        var result = AppConfigParser.ParseCommandLine(args);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.SortOutput.ShouldBeFalse();
    }
}
