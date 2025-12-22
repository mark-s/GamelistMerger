using System.Diagnostics;

namespace GamelistMerger.Models;

[DebuggerDisplay("[{Id}] [{Name}] [{Path}]")]
public sealed record Game(
    string? Id,
    string? Source,
    string? Name,
    string? Desc,
    string? Image,
    string? Thumbnail,
    string? Path,
    string? Rating,
    string? ReleaseDate,
    string? Developer,
    string? Publisher,
    string? Genre,
    string? Players,
    string? PlayCount,
    string? LastPlayed,
    string? Favorite,
    string? Hash,
    string? Crc32,
    string? Lang,
    string? Region,
    string? GenreId);