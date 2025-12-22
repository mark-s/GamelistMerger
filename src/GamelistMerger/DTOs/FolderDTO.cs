using System.Xml.Serialization;

namespace GamelistMerger.DTOs;

/// <summary>
/// Folder metadata for organizing games into directories.
/// </summary>
public sealed class FolderDTO
{
    // Attributes on the <folder> element
    [XmlAttribute("id")]
    public string? Id { get; set; }

    [XmlAttribute("source")]
    public string? Source { get; set; }

    // Elements
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
}