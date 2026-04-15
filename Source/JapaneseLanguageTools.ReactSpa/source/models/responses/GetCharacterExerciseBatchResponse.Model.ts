import CharacterExerciseBatchModel from "@/models/CharacterExerciseBatch.Model";
import GetCharacterExerciseBatchRequestModel from "@/models/requests/GetCharacterExerciseBatchRequest.Model";

export default interface GetCharacterExerciseBatchResponseModel {
  characterExerciseBatch: CharacterExerciseBatchModel;

  request: GetCharacterExerciseBatchRequestModel;
}
