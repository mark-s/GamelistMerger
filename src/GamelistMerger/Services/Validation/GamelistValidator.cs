using System.Xml;
using System.Xml.Linq;
using CSharpFunctionalExtensions;

namespace GamelistMerger.Services.Validation;

public static class GamelistValidator
{
    private const string VALID_ROOT = "gameList";
    private static readonly HashSet<string> _validRootChildren = ["game", "folder", "provider"];

    public static Result<ValidationSuccess, IReadOnlyList<ValidationError>> ValidateDocument(XDocument xDocument)
        => EnsureRootExists(xDocument)
            .Bind(ValidateRootElement)
            .Bind(ValidateGamelistContent);

    private static Result<XElement, IReadOnlyList<ValidationError>> EnsureRootExists(XDocument doc)
    {
        if (doc.Root is null)
        {
            var error = new ValidationError("XML document has no root element");
            return Result.Failure<XElement, IReadOnlyList<ValidationError>>([error]);
        }
        else
        {
            return Result.Success<XElement, IReadOnlyList<ValidationError>>(doc.Root);
        }
    }

    private static Result<XElement, IReadOnlyList<ValidationError>> ValidateRootElement(XElement root)
    {
        if (root.Name.LocalName != VALID_ROOT)
        {
            var error = new ValidationError($"Root element must be <{VALID_ROOT}>, found <{root.Name.LocalName}>", GetLineNumber(root));
            return Result.Failure<XElement, IReadOnlyList<ValidationError>>([error]);
        }
        else
        {
            return Result.Success<XElement, IReadOnlyList<ValidationError>>(root);
        }
    }

    private static Result<ValidationSuccess, IReadOnlyList<ValidationError>> ValidateGamelistContent(XElement gameList)
    {
        var errorCollector = new ValidationErrorCollector();
        var gameCount = 0;
        var folderCount = 0;

        foreach (var element in gameList.Elements())
        {
            var elementName = element.Name.LocalName;
            var lineNumber = GetLineNumber(element);

            if (_validRootChildren.Contains(elementName) == false)
            {
                errorCollector.Add($"Unknown element <{elementName}> under <gameList>", lineNumber, elementName);
                continue;
            }

            switch (elementName)
            {
                case "game":
                    ValidateGameElement(element, errorCollector);
                    gameCount++;
                    break;
                case "folder":
                    ValidateFolderElement(element, errorCollector);
                    folderCount++;
                    break;
                case "provider":
                    // ignore
                    break;
            }
        }

        if (errorCollector.HasErrors)
            return Result.Failure<ValidationSuccess, IReadOnlyList<ValidationError>>(errorCollector.Errors);
        else
            return Result.Success<ValidationSuccess, IReadOnlyList<ValidationError>>(new ValidationSuccess(gameCount, folderCount));
    }

    private static void ValidateGameElement(XElement game, ValidationErrorCollector collector)
    {
        if (game.Element("path") is null)
            collector.Add("<game> element missing required <path> child", GetLineNumber(game), "game");
    }

    private static void ValidateFolderElement(XElement folder, ValidationErrorCollector collector)
    {
        if (folder.Element("path") is null)
            collector.Add("<folder> element missing required <path> child", GetLineNumber(folder), "folder");
    }

    private static int? GetLineNumber(XElement element)
        => element is IXmlLineInfo lineInfo && lineInfo.HasLineInfo()
            ? lineInfo.LineNumber
            : null;
}
