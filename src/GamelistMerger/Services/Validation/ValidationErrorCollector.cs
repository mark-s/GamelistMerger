namespace GamelistMerger.Services.Validation;

public sealed class ValidationErrorCollector
{
    private readonly List<ValidationError> _errors = [];

    public IReadOnlyList<ValidationError> Errors => _errors;
    public bool HasErrors => _errors.Any();

    public void Add(string message, int? lineNumber = null, string? element = null)
    {
        _errors.Add(new ValidationError(message, lineNumber, element));
    }
}