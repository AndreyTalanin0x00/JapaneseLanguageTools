import WordExerciseBatchModel from "@/models/WordExerciseBatch.Model";
import GetWordExerciseBatchRequestModel from "@/models/requests/GetWordExerciseBatchRequest.Model";

export default interface GetWordExerciseBatchResponseModel {
  wordExerciseBatch: WordExerciseBatchModel;

  request: GetWordExerciseBatchRequestModel;
}
