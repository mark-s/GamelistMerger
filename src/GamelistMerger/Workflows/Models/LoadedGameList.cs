using System.Xml.Linq;
using GamelistMerger.Services.Validation;

namespace GamelistMerger.Workflows.Models;

public sealed record LoadedGameList(XDocument Xml, ValidationSuccess Validation);