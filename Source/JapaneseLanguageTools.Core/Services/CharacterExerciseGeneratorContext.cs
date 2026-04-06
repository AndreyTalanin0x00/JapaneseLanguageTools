using System.Collections.Generic;

using JapaneseLanguageTools.Contracts.Models;

namespace JapaneseLanguageTools.Core.Services;

public class CharacterExerciseGeneratorContext
{
    public required IReadOnlyList<CharacterModel> AllCharacterModels { get; init; }

    public required IReadOnlyList<CharacterModel> AvailableCharacterModels { get; init; }

    public required IReadOnlyList<CharacterGroupModel> AllCharacterGroupModels { get; init; }

    public required IReadOnlyList<CharacterGroupModel> AvailableCharacterGroupModels { get; init; }

    public required IReadOnlyDictionary<string, int> TagDistributionDictionary { get; init; }
}
