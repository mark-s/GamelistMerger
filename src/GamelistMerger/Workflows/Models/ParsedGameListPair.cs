using GamelistMerger.DTOs;
using GamelistMerger.Services.Validation;

namespace GamelistMerger.Workflows.Models;

public sealed record ParsedGameListPair(ValidationSuccess ValidationA, ValidationSuccess ValidationB, GameListDTO ParsedA, GameListDTO ParsedB);