using System;

using JapaneseLanguageTools.Contracts.Models.Requests;

namespace JapaneseLanguageTools.Contracts.Models.Responses;

public class GenerateCharacterExerciseBatchResponseModel
{
    public required Guid CharacterExerciseBatchId { get; set; }

    public required GenerateCharacterExerciseBatchRequestModel Request { get; set; }
}
