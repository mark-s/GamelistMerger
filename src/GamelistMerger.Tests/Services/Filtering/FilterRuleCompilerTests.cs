using System.Collections.Immutable;
using GamelistMerger.Models;
using GamelistMerger.Services.Filtering;
using GamelistMerger.Tests.TestHelpers;

namespace GamelistMerger.Tests.Services.Filtering;

[TestFixture]
public class FilterRuleCompilerTests
{
    private static Game CreateTestGame(
        string? name = null,
        string? path = null,
        string? region = null,
        string? lang = null,
        string? genre = null,
        string? developer = null,
        string? publisher = null,
        string? image = null,
        string? description = null,
        string? rating = null,
        string? hash = null,
        string? crc32 = null,
        string? id = null,
        string? source = null) =>
        TestGameBuilder.Create()
            .WithId(id)
            .WithSource(source)
            .WithName(name)
            .WithDescription(description)
            .WithImage(image)
            .WithPath(path)
            .WithRating(rating)
            .WithDeveloper(developer)
            .WithPublisher(publisher)
            .WithGenre(genre)
            .WithHash(hash)
            .WithCrc32(crc32)
            .WithLang(lang)
            .WithRegion(region)
            .Build();

    [Test]
    public void Compile_WithNoRules_IncludesAllGames()
    {
        // Arrange
        var config = new FilterConfig(
            ExcludeRules: ImmutableArray<FilterRule>.Empty,
            IncludeRules: ImmutableArray<FilterRule>.Empty);
        var game = CreateTestGame(name: "Test Game");

        // Act
        var filter = FilterRuleCompiler.Compile(config);
        var result = filter(game);

        // Assert
        result.ShouldBeTrue();
    }

