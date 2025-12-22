using System.Text;

namespace GamelistMerger.Tests.TestHelpers;

public sealed class GamelistXmlBuilder
{
    private readonly List<string> _elements = [];
    private bool _includeXmlDeclaration = true;
    private string? _providerContent;

    public static GamelistXmlBuilder Create() => new();

    public GamelistXmlBuilder WithoutXmlDeclaration()
    {
        _includeXmlDeclaration = false;
        return this;
    }

    public GamelistXmlBuilder WithProvider(string system, string software, string database, string web)
    {
        _providerContent = $$"""
            	<provider>
            		<System>{{system}}</System>
            		<software>{{software}}</software>
            		<database>{{database}}</database>
            		<web>{{web}}</web>
            	</provider>
            """;
        return this;
    }

    public GamelistXmlBuilder WithGame(Action<GameElementBuilder> configure)
    {
        var builder = new GameElementBuilder();
        configure(builder);
        _elements.Add(builder.Build());
        return this;
    }

    public GamelistXmlBuilder WithFolder(Action<FolderElementBuilder> configure)
    {
        var builder = new FolderElementBuilder();
        configure(builder);
        _elements.Add(builder.Build());
        return this;
    }

    public GamelistXmlBuilder WithUnknownElement(string elementName, string? content = null)
    {
        var contentXml = string.IsNullOrEmpty(content) ? "" : $"\n\t\t<content>{content}</content>\n\t";
        _elements.Add($"\t<{elementName}>{contentXml}</{elementName}>");
        return this;
    }

    public string Build()
    {
        var sb = new StringBuilder();

        if (_includeXmlDeclaration)
        {
            sb.AppendLine("<?xml version=\"1.0\"?>");
        }

        sb.AppendLine("<gameList>");

        if (_providerContent != null)
        {
            sb.AppendLine(_providerContent);
        }

        foreach (var element in _elements)
        {
            sb.AppendLine(element);
        }

        sb.Append("</gameList>");

        return sb.ToString();
    }

    public sealed class GameElementBuilder
    {
        private string? _id;
        private string? _source;
        private string? _path;
        private string? _name;
        private string? _desc;
        private string? _image;
        private string? _thumbnail;
        private string? _rating;
        private string? _releasedate;
        private string? _developer;
        private string? _publisher;
        private string? _genre;
        private string? _players;
        private string? _hash;
        private string? _crc32;
        private string? _lang;
        private string? _region;
        private string? _genreid;
        private string? _playcount;
        private string? _lastplayed;

        public GameElementBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public GameElementBuilder WithSource(string source)
        {
            _source = source;
            return this;
        }

        public GameElementBuilder WithPath(string path)
        {
            _path = path;
            return this;
        }

        public GameElementBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public GameElementBuilder WithDescription(string desc)
        {
            _desc = desc;
            return this;
        }

        public GameElementBuilder WithImage(string image)
        {
            _image = image;
            return this;
        }

        public GameElementBuilder WithThumbnail(string thumbnail)
        {
            _thumbnail = thumbnail;
            return this;
        }

        public GameElementBuilder WithRating(double rating)
        {
            _rating = rating.ToString("0.00");
            return this;
        }

        public GameElementBuilder WithReleaseDate(string releasedate)
        {
            _releasedate = releasedate;
            return this;
        }

        public GameElementBuilder WithDeveloper(string developer)
        {
            _developer = developer;
            return this;
        }

        public GameElementBuilder WithPublisher(string publisher)
        {
            _publisher = publisher;
            return this;
        }

        public GameElementBuilder WithGenre(string genre)
        {
            _genre = genre;
            return this;
        }

        public GameElementBuilder WithPlayers(int players)
        {
            _players = players.ToString();
            return this;
        }

        public GameElementBuilder WithHash(string hash)
        {
            _hash = hash;
            return this;
        }

        public GameElementBuilder WithCrc32(string crc32)
        {
            _crc32 = crc32;
            return this;
        }

        public GameElementBuilder WithLanguage(string lang)
        {
            _lang = lang;
            return this;
        }

        public GameElementBuilder WithRegion(string region)
        {
            _region = region;
            return this;
        }

        public GameElementBuilder WithGenreId(string genreid)
        {
            _genreid = genreid;
            return this;
        }

