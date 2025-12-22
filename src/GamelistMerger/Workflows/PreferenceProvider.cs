using GamelistMerger.Models;

namespace GamelistMerger.Workflows;

public static class PreferenceProvider
{
    private static readonly string[] _compressedTypes = [".zip", ".7z"];

    public static Func<Game, Game, Game?> PreferCompressedFile(AppConfig config)
    {
        if (config.PreferArchives)
            return static (game1, game2) =>
            {
                if (IsFileTypeCompressed(GetFileType(game1.Path)))
                    return game1;
                if (IsFileTypeCompressed(GetFileType(game2.Path)))
                    return game2;
                return game1;
            };

        return static (_, __) => null;
    }

    private static string GetFileType(string? fullPath)
        => string.IsNullOrWhiteSpace(fullPath) == false
            ? Path.GetExtension(fullPath).ToLowerInvariant()
            : string.Empty;

    private static bool IsFileTypeCompressed(string fileType)
        => _compressedTypes.Contains(fileType);
}