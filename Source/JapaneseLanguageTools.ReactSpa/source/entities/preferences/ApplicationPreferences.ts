import CharacterExercisePreferences, { DefaultCharacterExercisePreferences } from "@/entities/preferences/CharacterExercisePreferences";
import WordExercisePreferences, { DefaultWordExercisePreferences } from "@/entities/preferences/WordExercisePreferences";

export default interface ApplicationPreferences {
  characterExercisePreferences?: CharacterExercisePreferences;
  wordExercisePreferences?: WordExercisePreferences;
}

export interface DefaultApplicationPreferences extends ApplicationPreferences {
  characterExercisePreferences: DefaultCharacterExercisePreferences;
  wordExercisePreferences: DefaultWordExercisePreferences;
}
