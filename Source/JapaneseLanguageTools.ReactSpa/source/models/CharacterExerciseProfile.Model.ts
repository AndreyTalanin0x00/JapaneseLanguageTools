import TagDistributionPreferencesModel from "@/models/TagDistributionPreferences.Model";

export default interface CharacterExerciseProfileModel {
  repeatedChallengeCountProgression: number[];
  tagDistributionPreferences?: TagDistributionPreferencesModel;
}
