import BlobReferenceModel from "@/models/blobs/BlobReference.Model";
import ImportRequestModel from "@/models/requests/base/ImportRequest.Model";

export default interface ImportTagsRequestModel extends ImportRequestModel {
  blobReferences: BlobReferenceModel[];
}
