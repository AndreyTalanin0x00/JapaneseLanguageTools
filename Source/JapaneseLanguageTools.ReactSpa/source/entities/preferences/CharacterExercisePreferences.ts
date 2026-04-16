import CharacterExerciseProfile from "@/entities/preferences/CharacterExerciseProfile";
import CharacterExerciseAlphabetMode from "@/enumerations/types/CharacterExerciseAlphabetMode";
import CharacterExerciseMode from "@/enumerations/types/CharacterExerciseMode";

export default interface CharacterExercisePreferences {
  defaultExerciseSize?: number;
  defaultExerciseMode?: CharacterExerciseMode;
  defaultExerciseAlphabetMode?: CharacterExerciseAlphabetMode;
  exerciseProfiles?: CharacterExerciseProfile[];
  defaultExerciseProfileId?: string;
  defaultCharacterGroupIds?: number[];
  navigateOnCompletion?: boolean;
  navigateOnCompletionDelayMilliseconds?: number;
  navigateOnFailure?: boolean;
  navigateOnFailureDelayMilliseconds?: number;
}

export interface DefaultCharacterExercisePreferences extends CharacterExercisePreferences {
  defaultExerciseSize: number;
  defaultExerciseMode: CharacterExerciseMode;
  defaultExerciseAlphabetMode: CharacterExerciseAlphabetMode;
  exerciseProfiles: CharacterExerciseProfile[];
  defaultExerciseProfileId?: string;
  defaultCharacterGroupIds: number[];
  navigateOnCompletion: boolean;
  navigateOnCompletionDelayMilliseconds: number;
  navigateOnFailure: boolean;
  navigateOnFailureDelayMilliseconds: number;
}
