import CharacterTypes from "@/enumerations/CharacterTypes";
import CharacterExerciseProfileModel from "@/models/CharacterExerciseProfile.Model";

export default interface GenerateCharacterExerciseBatchRequestModel {
  size: number;
  useCharacterExerciseProfile?: CharacterExerciseProfileModel;
  useCharacterGroupIds: number[];
  useCharacterTypes: CharacterTypes;
}
