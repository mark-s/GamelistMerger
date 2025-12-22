using GamelistMerger.Services.Filtering;

namespace GamelistMerger.Tests.Services.Filtering;

[TestFixture]
public class CliFilterArgsParserTests
{
    private static FilterConfig GetFilterConfig(
        bool excludeBios = false,
        string[]? excludeNameContains = null,
        string[]? excludeRegions = null,
        string[]? includeLanguages = null) =>
        CliFilterArgsParser.GetFilterConfig(
            excludeBios,
            excludeNameContains ?? [],
            excludeRegions ?? [],
            includeLanguages ?? []);

    [TestFixture]
    public class EmptyConfigTests
    {
        [Test]
        public void GetFilterConfig_WithNoOptions_ReturnsEmptyConfig()
        {
            // Act
            var config = GetFilterConfig();

            // Assert
            config.ExcludeRules.Length.ShouldBe(0);
            config.IncludeRules.Length.ShouldBe(0);
        }

        [Test]
        public void GetFilterConfig_WithEmptyExcludeNameContains_DoesNotCreateRule()
        {
            // Act
            var config = GetFilterConfig(excludeNameContains: []);

            // Assert
            config.ExcludeRules.Length.ShouldBe(0);
        }

        [Test]
        public void GetFilterConfig_WithEmptyExcludeRegions_DoesNotCreateRule()
        {
            // Act
            var config = GetFilterConfig(excludeRegions: []);

            // Assert
            config.ExcludeRules.Length.ShouldBe(0);
        }

        [Test]
        public void GetFilterConfig_WithEmptyIncludeLanguages_DoesNotCreateRule()
        {
            // Act
            var config = GetFilterConfig(includeLanguages: []);

            // Assert
            config.IncludeRules.Length.ShouldBe(0);
        }
    }

    [TestFixture]
    public class ExcludeBiosTests
    {
        [Test]
        public void GetFilterConfig_WithExcludeBios_CreatesExcludeRule()
        {
            // Act
            var config = GetFilterConfig(excludeBios: true);

            // Assert
            config.ExcludeRules.Length.ShouldBe(1);
            config.ExcludeRules[0].Property.ShouldBe(FilterProperty.Name);
            config.ExcludeRules[0].Operation.ShouldBe(FilterOperation.Contains);
            config.ExcludeRules[0].Values.ShouldContain("[BIOS]");
            config.ExcludeRules[0].Values.ShouldContain("(BIOS)");
        }

        [Test]
        public void GetFilterConfig_WithExcludeBiosAndOtherNameExclusions_CreatesSeparateRules()
        {
            // Act
            var config = GetFilterConfig(excludeBios: true, excludeNameContains: ["Sample"]);

            // Assert
            config.ExcludeRules.Length.ShouldBe(2);

            var biosRule = config.ExcludeRules.FirstOrDefault(r => r.Values.Contains("[BIOS]"));
            biosRule.ShouldNotBeNull();
            biosRule.Values.ShouldContain("[BIOS]");
            biosRule.Values.ShouldContain("(BIOS)");

            var sampleRule = config.ExcludeRules.FirstOrDefault(r => r.Values.Contains("Sample"));
            sampleRule.ShouldNotBeNull();
            sampleRule.Values.Length.ShouldBe(1);
        }
    }

    [TestFixture]
    public class ExcludeNameContainsTests
    {
        [Test]
        public void GetFilterConfig_WithExcludeNameContains_CreatesExcludeRule()
        {
            // Act
            var config = GetFilterConfig(excludeNameContains: ["Beta", "Alpha", "Demo"]);

            // Assert
            config.ExcludeRules.Length.ShouldBe(1);
            config.ExcludeRules[0].Property.ShouldBe(FilterProperty.Name);
            config.ExcludeRules[0].Operation.ShouldBe(FilterOperation.Contains);
            config.ExcludeRules[0].Values.Length.ShouldBe(3);
            config.ExcludeRules[0].Values.ShouldContain("Beta");
            config.ExcludeRules[0].Values.ShouldContain("Alpha");
            config.ExcludeRules[0].Values.ShouldContain("Demo");
        }

        [Test]
        public void GetFilterConfig_WithSingleExcludeNameValue_CreatesRuleWithOneValue()
        {
            // Act
            var config = GetFilterConfig(excludeNameContains: ["Proto"]);

            // Assert
            config.ExcludeRules.Length.ShouldBe(1);
            config.ExcludeRules[0].Values.Length.ShouldBe(1);
            config.ExcludeRules[0].Values[0].ShouldBe("Proto");
        }
    }

