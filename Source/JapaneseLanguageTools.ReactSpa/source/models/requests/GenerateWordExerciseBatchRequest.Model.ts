import WordExerciseProfileModel from "@/models/WordExerciseProfile.Model";

export default interface GenerateWordExerciseBatchRequestModel {
  size: number;
  useWordExerciseProfile?: WordExerciseProfileModel;
  useWordGroupIds: number[];
}
