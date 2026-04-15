import CharacterTypes from "@/enumerations/CharacterTypes";
import CharacterGroupModel from "@/models/CharacterGroup.Model";
import TagModel from "@/models/Tag.Model";

export default interface CharacterModel {
  id: number;
  characterGroupId?: number;
  symbol: string;
  type: CharacterTypes;
  pronunciation?: string;
  syllable?: string;
  onyomi?: string;
  kunyomi?: string;
  meaning?: string;
  createdOn?: string;
  updatedOn?: string;

  characterTags: TagModel[];

  characterGroup?: CharacterGroupModel;
}
