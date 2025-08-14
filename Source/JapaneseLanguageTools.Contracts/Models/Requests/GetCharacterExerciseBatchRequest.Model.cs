using System;

namespace JapaneseLanguageTools.Contracts.Models.Requests;

public class GetCharacterExerciseBatchRequestModel
{
    public required Guid CharacterExerciseBatchId { get; set; }
}
