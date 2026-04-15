import ExportTagsRequestModel from "@/models/requests/x-export/ExportTagsRequest.Model";
import ExportResponseModel from "@/models/responses/base/ExportResponse.Model";

export default interface ExportTagsResponseModel extends ExportResponseModel {
  request: ExportTagsRequestModel;
}
