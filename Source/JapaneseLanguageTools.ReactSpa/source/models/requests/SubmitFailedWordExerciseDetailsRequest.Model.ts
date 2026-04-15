import WordExerciseModel from "@/models/WordExercise.Model";
import GenerateWordExerciseBatchResponseModel from "@/models/responses/GenerateWordExerciseBatchResponse.Model";

export default interface SubmitFailedWordExerciseDetailsRequestModel {
  failedItems: WordExerciseModel[];

  originalResponse: GenerateWordExerciseBatchResponseModel;
}
