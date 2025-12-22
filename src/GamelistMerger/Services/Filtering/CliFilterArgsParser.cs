using System.Collections.Immutable;

namespace GamelistMerger.Services.Filtering;

public static class CliFilterArgsParser
{
    public static FilterConfig GetFilterConfig(bool excludeBios, string[] excludeNameContains, string[] excludeRegions, string[] includeLanguages)
    {
        var excludeRules = ImmutableArray.CreateBuilder<FilterRule>();
        var includeRules = ImmutableArray.CreateBuilder<FilterRule>();

        if (excludeBios)
        {
            excludeRules.Add(new FilterRule(
                FilterProperty.Name,
                FilterOperation.Contains,
                ["[BIOS]", "(BIOS)"]));
        }

        if (excludeNameContains is { Length: > 0 })
        {
            excludeRules.Add(new FilterRule(
                FilterProperty.Name,
                FilterOperation.Contains,
                excludeNameContains));
        }

        if (excludeRegions is { Length: > 0 })
        {
            excludeRules.Add(new FilterRule(
                FilterProperty.Region,
                FilterOperation.In,
                excludeRegions));
        }

        if (includeLanguages is { Length: > 0 })
        {
            includeRules.Add(new FilterRule(
                FilterProperty.Lang,
                FilterOperation.In,
                includeLanguages));
        }

        return new FilterConfig(excludeRules.ToImmutable(), includeRules.ToImmutable());
    }
}