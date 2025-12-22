using GamelistMerger.Services.Filtering;
using GamelistMerger.Services.Validation;

namespace GamelistMerger.Workflows.Models;

public sealed record MergeWorkflowResult(ValidationSuccess ValidationA, ValidationSuccess ValidationB, FileInfo OutputFile, FilterStatistics FilterStats, TimeSpan Duration, bool Verbose);