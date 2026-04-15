import CharacterExerciseModel from "@/models/CharacterExercise.Model";
import GenerateCharacterExerciseBatchResponseModel from "@/models/responses/GenerateCharacterExerciseBatchResponse.Model";

export default interface SubmitCompletedCharacterExerciseDetailsRequestModel {
  completedItems: CharacterExerciseModel[];

  originalResponse: GenerateCharacterExerciseBatchResponseModel;
}
