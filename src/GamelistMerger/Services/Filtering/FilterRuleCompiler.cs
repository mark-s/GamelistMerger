using GamelistMerger.Models;

namespace GamelistMerger.Services.Filtering;

public static class FilterRuleCompiler
{
    private static readonly Dictionary<FilterOperation, Func<Func<Game, string?>, string[], Func<Game, bool>>> OperationHandlers = new()
    {
        [FilterOperation.StartsWith] = CreateStartsWithPredicate,
        [FilterOperation.EndsWith] = CreateEndsWithPredicate,
        [FilterOperation.Contains] = CreateContainsPredicate,
        [FilterOperation.Equals] = CreateEqualsPredicate,
        [FilterOperation.In] = CreateInPredicate,
        [FilterOperation.HasValue] = (getter, _) => CreateHasValuePredicate(getter)
    };

    private static readonly Dictionary<FilterProperty, Func<Game, string?>> PropertyGetters = new()
    {
        [FilterProperty.Name] = g => g.Name,
        [FilterProperty.Path] = g => g.Path,
        [FilterProperty.Region] = g => g.Region,
        [FilterProperty.Lang] = g => g.Lang,
        [FilterProperty.Genre] = g => g.Genre,
        [FilterProperty.Id] = g => g.Id,
        [FilterProperty.Source] = g => g.Source,
        [FilterProperty.Developer] = g => g.Developer,
        [FilterProperty.Publisher] = g => g.Publisher,
        [FilterProperty.Image] = g => g.Image,
        [FilterProperty.Description] = g => g.Desc,
        [FilterProperty.Rating] = g => g.Rating,
        [FilterProperty.Hash] = g => g.Hash,
        [FilterProperty.Crc32] = g => g.Crc32
    };

    public static Func<Game, bool> Compile(FilterConfig config)
    {
        var excludePredicates = config.ExcludeRules.Select(CompileRule).ToArray();
        var includePredicates = config.IncludeRules.Select(CompileRule).ToArray();

        return game =>
        {
            // Exclude if ANY exclude rule matches
            if (excludePredicates.Length > 0 && excludePredicates.Any(p => p(game)))
                return false;

            // Include only if ALL include rules match (or no include rules)
            if (includePredicates.Length > 0 && includePredicates.All(p => p(game)) == false)
                return false;

            return true;
        };
    }

    private static Func<Game, bool> CompileRule(FilterRule rule)
    {
        if (!PropertyGetters.TryGetValue(rule.Property, out var propertyGetter))
            throw new ArgumentException($"Unknown filter property: {rule.Property}");

        if (!OperationHandlers.TryGetValue(rule.Operation, out var handler))
            throw new ArgumentException($"Unknown filter operation: {rule.Operation}");

        return handler(propertyGetter, rule.Values);
    }

    private static Func<Game, bool> CreateStartsWithPredicate(Func<Game, string?> getter, string[] values)
        => game =>
        {
            var value = getter(game);
            return value is not null && values.Any(v => value.StartsWith(v, StringComparison.OrdinalIgnoreCase));
        };

    private static Func<Game, bool> CreateEndsWithPredicate(Func<Game, string?> getter, string[] values)
        => game =>
        {
            var value = getter(game);
            return value is not null && values.Any(v => value.EndsWith(v, StringComparison.OrdinalIgnoreCase));
        };

    private static Func<Game, bool> CreateContainsPredicate(Func<Game, string?> getter, string[] values)
        => game =>
        {
            var value = getter(game);
            return value is not null && values.Any(v => value.Contains(v, StringComparison.OrdinalIgnoreCase));
        };

    private static Func<Game, bool> CreateEqualsPredicate(Func<Game, string?> getter, string[] values)
        => game =>
        {
            var value = getter(game);
            return values.Any(v => string.Equals(value, v, StringComparison.OrdinalIgnoreCase));
        };

    private static Func<Game, bool> CreateInPredicate(Func<Game, string?> getter, string[] values)
        => CreateEqualsPredicate(getter, values);

    private static Func<Game, bool> CreateHasValuePredicate(Func<Game, string?> getter)
        => g => string.IsNullOrEmpty(getter(g)) == false;
}
