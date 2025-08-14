using System.Collections.Generic;

using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Models.Requests;

public class SubmitCompletedCharacterExerciseDetailsRequestModel
{
    public IList<CharacterExerciseModel> CompletedItems { get; set; } = [];

    public required GenerateCharacterExerciseBatchResponseModel OriginalResponse { get; set; }
}
