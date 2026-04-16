import { DefaultApplicationPreferences } from "@/entities/preferences/ApplicationPreferences";
import { DefaultCharacterExercisePreferences } from "@/entities/preferences/CharacterExercisePreferences";
import { DefaultWordExercisePreferences } from "@/entities/preferences/WordExercisePreferences";

export const applicationPreferencesLocalStorageKey = "application-preferences";

export const characterExercisePreferencesDefaultValues: DefaultCharacterExercisePreferences = {
  defaultExerciseSize: 36,
  defaultExerciseMode: "character",
  defaultExerciseAlphabetMode: "kana",
  exerciseProfiles: [],
  defaultExerciseProfileId: undefined,
  defaultCharacterGroupIds: [],
  navigateOnCompletion: false,
  navigateOnCompletionDelayMilliseconds: 2000,
  navigateOnFailure: false,
  navigateOnFailureDelayMilliseconds: 5000,
};

export const wordExercisePreferencesDefaultValues: DefaultWordExercisePreferences = {
  defaultExerciseSize: 36,
  defaultExerciseMode: "characters-pronunciation",
  exerciseProfiles: [],
  defaultExerciseProfileId: undefined,
  defaultWordGroupIds: [],
  navigateOnCompletion: false,
  navigateOnCompletionDelayMilliseconds: 2000,
  navigateOnFailure: false,
  navigateOnFailureDelayMilliseconds: 5000,
};

export const applicationPreferencesDefaultValues: DefaultApplicationPreferences = {
  characterExercisePreferences: characterExercisePreferencesDefaultValues,
  wordExercisePreferences: wordExercisePreferencesDefaultValues,
};
