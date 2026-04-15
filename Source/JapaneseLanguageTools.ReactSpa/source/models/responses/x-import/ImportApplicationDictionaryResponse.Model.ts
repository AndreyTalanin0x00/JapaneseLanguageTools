import ImportApplicationDictionaryRequestModel from "@/models/requests/x-import/ImportApplicationDictionaryRequest.Model";
import ImportResponseModel from "@/models/responses/base/ImportResponse.Model";

export default interface ImportApplicationDictionaryResponseModel extends ImportResponseModel {
  request: ImportApplicationDictionaryRequestModel;
}
