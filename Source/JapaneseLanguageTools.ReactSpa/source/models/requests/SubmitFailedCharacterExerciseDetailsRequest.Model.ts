import CharacterExerciseModel from "@/models/CharacterExercise.Model";
import GenerateCharacterExerciseBatchResponseModel from "@/models/responses/GenerateCharacterExerciseBatchResponse.Model";

export default interface SubmitFailedCharacterExerciseDetailsRequestModel {
  failedItems: CharacterExerciseModel[];

  originalResponse: GenerateCharacterExerciseBatchResponseModel;
}
