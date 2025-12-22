namespace GamelistMerger.Services.Validation;

public sealed record ValidationError(string Message, int? LineNumber = null, string? Element = null, string? FileName = null)
{
    public override string ToString()
    {
        var fileInfo = FileName is not null ? $"[{FileName}] " : "";
        var lineInfo = LineNumber.HasValue ? $"Line {LineNumber}: " : "";
        return $"{fileInfo}{lineInfo}{Message}";
    }
}