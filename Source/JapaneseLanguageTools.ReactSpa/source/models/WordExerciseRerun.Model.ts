import WordExerciseModel from "@/models/WordExercise.Model";

export default interface WordExerciseRerunModel {
  id: number;
  wordExerciseId: number;
  requiredChallengeCount: number;
  continuousChallengeCount: number;
  totalChallengeCount: number;
  initiallyScheduledOn?: string;
  repeatedlyScheduledOn?: string;

  wordExercise?: WordExerciseModel;
}
