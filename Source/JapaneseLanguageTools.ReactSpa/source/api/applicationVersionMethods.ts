import axios from "axios";

export async function getInformationalVersion(): Promise<string> {
  const { data: informationalVersion } = await axios.get<string>(`/api/ApplicationVersion/GetInformationalVersion`);
  return informationalVersion;
}

export async function getCommitDate(): Promise<string> {
  const { data: commitDate } = await axios.get<string>(`/api/ApplicationVersion/GetCommitDate`);
  return commitDate;
}
