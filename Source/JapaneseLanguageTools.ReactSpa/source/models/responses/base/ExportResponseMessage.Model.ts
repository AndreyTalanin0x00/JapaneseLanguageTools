import ExportResponseMessageType from "@/models/responses/base/ExportResponseMessageType";

export default interface ExportResponseMessageModel {
  type: ExportResponseMessageType;
  text: string;
}
