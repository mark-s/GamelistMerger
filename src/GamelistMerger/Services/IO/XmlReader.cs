using System.Xml;
using System.Xml.Linq;
using CSharpFunctionalExtensions;
using GamelistMerger.Services.Validation;

namespace GamelistMerger.Services.IO;

public static class XmlReader
{
    public static async Task<Result<XDocument, IReadOnlyList<ValidationError>>> TryLoadXmlFileAsync(FileInfo xmlFile)
    {
        if (xmlFile.Exists == false)
        {
            var error = new ValidationError($"File not found [{xmlFile.FullName}]");
            return Result.Failure<XDocument, IReadOnlyList<ValidationError>>([error]);
        }
        try
        {
            await using var stream = xmlFile.OpenRead();
            var xmlDoc = await XDocument.LoadAsync(stream, LoadOptions.SetLineInfo, CancellationToken.None);
            return Result.Success<XDocument, IReadOnlyList<ValidationError>>(xmlDoc);
        }
        catch (XmlException ex)
        {
            var error = new ValidationError($"XML is not well-formed [{ex.Message}]", ex.LineNumber);
            return Result.Failure<XDocument, IReadOnlyList<ValidationError>>([error]);
        }
    }
}
