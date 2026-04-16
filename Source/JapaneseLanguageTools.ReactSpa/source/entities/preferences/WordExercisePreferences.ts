import WordExerciseProfile from "@/entities/preferences/WordExerciseProfile";
import WordExerciseMode from "@/enumerations/types/WordExerciseMode";

export default interface WordExercisePreferences {
  defaultExerciseSize?: number;
  defaultExerciseMode?: WordExerciseMode;
  exerciseProfiles?: WordExerciseProfile[];
  defaultExerciseProfileId?: string;
  defaultWordGroupIds?: number[];
  navigateOnCompletion?: boolean;
  navigateOnCompletionDelayMilliseconds?: number;
  navigateOnFailure?: boolean;
  navigateOnFailureDelayMilliseconds?: number;
}

export interface DefaultWordExercisePreferences extends WordExercisePreferences {
  defaultExerciseSize: number;
  defaultExerciseMode: WordExerciseMode;
  exerciseProfiles: WordExerciseProfile[];
  defaultExerciseProfileId?: string;
  defaultWordGroupIds: number[];
  navigateOnCompletion: boolean;
  navigateOnCompletionDelayMilliseconds: number;
  navigateOnFailure: boolean;
  navigateOnFailureDelayMilliseconds: number;
}
