using System.Collections.Generic;

namespace JapaneseLanguageTools.Contracts.Models.Requests;

public class GenerateWordExerciseBatchRequestModel
{
    public int Size { get; set; }

    public required WordExerciseProfileModel? UseWordExerciseProfile { get; set; }

    public IList<int> UseWordGroupIds { get; set; } = [];
}
