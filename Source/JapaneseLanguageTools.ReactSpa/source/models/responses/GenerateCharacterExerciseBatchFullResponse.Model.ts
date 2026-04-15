import CharacterExerciseBatchModel from "@/models/CharacterExerciseBatch.Model";
import GenerateCharacterExerciseBatchResponseModel from "@/models/responses/GenerateCharacterExerciseBatchResponse.Model";

export default interface GenerateCharacterExerciseBatchFullResponseModel extends GenerateCharacterExerciseBatchResponseModel {
  characterExerciseBatch: CharacterExerciseBatchModel;
}