    [TestFixture]
    public class ExcludeRulesTests
    {
        [Test]
        public void Compile_WithExcludeNameContains_ExcludesMatchingGames()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["[BIOS]"]);
            var config = new FilterConfig(
                ExcludeRules: [rule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var biosGame = CreateTestGame(name: "System [BIOS]");
            var normalGame = CreateTestGame(name: "Normal Game");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(biosGame).ShouldBeFalse();
            filter(normalGame).ShouldBeTrue();
        }

        [Test]
        public void Compile_WithExcludeNameContains_IsCaseInsensitive()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["bios"]);
            var config = new FilterConfig(
                ExcludeRules: [rule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var biosGame = CreateTestGame(name: "System [BIOS]");

            // Act
            var filter = FilterRuleCompiler.Compile(config);
            var result = filter(biosGame);

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public void Compile_WithExcludeRegion_ExcludesMatchingGames()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Region, FilterOperation.In, ["jp", "eu"]);
            var config = new FilterConfig(
                ExcludeRules: [rule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var jpGame = CreateTestGame(region: "jp");
            var usGame = CreateTestGame(region: "us");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(jpGame).ShouldBeFalse();
            filter(usGame).ShouldBeTrue();
        }

        [Test]
        public void Compile_WithMultipleExcludeRules_ExcludesIfAnyMatches()
        {
            // Arrange
            var nameRule = new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["[BIOS]"]);
            var regionRule = new FilterRule(FilterProperty.Region, FilterOperation.In, ["jp"]);
            var config = new FilterConfig(
                ExcludeRules: [nameRule, regionRule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var biosGame = CreateTestGame(name: "System [BIOS]", region: "us");
            var jpGame = CreateTestGame(name: "Normal Game", region: "jp");
            var normalGame = CreateTestGame(name: "Normal Game", region: "us");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(biosGame).ShouldBeFalse();
            filter(jpGame).ShouldBeFalse();
            filter(normalGame).ShouldBeTrue();
        }

        [Test]
        public void Compile_WithNullPropertyValue_ReturnsTrueForExcludeRules()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Developer, FilterOperation.Contains, ["Test"]);
            var config = new FilterConfig(
                ExcludeRules: [rule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var gameWithNullDeveloper = CreateTestGame(developer: null);

            // Act
            var filter = FilterRuleCompiler.Compile(config);
            var result = filter(gameWithNullDeveloper);

            // Assert
            result.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class IncludeRulesTests
    {
        [Test]
        public void Compile_WithIncludeLanguage_IncludesOnlyMatchingGames()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Lang, FilterOperation.In, ["en", "us"]);
            var config = new FilterConfig(
                ExcludeRules: ImmutableArray<FilterRule>.Empty,
                IncludeRules: [rule]);
            var enGame = CreateTestGame(lang: "en");
            var jpGame = CreateTestGame(lang: "jp");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(enGame).ShouldBeTrue();
            filter(jpGame).ShouldBeFalse();
        }

        [Test]
        public void Compile_WithMultipleIncludeRules_IncludesOnlyIfAllMatch()
        {
            // Arrange
            var langRule = new FilterRule(FilterProperty.Lang, FilterOperation.In, ["en"]);
            var regionRule = new FilterRule(FilterProperty.Region, FilterOperation.In, ["us"]);
            var config = new FilterConfig(
                ExcludeRules: ImmutableArray<FilterRule>.Empty,
                IncludeRules: [langRule, regionRule]);
            var enUsGame = CreateTestGame(lang: "en", region: "us");
            var enEuGame = CreateTestGame(lang: "en", region: "eu");
            var jpUsGame = CreateTestGame(lang: "jp", region: "us");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(enUsGame).ShouldBeTrue();
            filter(enEuGame).ShouldBeFalse();
            filter(jpUsGame).ShouldBeFalse();
        }

        [Test]
        public void Compile_WithHasValue_IncludesGamesWithNonEmptyValue()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Image, FilterOperation.HasValue, []);
            var config = new FilterConfig(
                ExcludeRules: ImmutableArray<FilterRule>.Empty,
                IncludeRules: [rule]);
            var gameWithImage = CreateTestGame(image: "./image.png");
            var gameWithoutImage = CreateTestGame(image: null);
            var gameWithEmptyImage = CreateTestGame(image: "");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(gameWithImage).ShouldBeTrue();
            filter(gameWithoutImage).ShouldBeFalse();
            filter(gameWithEmptyImage).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class CombinedRulesTests
    {
        [Test]
        public void Compile_WithExcludeAndIncludeRules_ExcludeTakesPrecedence()
        {
            // Arrange
            var excludeRule = new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["[BIOS]"]);
            var includeRule = new FilterRule(FilterProperty.Lang, FilterOperation.In, ["en"]);
            var config = new FilterConfig(
                ExcludeRules: [excludeRule],
                IncludeRules: [includeRule]);
            var enBiosGame = CreateTestGame(name: "System [BIOS]", lang: "en");
            var enGame = CreateTestGame(name: "Normal Game", lang: "en");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(enBiosGame).ShouldBeFalse();
            filter(enGame).ShouldBeTrue();
        }

        [Test]
        public void Compile_WithComplexRealWorldScenario_FiltersCorrectly()
        {
            // Arrange
            var excludeBios = new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["[BIOS]", "(BIOS)"]);
            var excludeBeta = new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["Beta", "Alpha"]);
            var excludeRegions = new FilterRule(FilterProperty.Region, FilterOperation.In, ["jp", "eu"]);
            var includeLanguage = new FilterRule(FilterProperty.Lang, FilterOperation.In, ["en", "us"]);

            var config = new FilterConfig(
                ExcludeRules: [excludeBios, excludeBeta, excludeRegions],
                IncludeRules: [includeLanguage]);

            var biosGame = CreateTestGame(name: "System [BIOS]", lang: "en", region: "us");
            var betaGame = CreateTestGame(name: "Game Beta", lang: "en", region: "us");
            var jpGame = CreateTestGame(name: "Normal Game", lang: "en", region: "jp");
            var frLangGame = CreateTestGame(name: "Normal Game", lang: "fr", region: "us");
            var validGame = CreateTestGame(name: "Normal Game", lang: "en", region: "us");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(biosGame).ShouldBeFalse();
            filter(betaGame).ShouldBeFalse();
            filter(jpGame).ShouldBeFalse();
            filter(frLangGame).ShouldBeFalse();
            filter(validGame).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class OperationTests
    {
        [Test]
        public void Compile_WithStartsWith_MatchesCorrectly()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Name, FilterOperation.StartsWith, ["Test", "Demo"]);
            var config = new FilterConfig(
                ExcludeRules: [rule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var testGame = CreateTestGame(name: "Test Game");
            var demoGame = CreateTestGame(name: "Demo Version");
            var normalGame = CreateTestGame(name: "Normal Game");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(testGame).ShouldBeFalse();
            filter(demoGame).ShouldBeFalse();
            filter(normalGame).ShouldBeTrue();
        }

        [Test]
        public void Compile_WithEndsWith_MatchesCorrectly()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Path, FilterOperation.EndsWith, [".zip", ".7z"]);
            var config = new FilterConfig(
                ExcludeRules: [rule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var zipGame = CreateTestGame(path: "./game.zip");
            var sevenZGame = CreateTestGame(path: "./game.7z");
            var romGame = CreateTestGame(path: "./game.rom");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(zipGame).ShouldBeFalse();
            filter(sevenZGame).ShouldBeFalse();
            filter(romGame).ShouldBeTrue();
        }

        [Test]
        public void Compile_WithEquals_MatchesExactly()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Genre, FilterOperation.Equals, ["Sports", "Racing"]);
            var config = new FilterConfig(
                ExcludeRules: [rule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var sportsGame = CreateTestGame(genre: "Sports");
            var racingGame = CreateTestGame(genre: "Racing");
            var rpgGame = CreateTestGame(genre: "RPG");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(sportsGame).ShouldBeFalse();
            filter(racingGame).ShouldBeFalse();
            filter(rpgGame).ShouldBeTrue();
        }

        [Test]
        public void Compile_WithInOperation_BehavesLikeEquals()
        {
            // Arrange
            var rule = new FilterRule(FilterProperty.Region, FilterOperation.In, ["us", "eu"]);
            var config = new FilterConfig(
                ExcludeRules: [rule],
                IncludeRules: ImmutableArray<FilterRule>.Empty);
            var usGame = CreateTestGame(region: "us");
            var euGame = CreateTestGame(region: "eu");
            var jpGame = CreateTestGame(region: "jp");

            // Act
            var filter = FilterRuleCompiler.Compile(config);

            // Assert
            filter(usGame).ShouldBeFalse();
            filter(euGame).ShouldBeFalse();
            filter(jpGame).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class AllPropertiesTests
    {
        [Test]
        public void Compile_WithAllFilterProperties_CompilesSuccessfully()
        {
            // Arrange
            var rules = new[]
            {
                new FilterRule(FilterProperty.Name, FilterOperation.Contains, ["test"]),
                new FilterRule(FilterProperty.Path, FilterOperation.Contains, ["test"]),
                new FilterRule(FilterProperty.Region, FilterOperation.In, ["us"]),
                new FilterRule(FilterProperty.Lang, FilterOperation.In, ["en"]),
                new FilterRule(FilterProperty.Genre, FilterOperation.Equals, ["RPG"]),
                new FilterRule(FilterProperty.Id, FilterOperation.Equals, ["1"]),
                new FilterRule(FilterProperty.Source, FilterOperation.Contains, ["Screen"]),
                new FilterRule(FilterProperty.Developer, FilterOperation.Contains, ["Dev"]),
                new FilterRule(FilterProperty.Publisher, FilterOperation.Contains, ["Pub"]),
                new FilterRule(FilterProperty.Image, FilterOperation.HasValue, []),
                new FilterRule(FilterProperty.Description, FilterOperation.Contains, ["test"]),
                new FilterRule(FilterProperty.Rating, FilterOperation.Contains, ["0.8"]),
                new FilterRule(FilterProperty.Hash, FilterOperation.Equals, ["ABC"]),
                new FilterRule(FilterProperty.Crc32, FilterOperation.Equals, ["123"])
            };

            var config = new FilterConfig(
                ExcludeRules: ImmutableArray<FilterRule>.Empty,
                IncludeRules: [.. rules]);

            var matchingGame = CreateTestGame(
                name: "test game",
                path: "./test.rom",
                region: "us",
                lang: "en",
                genre: "RPG",
                id: "1",
                source: "ScreenScraper",
                developer: "DevStudio",
                publisher: "PubCo",
                image: "./image.png",
                description: "test description",
                rating: "0.8",
                hash: "ABC",
                crc32: "123");

            // Act
            var filter = FilterRuleCompiler.Compile(config);
            var result = filter(matchingGame);

            // Assert
            result.ShouldBeTrue();
        }
    }
}
