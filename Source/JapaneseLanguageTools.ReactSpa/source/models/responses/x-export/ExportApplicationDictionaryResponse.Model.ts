import ExportApplicationDictionaryRequestModel from "@/models/requests/x-export/ExportApplicationDictionaryRequest.Model";
import ExportResponseModel from "@/models/responses/base/ExportResponse.Model";

export default interface ExportApplicationDictionaryResponseModel extends ExportResponseModel {
  request: ExportApplicationDictionaryRequestModel;
}
