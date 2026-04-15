import BlobReferenceModel from "@/models/blobs/BlobReference.Model";
import ExportResponseMessageModel from "@/models/responses/base/ExportResponseMessage.Model";
import ExportStatus from "@/models/responses/base/ExportStatus";

export default interface ExportResponseModel {
  status: ExportStatus;
  messages: ExportResponseMessageModel[];
  blobReference: BlobReferenceModel;
}
