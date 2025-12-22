using GamelistMerger.DTOs;

namespace GamelistMerger.Models;

public static class Mapper
{
    public static GameListPair MapToGameLists(GameListDTO gamelist1, GameListDTO gameList2)
        => new(MapToModel(gamelist1), MapToModel(gameList2));

    private static GameList MapToModel(GameListDTO dto)
        => new(
            provider: MapToModel(dto.Provider),
            folders: dto.Folders.Select(MapToModel).ToList(),
            games: dto.Games.Select(MapToModel).ToList());

    private static Provider? MapToModel(ProviderDTO? dto)
        => dto is not null
            ? new Provider(System: dto.System, Software: dto.Software, Database: dto.Database, Web: dto.Web)
            : null;

    private static Folder MapToModel(FolderDTO dto)
        => new(Id: dto.Id, Source: dto.Source, Name: dto.Name, Desc: dto.Desc, Image: dto.Image, Thumbnail: dto.Thumbnail, Path: dto.Path);

    private static Game MapToModel(GameDTO dto)
        => new(Id: dto.Id, Source: dto.Source, Name: dto.Name, Desc: dto.Desc, Image: dto.Image, Thumbnail: dto.Thumbnail, Path: dto.Path,
            Rating: dto.Rating, ReleaseDate: dto.ReleaseDate, Developer: dto.Developer, Publisher: dto.Publisher, Genre: dto.Genre,
            Players: dto.Players, PlayCount: dto.PlayCount, LastPlayed: dto.LastPlayed, Favorite: dto.Favorite, Hash: dto.Hash, Crc32: dto.Crc32,
            Lang: dto.Lang, Region: dto.Region, GenreId: dto.GenreId);

    public static GameListDTO MapToDto(GameList mergedGameList)
        => new()
        {
            Provider = MapToDto(mergedGameList.Provider),
            Folders = MapToDto(mergedGameList.Folders),
            Games = MapToDto(mergedGameList.Games)
        };

    private static ProviderDTO? MapToDto(Provider? provider)
        => provider is not null
            ? new ProviderDTO { System = provider.System, Software = provider.Software, Database = provider.Database, Web = provider.Web }
            : null;

    private static List<FolderDTO> MapToDto(IReadOnlyList<Folder> folders)
        => folders.Select(folder => new FolderDTO
        {
            Id = folder.Id,
            Source = folder.Source,
            Path = folder.Path ?? "",
            Name = folder.Name,
            Desc = folder.Desc,
            Image = folder.Image,
            Thumbnail = folder.Thumbnail
        }).ToList();

    private static List<GameDTO> MapToDto(IReadOnlyList<Game> games)
        => games.Select(game => new GameDTO
        {
            Id = game.Id,
            Source = game.Source,
            Path = game.Path ?? "",
            Name = game.Name,
            Desc = game.Desc,
            Image = game.Image,
            Thumbnail = game.Thumbnail,
            Rating = game.Rating,
            ReleaseDate = game.ReleaseDate,
            Developer = game.Developer,
            Publisher = game.Publisher,
            Genre = game.Genre,
            Players = game.Players,
            PlayCount = game.PlayCount,
            LastPlayed = game.LastPlayed,
            Favorite = game.Favorite,
            Hash = game.Hash,
            Crc32 = game.Crc32,
            Lang = game.Lang,
            Region = game.Region,
            GenreId = game.GenreId
        }).ToList();

}