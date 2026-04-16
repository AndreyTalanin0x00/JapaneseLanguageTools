import axios from "axios";
import queryString from "query-string";

import CharacterGroupModel from "@/models/CharacterGroup.Model";
import WordGroupModel from "@/models/WordGroup.Model";

export async function getCharacterGroups(characterGroupIds: number[]): Promise<CharacterGroupModel[]> {
  const queryStringObject = { characterGroupIds };
  const queryStringValues = queryString.stringify(queryStringObject);
  const { data: characterGroupModels } = await axios.get<CharacterGroupModel[]>(`/api/ApplicationDictionary/GetCharacterGroups?${queryStringValues}`);
  return characterGroupModels;
}

export async function getAllCharacterGroups(): Promise<CharacterGroupModel[]> {
  const { data: characterGroupModels } = await axios.get<CharacterGroupModel[]>(`/api/ApplicationDictionary/GetAllCharacterGroups`);
  return characterGroupModels;
}

export async function getWordGroups(wordGroupIds: number[]): Promise<WordGroupModel[]> {
  const queryStringObject = { wordGroupIds };
  const queryStringValues = queryString.stringify(queryStringObject);
  const { data: wordGroupModels } = await axios.get<WordGroupModel[]>(`/api/ApplicationDictionary/GetWordGroups?${queryStringValues}`);
  return wordGroupModels;
}

export async function getAllWordGroups(): Promise<WordGroupModel[]> {
  const { data: wordGroupModels } = await axios.get<WordGroupModel[]>(`/api/ApplicationDictionary/GetAllWordGroups`);
  return wordGroupModels;
}
