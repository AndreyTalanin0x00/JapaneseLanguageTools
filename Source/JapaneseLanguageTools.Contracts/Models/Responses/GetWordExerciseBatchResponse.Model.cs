using JapaneseLanguageTools.Contracts.Models.Requests;

namespace JapaneseLanguageTools.Contracts.Models.Responses;

public class GetWordExerciseBatchResponseModel
{
    public required WordExerciseBatchModel WordExerciseBatch { get; set; }

    public required GetWordExerciseBatchRequestModel Request { get; set; }
}
