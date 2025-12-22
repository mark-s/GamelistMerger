namespace GamelistMerger.Services.Validation;

public sealed record ValidationSuccess(int GameCount, int FolderCount)
{
    public override string ToString() => $"{GameCount} games, {FolderCount} folders";
}