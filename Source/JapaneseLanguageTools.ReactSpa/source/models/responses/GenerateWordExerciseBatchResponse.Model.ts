import GenerateWordExerciseBatchRequestModel from "@/models/requests/GenerateWordExerciseBatchRequest.Model";

export default interface GenerateWordExerciseBatchResponseModel {
  wordExerciseBatchId: string;

  request: GenerateWordExerciseBatchRequestModel;
}
