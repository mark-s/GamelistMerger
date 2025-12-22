using CSharpFunctionalExtensions;
using GamelistMerger.Services.IO;
using GamelistMerger.Services.Validation;
using GamelistMerger.Workflows.Models;

namespace GamelistMerger.Workflows.Steps;

public static class LoadStep
{
    public static async Task<Result<LoadedGameListPair, IReadOnlyList<ValidationError>>> ExecuteAsync(AppConfig config)
    {
        var loadA = LoadAndValidateGameListAsync(config.GameListA);
        var loadB = LoadAndValidateGameListAsync(config.GameListB);

        await Task.WhenAll(loadA, loadB);

        return loadA.Result.Bind(a => loadB.Result.Map(b => new LoadedGameListPair(a, b)));
    }

    private static async Task<Result<LoadedGameList, IReadOnlyList<ValidationError>>> LoadAndValidateGameListAsync(FileInfo file)
        => await XmlReader.TryLoadXmlFileAsync(file)
            .Bind(xml => GamelistValidator.ValidateDocument(xml)
                .Map(validation => new LoadedGameList(xml, validation)));
}
