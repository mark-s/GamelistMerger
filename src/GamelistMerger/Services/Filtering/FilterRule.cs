namespace GamelistMerger.Services.Filtering;

public enum FilterProperty
{
    Name,
    Path,
    Region,
    Lang,
    Genre,
    Id,
    Source,
    Developer,
    Publisher,
    Image,
    Description,
    Rating,
    Hash,
    Crc32
}

public enum FilterOperation
{
    StartsWith,
    EndsWith,
    Contains,
    Equals,
    In,
    HasValue
}

public sealed record FilterRule(FilterProperty Property, FilterOperation Operation, string[] Values);