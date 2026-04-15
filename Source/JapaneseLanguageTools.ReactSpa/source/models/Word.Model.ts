import CharacterTypes from "@/enumerations/CharacterTypes";
import WordGroupModel from "@/models/WordGroup.Model";
import TagModel from "@/models/Tag.Model";

export default interface WordModel {
  id: number;
  wordGroupId?: number;
  characters: string;
  characterTypes: CharacterTypes;
  pronunciation?: string;
  furigana?: string;
  okurigana?: string;
  meaning?: string;
  createdOn?: string;
  updatedOn?: string;

  wordTags: TagModel[];

  wordGroup?: WordGroupModel;
}
