import ImportTagsRequestModel from "@/models/requests/x-import/ImportTagsRequest.Model";
import ImportResponseModel from "@/models/responses/base/ImportResponse.Model";

export default interface ImportTagsResponseModel extends ImportResponseModel {
  request: ImportTagsRequestModel;
}
