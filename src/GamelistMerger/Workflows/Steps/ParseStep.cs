using CSharpFunctionalExtensions;
using GamelistMerger.Services.Parsers;
using GamelistMerger.Services.Validation;
using GamelistMerger.Workflows.Models;

namespace GamelistMerger.Workflows.Steps;

public static class ParseStep
{
    public static Result<ParsedGameListPair, IReadOnlyList<ValidationError>> Execute(LoadedGameListPair gamelists)
    {
        var parsedA = GamelistParser.Parse(gamelists.A.Xml);
        var parsedB = GamelistParser.Parse(gamelists.B.Xml);

        return parsedA.Bind(a => parsedB.Map(b => new ParsedGameListPair(gamelists.A.Validation, gamelists.B.Validation, a, b)));
    }
}
