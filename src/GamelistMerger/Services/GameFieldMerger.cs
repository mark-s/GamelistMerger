using GamelistMerger.Models;

namespace GamelistMerger.Services;

public static class GameFieldMerger
{
    public static Game Merge(Game master, Game secondary, Func<Game, Game, Game?> fileTypePreferenceFunc)
    {
        // Choose the preferred game file based on the file type in the Path property
        var gamePreferred = fileTypePreferenceFunc(master, secondary);

        return new Game(
            Id: master.Id,
            Name: GetIfHasAValue(master.Name, secondary.Name),
            Path: gamePreferred?.Path ?? GetIfHasAValue(master.Path, secondary.Path),
            Image: gamePreferred?.Image ?? GetIfHasAValue(master.Image, secondary.Image),
            Thumbnail: gamePreferred?.Thumbnail ?? GetIfHasAValue(master.Thumbnail, secondary.Thumbnail),
            Source: GetIfHasAValue(master.Source, secondary.Source),
            Desc: GetIfHasAValue(master.Desc, secondary.Desc),
            Rating: GetIfHasAValue(master.Rating, secondary.Rating),
            ReleaseDate: GetIfHasAValue(master.ReleaseDate, secondary.ReleaseDate),
            Developer: GetIfHasAValue(master.Developer, secondary.Developer),
            Publisher: GetIfHasAValue(master.Publisher, secondary.Publisher),
            Genre: GetIfHasAValue(master.Genre, secondary.Genre),
            Players: GetIfHasAValue(master.Players, secondary.Players),
            PlayCount: GetIfHasAValue(master.PlayCount, secondary.PlayCount),
            LastPlayed: GetIfHasAValue(master.LastPlayed, secondary.LastPlayed),
            Favorite: GetIfHasAValue(master.Favorite, secondary.Favorite),
            Hash: GetIfHasAValue(master.Hash, secondary.Hash),
            Crc32: GetIfHasAValue(master.Crc32, secondary.Crc32),
            Lang: GetIfHasAValue(master.Lang, secondary.Lang),
            Region: GetIfHasAValue(master.Region, secondary.Region),
            GenreId: GetIfHasAValue(master.GenreId, secondary.GenreId)
        );
    }

    private static string? GetIfHasAValue(string? masterValue, string? secondaryValue)
        => string.IsNullOrEmpty(masterValue) ? secondaryValue : masterValue;
}
