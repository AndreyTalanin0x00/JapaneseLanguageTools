import TagDistributionPreferencesModel from "@/models/TagDistributionPreferences.Model";

export default interface WordExerciseProfileModel {
  repeatedChallengeCountProgression: number[];
  tagDistributionPreferences?: TagDistributionPreferencesModel;
}
