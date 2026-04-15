import WordExerciseBatchModel from "@/models/WordExerciseBatch.Model";
import GenerateWordExerciseBatchResponseModel from "@/models/responses/GenerateWordExerciseBatchResponse.Model";

export default interface GenerateWordExerciseBatchFullResponseModel extends GenerateWordExerciseBatchResponseModel {
  wordExerciseBatch: WordExerciseBatchModel;
}
