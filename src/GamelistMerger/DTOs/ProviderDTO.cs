using System.Xml.Serialization;

namespace GamelistMerger.DTOs;

/// <summary>
/// Scraper metadata - optional element containing info about how the data was scraped.
/// </summary>
public sealed class ProviderDTO
{
    [XmlElement("System")]
    public string? System { get; set; }

    [XmlElement("software")]
    public string? Software { get; set; }

    [XmlElement("database")]
    public string? Database { get; set; }

    [XmlElement("web")]
    public string? Web { get; set; }
}