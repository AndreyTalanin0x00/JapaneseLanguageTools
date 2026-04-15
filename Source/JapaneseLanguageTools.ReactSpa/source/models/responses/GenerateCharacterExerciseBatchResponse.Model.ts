import GenerateCharacterExerciseBatchRequestModel from "@/models/requests/GenerateCharacterExerciseBatchRequest.Model";

export default interface GenerateCharacterExerciseBatchResponseModel {
  characterExerciseBatchId: string;

  request: GenerateCharacterExerciseBatchRequestModel;
}
