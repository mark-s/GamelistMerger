using System.Xml.Serialization;
using CSharpFunctionalExtensions;
using GamelistMerger.DTOs;

namespace GamelistMerger.Services.IO;

public static class XmlWriter
{
    public static Result SaveAsXml(GameListDTO gameList, FileInfo saveTo)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(GameListDTO));
            using TextWriter writer = new StreamWriter(saveTo.FullName);
            serializer.Serialize(writer, gameList);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
