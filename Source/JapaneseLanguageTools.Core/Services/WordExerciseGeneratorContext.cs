using System.Collections.Generic;

using JapaneseLanguageTools.Contracts.Models;

namespace JapaneseLanguageTools.Core.Services;

public class WordExerciseGeneratorContext
{
    public required IReadOnlyList<WordModel> AllWordModels { get; init; }

    public required IReadOnlyList<WordModel> AvailableWordModels { get; init; }

    public required IReadOnlyList<WordGroupModel> AllWordGroupModels { get; init; }

    public required IReadOnlyList<WordGroupModel> AvailableWordGroupModels { get; init; }

    public required IReadOnlyDictionary<string, int> TagDistributionDictionary { get; init; }
}