        public GameElementBuilder WithPlayCount(int playcount)
        {
            _playcount = playcount.ToString();
            return this;
        }

        public GameElementBuilder WithLastPlayed(string lastplayed)
        {
            _lastplayed = lastplayed;
            return this;
        }

        public string Build()
        {
            var sb = new StringBuilder();
            var attributes = new List<string>();

            if (_id != null)
                attributes.Add($"id=\"{_id}\"");
            if (_source != null)
                attributes.Add($"source=\"{_source}\"");

            var attributeString = attributes.Count > 0 ? " " + string.Join(" ", attributes) : "";

            sb.AppendLine($"\t<game{attributeString}>");
            if (_path != null)
                sb.AppendLine($"\t\t<path>{_path}</path>");
            if (_name != null)
                sb.AppendLine($"\t\t<name>{_name}</name>");
            if (_desc != null)
                sb.AppendLine($"\t\t<desc>{_desc}</desc>");
            if (_image != null)
                sb.AppendLine($"\t\t<image>{_image}</image>");
            if (_thumbnail != null)
                sb.AppendLine($"\t\t<thumbnail>{_thumbnail}</thumbnail>");
            if (_rating != null)
                sb.AppendLine($"\t\t<rating>{_rating}</rating>");
            if (_releasedate != null)
                sb.AppendLine($"\t\t<releasedate>{_releasedate}</releasedate>");
            if (_developer != null)
                sb.AppendLine($"\t\t<developer>{_developer}</developer>");
            if (_publisher != null)
                sb.AppendLine($"\t\t<publisher>{_publisher}</publisher>");
            if (_genre != null)
                sb.AppendLine($"\t\t<genre>{_genre}</genre>");
            if (_players != null)
                sb.AppendLine($"\t\t<players>{_players}</players>");
            if (_hash != null)
                sb.AppendLine($"\t\t<hash>{_hash}</hash>");
            if (_crc32 != null)
                sb.AppendLine($"\t\t<crc32>{_crc32}</crc32>");
            if (_lang != null)
                sb.AppendLine($"\t\t<lang>{_lang}</lang>");
            if (_region != null)
                sb.AppendLine($"\t\t<region>{_region}</region>");
            if (_genreid != null)
                sb.AppendLine($"\t\t<genreid>{_genreid}</genreid>");
            if (_playcount != null)
                sb.AppendLine($"\t\t<playcount>{_playcount}</playcount>");
            if (_lastplayed != null)
                sb.AppendLine($"\t\t<lastplayed>{_lastplayed}</lastplayed>");
            sb.Append("\t</game>");

            return sb.ToString();
        }
    }

    public sealed class FolderElementBuilder
    {
        private string? _id;
        private string? _source;
        private string? _path;
        private string? _name;
        private string? _desc;
        private string? _image;
        private string? _thumbnail;

        public FolderElementBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public FolderElementBuilder WithSource(string source)
        {
            _source = source;
            return this;
        }

        public FolderElementBuilder WithPath(string path)
        {
            _path = path;
            return this;
        }

        public FolderElementBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public FolderElementBuilder WithDescription(string desc)
        {
            _desc = desc;
            return this;
        }

        public FolderElementBuilder WithImage(string image)
        {
            _image = image;
            return this;
        }

        public FolderElementBuilder WithThumbnail(string thumbnail)
        {
            _thumbnail = thumbnail;
            return this;
        }

        public string Build()
        {
            var sb = new StringBuilder();
            var attributes = new List<string>();

            if (_id != null)
                attributes.Add($"id=\"{_id}\"");
            if (_source != null)
                attributes.Add($"source=\"{_source}\"");

            var attributeString = attributes.Count > 0 ? " " + string.Join(" ", attributes) : "";

            sb.AppendLine($"\t<folder{attributeString}>");
            if (_path != null)
                sb.AppendLine($"\t\t<path>{_path}</path>");
            if (_name != null)
                sb.AppendLine($"\t\t<name>{_name}</name>");
            if (_desc != null)
                sb.AppendLine($"\t\t<desc>{_desc}</desc>");
            if (_image != null)
                sb.AppendLine($"\t\t<image>{_image}</image>");
            if (_thumbnail != null)
                sb.AppendLine($"\t\t<thumbnail>{_thumbnail}</thumbnail>");
            sb.Append("\t</folder>");

            return sb.ToString();
        }
    }
}
