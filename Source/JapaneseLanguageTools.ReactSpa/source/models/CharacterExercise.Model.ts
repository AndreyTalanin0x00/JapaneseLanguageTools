import CharacterModel from "@/models/Character.Model";
import CharacterExerciseRerunModel from "@/models/CharacterExerciseRerun.Model";

export default interface CharacterExerciseModel {
  id: number;
  characterId: number;
  instanceData?: string;
  generatedOn?: string;

  characterExerciseReruns: CharacterExerciseRerunModel[];

  character?: CharacterModel;
}
