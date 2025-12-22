using GamelistMerger.Models;

namespace GamelistMerger.Tests.TestHelpers;

public sealed class TestFolderBuilder
{
    private string? _id;
    private string? _source;
    private string? _name;
    private string? _desc;
    private string? _image;
    private string? _thumbnail;
    private string? _path;

    private TestFolderBuilder() { }

    public static TestFolderBuilder Create() => new();

    public TestFolderBuilder WithId(string? id)
    {
        _id = id;
        return this;
    }

    public TestFolderBuilder WithSource(string? source)
    {
        _source = source;
        return this;
    }

    public TestFolderBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }

    public TestFolderBuilder WithDescription(string? desc)
    {
        _desc = desc;
        return this;
    }

    public TestFolderBuilder WithImage(string? image)
    {
        _image = image;
        return this;
    }

    public TestFolderBuilder WithThumbnail(string? thumbnail)
    {
        _thumbnail = thumbnail;
        return this;
    }

    public TestFolderBuilder WithPath(string? path)
    {
        _path = path;
        return this;
    }

    public Folder Build() => new(
        Id: _id,
        Source: _source,
        Name: _name,
        Desc: _desc,
        Image: _image,
        Thumbnail: _thumbnail,
        Path: _path);
}
