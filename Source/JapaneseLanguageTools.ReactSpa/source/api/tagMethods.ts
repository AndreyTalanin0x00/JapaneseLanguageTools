import axios from "axios";

import TagModel from "@/models/Tag.Model";

export async function getAllTags(): Promise<TagModel[]> {
  const { data: tagModels } = await axios.get<TagModel[]>(`/api/Tag/GetAllTags`);
  return tagModels;
}
