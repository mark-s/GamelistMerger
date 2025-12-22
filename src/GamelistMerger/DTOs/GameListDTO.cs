using System.Xml.Serialization;

namespace GamelistMerger.DTOs;

/// <summary>
/// Root element containing all gamelist data.
/// </summary>
[XmlRoot("gameList")]
public class GameListDTO
{
    [XmlElement("provider")]
    public ProviderDTO? Provider { get; set; }

    [XmlElement("folder")]
    public List<FolderDTO> Folders { get; set; } = [];

    [XmlElement("game")]
    public List<GameDTO> Games { get; set; } = [];
}