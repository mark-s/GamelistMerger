using System.Xml.Serialization;

namespace GamelistMerger.DTOs;

[XmlRoot("game")]
public class GameDTO
{
    [XmlAttribute("id")]
    public string? Id { get; set; }

    [XmlAttribute("source")]
    public string? Source { get; set; }

    [XmlElement("path")]
    public string Path { get; set; } = string.Empty;

    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("desc")]
    public string? Desc { get; set; }

    [XmlElement("image")]
    public string? Image { get; set; }

    [XmlElement("thumbnail")]
    public string? Thumbnail { get; set; }

    [XmlElement("rating")]
    public string? Rating { get; set; }

    [XmlElement("releasedate")]
    public string? ReleaseDate { get; set; }

    [XmlElement("developer")]
    public string? Developer { get; set; }

    [XmlElement("publisher")]
    public string? Publisher { get; set; }

    [XmlElement("genre")]
    public string? Genre { get; set; }

    [XmlElement("players")]
    public string? Players { get; set; }

    [XmlElement("playcount")]
    public string? PlayCount { get; set; }

    [XmlElement("lastplayed")]
    public string? LastPlayed { get; set; }

    [XmlElement("favorite")]
    public string? Favorite { get; set; }  // "true" or absent

    [XmlElement("hash")]
    public string? Hash { get; set; }

    [XmlElement("crc32")]
    public string? Crc32 { get; set; }

    [XmlElement("lang")]
    public string? Lang { get; set; }

    [XmlElement("region")]
    public string? Region { get; set; }

    [XmlElement("genreid")]
    public string? GenreId { get; set; }
}