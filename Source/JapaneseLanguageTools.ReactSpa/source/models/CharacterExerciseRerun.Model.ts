import CharacterExerciseModel from "@/models/CharacterExercise.Model";

export default interface CharacterExerciseRerunModel {
  id: number;
  characterExerciseId: number;
  requiredChallengeCount: number;
  continuousChallengeCount: number;
  totalChallengeCount: number;
  initiallyScheduledOn?: string;
  repeatedlyScheduledOn?: string;

  characterExercise?: CharacterExerciseModel;
}