    [TestFixture]
    public class ExcludeRegionsTests
    {
        [Test]
        public void GetFilterConfig_WithExcludeRegions_CreatesExcludeRule()
        {
            // Act
            var config = GetFilterConfig(excludeRegions: ["jp", "eu", "fr"]);

            // Assert
            config.ExcludeRules.Length.ShouldBe(1);
            config.ExcludeRules[0].Property.ShouldBe(FilterProperty.Region);
            config.ExcludeRules[0].Operation.ShouldBe(FilterOperation.In);
            config.ExcludeRules[0].Values.Length.ShouldBe(3);
            config.ExcludeRules[0].Values.ShouldContain("jp");
            config.ExcludeRules[0].Values.ShouldContain("eu");
            config.ExcludeRules[0].Values.ShouldContain("fr");
        }
    }

    [TestFixture]
    public class IncludeLanguagesTests
    {
        [Test]
        public void GetFilterConfig_WithIncludeLanguages_CreatesIncludeRule()
        {
            // Act
            var config = GetFilterConfig(includeLanguages: ["en", "us"]);

            // Assert
            config.IncludeRules.Length.ShouldBe(1);
            config.IncludeRules[0].Property.ShouldBe(FilterProperty.Lang);
            config.IncludeRules[0].Operation.ShouldBe(FilterOperation.In);
            config.IncludeRules[0].Values.Length.ShouldBe(2);
            config.IncludeRules[0].Values.ShouldContain("en");
            config.IncludeRules[0].Values.ShouldContain("us");
        }
    }

    [TestFixture]
    public class CombinedOptionsTests
    {
        [Test]
        public void GetFilterConfig_WithMultipleExcludeOptions_CreatesMultipleRules()
        {
            // Act
            var config = GetFilterConfig(
                excludeBios: true,
                excludeNameContains: ["Beta"],
                excludeRegions: ["jp"]);

            // Assert
            config.ExcludeRules.Length.ShouldBe(3);
            config.ExcludeRules.ShouldContain(r => r.Property == FilterProperty.Name && r.Values.Contains("[BIOS]"));
            config.ExcludeRules.ShouldContain(r => r.Property == FilterProperty.Name && r.Values.Contains("Beta"));
            config.ExcludeRules.ShouldContain(r => r.Property == FilterProperty.Region && r.Values.Contains("jp"));
        }

        [Test]
        public void GetFilterConfig_WithAllOptions_CreatesAllRules()
        {
            // Act
            var config = GetFilterConfig(
                excludeBios: true,
                excludeNameContains: ["Beta", "Alpha"],
                excludeRegions: ["jp", "eu"],
                includeLanguages: ["en"]);

            // Assert
            config.ExcludeRules.Length.ShouldBe(3);
            config.IncludeRules.Length.ShouldBe(1);

            config.ExcludeRules.ShouldContain(r => r.Property == FilterProperty.Name && r.Values.Contains("[BIOS]"));
            config.ExcludeRules.ShouldContain(r => r.Property == FilterProperty.Name && r.Values.Contains("Beta"));
            config.ExcludeRules.ShouldContain(r => r.Property == FilterProperty.Region);

            config.IncludeRules.ShouldContain(r => r.Property == FilterProperty.Lang && r.Values.Contains("en"));
        }

        [Test]
        public void GetFilterConfig_RealWorldScenario_CreatesCorrectConfig()
        {
            // Act
            var config = GetFilterConfig(
                excludeBios: true,
                excludeNameContains: ["Beta", "Alpha", "Demo", "Sample"],
                excludeRegions: ["jp", "eu", "fr", "de"],
                includeLanguages: ["en", "us"]);

            // Assert
            config.ExcludeRules.Length.ShouldBe(3);
            config.IncludeRules.Length.ShouldBe(1);

            var biosRule = config.ExcludeRules.First(r => r.Values.Contains("[BIOS]"));
            biosRule.Property.ShouldBe(FilterProperty.Name);
            biosRule.Operation.ShouldBe(FilterOperation.Contains);

            var nameRule = config.ExcludeRules.First(r => r.Values.Contains("Beta"));
            nameRule.Property.ShouldBe(FilterProperty.Name);
            nameRule.Operation.ShouldBe(FilterOperation.Contains);
            nameRule.Values.Length.ShouldBe(4);

            var regionRule = config.ExcludeRules.First(r => r.Property == FilterProperty.Region);
            regionRule.Operation.ShouldBe(FilterOperation.In);
            regionRule.Values.Length.ShouldBe(4);

            var langRule = config.IncludeRules.First();
            langRule.Property.ShouldBe(FilterProperty.Lang);
            langRule.Operation.ShouldBe(FilterOperation.In);
            langRule.Values.Length.ShouldBe(2);
        }
    }
}
