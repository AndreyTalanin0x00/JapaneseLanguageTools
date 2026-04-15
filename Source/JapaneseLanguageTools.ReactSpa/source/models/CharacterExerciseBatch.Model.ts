import CharacterExerciseModel from "@/models/CharacterExercise.Model";

export default interface CharacterExerciseBatchModel {
  id: string;
  items: CharacterExerciseModel[];
}
