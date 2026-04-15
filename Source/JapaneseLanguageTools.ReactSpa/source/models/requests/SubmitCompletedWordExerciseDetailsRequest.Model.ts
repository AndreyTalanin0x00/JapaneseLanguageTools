import WordExerciseModel from "@/models/WordExercise.Model";
import GenerateWordExerciseBatchResponseModel from "@/models/responses/GenerateWordExerciseBatchResponse.Model";

export default interface SubmitCompletedWordExerciseDetailsRequestModel {
  completedItems: WordExerciseModel[];

  originalResponse: GenerateWordExerciseBatchResponseModel;
}
