import { message as antdMessage } from "antd";

export default function logApiError(message: string): void {
  antdMessage.error(message);
}
