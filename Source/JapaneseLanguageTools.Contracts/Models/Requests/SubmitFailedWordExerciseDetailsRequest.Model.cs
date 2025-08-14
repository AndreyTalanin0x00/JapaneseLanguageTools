using System.Collections.Generic;

using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Models.Requests;

public class SubmitFailedWordExerciseDetailsRequestModel
{
    public IList<WordExerciseModel> FailedItems { get; set; } = [];

    public required GenerateWordExerciseBatchResponseModel OriginalResponse { get; set; }
}
