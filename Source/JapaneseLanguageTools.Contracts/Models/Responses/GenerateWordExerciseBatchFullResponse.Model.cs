namespace JapaneseLanguageTools.Contracts.Models.Responses;

public class GenerateWordExerciseBatchFullResponseModel : GenerateWordExerciseBatchResponseModel
{
    public required WordExerciseBatchModel WordExerciseBatch { get; set; }
}
