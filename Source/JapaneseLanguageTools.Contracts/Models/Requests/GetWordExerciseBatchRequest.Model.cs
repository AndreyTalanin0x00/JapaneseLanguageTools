using System;

namespace JapaneseLanguageTools.Contracts.Models.Requests;

public class GetWordExerciseBatchRequestModel
{
    public required Guid WordExerciseBatchId { get; set; }
}
