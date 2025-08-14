using JapaneseLanguageTools.Contracts.Models.Requests;

namespace JapaneseLanguageTools.Contracts.Models.Responses;

public class GetCharacterExerciseBatchResponseModel
{
    public required CharacterExerciseBatchModel CharacterExerciseBatch { get; set; }

    public required GetCharacterExerciseBatchRequestModel Request { get; set; }
}
