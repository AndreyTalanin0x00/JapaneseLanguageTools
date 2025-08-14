namespace JapaneseLanguageTools.Contracts.Models.Responses;

public class GenerateCharacterExerciseBatchFullResponseModel : GenerateCharacterExerciseBatchResponseModel
{
    public required CharacterExerciseBatchModel CharacterExerciseBatch { get; set; }
}
