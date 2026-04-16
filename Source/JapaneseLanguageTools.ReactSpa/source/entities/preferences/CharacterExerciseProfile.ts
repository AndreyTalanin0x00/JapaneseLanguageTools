import TagDistributionSettings from "@/entities/TagDistributionSettings";

export default interface CharacterExerciseProfile {
  id: string;
  name: string;
  repeatedChallengeCountProgression: number[];
  tagDistributionSettings?: TagDistributionSettings;
}
