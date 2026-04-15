import WordModel from "@/models/Word.Model";

export default interface WordGroupModel {
  id: number;
  caption: string;
  comment?: string;
  enabled: boolean;
  alwaysUse: boolean;
  hidden: boolean;
  createdOn?: string;
  updatedOn?: string;

  words: WordModel[];
}
