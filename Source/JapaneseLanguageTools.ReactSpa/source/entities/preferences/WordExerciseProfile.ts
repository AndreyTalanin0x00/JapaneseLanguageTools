import TagDistributionSettings from "@/entities/TagDistributionSettings";

export default interface WordExerciseProfile {
  id: string;
  name: string;
  repeatedChallengeCountProgression: number[];
  tagDistributionSettings?: TagDistributionSettings;
}
