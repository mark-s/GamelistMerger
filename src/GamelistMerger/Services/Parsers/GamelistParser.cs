using System.Xml.Linq;
using System.Xml.Serialization;
using CSharpFunctionalExtensions;
using GamelistMerger.DTOs;
using GamelistMerger.Services.Validation;

namespace GamelistMerger.Services.Parsers;

public static class GamelistParser
{
    public static Result<GameListDTO, IReadOnlyList<ValidationError>> Parse(XDocument document)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(GameListDTO));
            using var reader = document.CreateReader();
            var gameList = (GameListDTO?)serializer.Deserialize(reader);

            if (gameList is not null)
                return Result.Success<GameListDTO, IReadOnlyList<ValidationError>>(gameList);
            else
                return Result.Failure<GameListDTO, IReadOnlyList<ValidationError>>([new ValidationError("Failed to deserialize gamelist XML")]);

        }
        catch (InvalidOperationException ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;
            var error = new ValidationError($"Failed to parse gamelist: {message}");
            return Result.Failure<GameListDTO, IReadOnlyList<ValidationError>>([error]);
        }
    }
}
