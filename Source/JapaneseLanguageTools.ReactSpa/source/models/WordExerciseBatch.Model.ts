import WordExerciseModel from "@/models/WordExercise.Model";

export default interface WordExerciseBatchModel {
  id: string;
  items: WordExerciseModel[];
}
