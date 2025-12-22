using System.Collections.Immutable;

namespace GamelistMerger.Services.Filtering;

public sealed record FilterConfig(
    ImmutableArray<FilterRule> ExcludeRules,
    ImmutableArray<FilterRule> IncludeRules);