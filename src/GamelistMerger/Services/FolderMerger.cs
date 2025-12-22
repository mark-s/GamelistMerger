using GamelistMerger.Models;

namespace GamelistMerger.Services;

public static class FolderMerger
{
    /// <summary>
    /// Merges two Folder objects, using the master as the primary source.
    /// Properties that are null or empty in the master will be filled from the secondary.
    /// The Id property always comes from the master.
    /// </summary>
    public static Folder Merge(Folder master, Folder secondary)
    {
        return new Folder(
            Id: master.Id,
            Source: GetValue(master.Source, secondary.Source),
            Name: GetValue(master.Name, secondary.Name),
            Desc: GetValue(master.Desc, secondary.Desc),
            Image: GetValue(master.Image, secondary.Image),
            Thumbnail: GetValue(master.Thumbnail, secondary.Thumbnail),
            Path: GetValue(master.Path, secondary.Path)
        );
    }

    private static string? GetValue(string? masterValue, string? secondaryValue)
        => string.IsNullOrEmpty(masterValue) ? secondaryValue : masterValue;
}
