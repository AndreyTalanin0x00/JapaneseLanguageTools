import ImportResponseMessageModel from "@/models/responses/base/ImportResponseMessage.Model";
import ImportStatus from "@/models/responses/base/ImportStatus";

export default interface ImportResponseModel {
  status: ImportStatus;
  messages: ImportResponseMessageModel[];
}
