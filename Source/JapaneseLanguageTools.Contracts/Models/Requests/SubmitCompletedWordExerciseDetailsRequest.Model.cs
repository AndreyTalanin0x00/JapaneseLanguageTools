using System.Collections.Generic;

using JapaneseLanguageTools.Contracts.Models.Responses;

namespace JapaneseLanguageTools.Contracts.Models.Requests;

public class SubmitCompletedWordExerciseDetailsRequestModel
{
    public IList<WordExerciseModel> CompletedItems { get; set; } = [];

    public required GenerateWordExerciseBatchResponseModel OriginalResponse { get; set; }
}
