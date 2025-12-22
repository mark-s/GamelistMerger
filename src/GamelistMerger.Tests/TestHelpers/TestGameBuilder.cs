using GamelistMerger.Models;

namespace GamelistMerger.Tests.TestHelpers;

public sealed class TestGameBuilder
{
    private string? _id;
    private string? _source;
    private string? _name;
    private string? _desc;
    private string? _image;
    private string? _thumbnail;
    private string? _path;
    private string? _rating;
    private string? _releaseDate;
    private string? _developer;
    private string? _publisher;
    private string? _genre;
    private string? _players;
    private string? _playCount;
    private string? _lastPlayed;
    private string? _favorite;
    private string? _hash;
    private string? _crc32;
    private string? _lang;
    private string? _region;
    private string? _genreId;

    private TestGameBuilder() { }

    public static TestGameBuilder Create() => new();

    public TestGameBuilder WithId(string? id)
    {
        _id = id;
        return this;
    }

    public TestGameBuilder WithSource(string? source)
    {
        _source = source;
        return this;
    }

    public TestGameBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }

    public TestGameBuilder WithDescription(string? desc)
    {
        _desc = desc;
        return this;
    }

    public TestGameBuilder WithImage(string? image)
    {
        _image = image;
        return this;
    }

    public TestGameBuilder WithThumbnail(string? thumbnail)
    {
        _thumbnail = thumbnail;
        return this;
    }

    public TestGameBuilder WithPath(string? path)
    {
        _path = path;
        return this;
    }

    public TestGameBuilder WithRating(string? rating)
    {
        _rating = rating;
        return this;
    }

    public TestGameBuilder WithReleaseDate(string? releaseDate)
    {
        _releaseDate = releaseDate;
        return this;
    }

    public TestGameBuilder WithDeveloper(string? developer)
    {
        _developer = developer;
        return this;
    }

    public TestGameBuilder WithPublisher(string? publisher)
    {
        _publisher = publisher;
        return this;
    }

    public TestGameBuilder WithGenre(string? genre)
    {
        _genre = genre;
        return this;
    }

    public TestGameBuilder WithPlayers(string? players)
    {
        _players = players;
        return this;
    }

    public TestGameBuilder WithPlayCount(string? playCount)
    {
        _playCount = playCount;
        return this;
    }

    public TestGameBuilder WithLastPlayed(string? lastPlayed)
    {
        _lastPlayed = lastPlayed;
        return this;
    }

    public TestGameBuilder WithFavorite(string? favorite)
    {
        _favorite = favorite;
        return this;
    }

    public TestGameBuilder WithHash(string? hash)
    {
        _hash = hash;
        return this;
    }

    public TestGameBuilder WithCrc32(string? crc32)
    {
        _crc32 = crc32;
        return this;
    }

    public TestGameBuilder WithLang(string? lang)
    {
        _lang = lang;
        return this;
    }

    public TestGameBuilder WithRegion(string? region)
    {
        _region = region;
        return this;
    }

    public TestGameBuilder WithGenreId(string? genreId)
    {
        _genreId = genreId;
        return this;
    }

    public Game Build() => new(
        Id: _id,
        Source: _source,
        Name: _name,
        Desc: _desc,
        Image: _image,
        Thumbnail: _thumbnail,
        Path: _path,
        Rating: _rating,
        ReleaseDate: _releaseDate,
        Developer: _developer,
        Publisher: _publisher,
        Genre: _genre,
        Players: _players,
        PlayCount: _playCount,
        LastPlayed: _lastPlayed,
        Favorite: _favorite,
        Hash: _hash,
        Crc32: _crc32,
        Lang: _lang,
        Region: _region,
        GenreId: _genreId);
}
