import ImportResponseMessageType from "@/models/responses/base/ImportResponseMessageType";

export default interface ImportResponseMessageModel {
  type: ImportResponseMessageType;
  text: string;
}
