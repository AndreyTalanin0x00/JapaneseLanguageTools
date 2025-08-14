using System.Collections.Generic;

using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Models.Requests;

public class SubmitFailedCharacterExerciseDetailsRequestModel
{
    public IList<CharacterExerciseModel> FailedItems { get; set; } = [];

    public required GenerateCharacterExerciseBatchResponseModel OriginalResponse { get; set; }
}
