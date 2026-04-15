import WordModel from "@/models/Word.Model";
import WordExerciseRerunModel from "@/models/WordExerciseRerun.Model";

export default interface WordExerciseModel {
  id: number;
  wordId: number;
  instanceData?: string;
  generatedOn?: string;

  wordExerciseReruns: WordExerciseRerunModel[];

  word?: WordModel;
}
