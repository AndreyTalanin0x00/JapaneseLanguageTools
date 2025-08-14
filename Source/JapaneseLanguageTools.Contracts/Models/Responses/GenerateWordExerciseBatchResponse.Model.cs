using System;

using JapaneseLanguageTools.Contracts.Models.Requests;

namespace JapaneseLanguageTools.Contracts.Models.Responses;

public class GenerateWordExerciseBatchResponseModel
{
    public required Guid WordExerciseBatchId { get; set; }

    public required GenerateWordExerciseBatchRequestModel Request { get; set; }
}
