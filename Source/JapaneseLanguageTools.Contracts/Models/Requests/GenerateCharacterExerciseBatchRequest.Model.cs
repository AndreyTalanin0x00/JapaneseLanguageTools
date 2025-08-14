using System.Collections.Generic;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Contracts.Models.Requests;

public class GenerateCharacterExerciseBatchRequestModel
{
    public int Size { get; set; }

    public required CharacterExerciseProfileModel? UseCharacterExerciseProfile { get; set; }

    public IList<int> UseCharacterGroupIds { get; set; } = [];

    public CharacterTypes UseCharacterTypes { get; set; }
}
