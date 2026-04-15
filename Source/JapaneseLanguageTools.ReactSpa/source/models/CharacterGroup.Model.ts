import CharacterModel from "@/models/Character.Model";

export default interface CharacterGroupModel {
  id: number;
  caption: string;
  comment?: string;
  enabled: boolean;
  alwaysUse: boolean;
  hidden: boolean;
  createdOn?: string;
  updatedOn?: string;

  characters: CharacterModel[];
}
