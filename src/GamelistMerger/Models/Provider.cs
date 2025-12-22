namespace GamelistMerger.Models;

public sealed record Provider(
    string? System,
    string? Software,
    string? Database,
    string? Web);