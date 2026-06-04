import axios from "axios";

export async function shutdown(): Promise<void> {
  await axios.post(`/api/ApplicationLifetime/Shutdown`);
}
