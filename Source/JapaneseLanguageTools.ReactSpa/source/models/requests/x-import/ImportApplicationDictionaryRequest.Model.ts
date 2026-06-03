import BlobReferenceModel from "@/models/blobs/BlobReference.Model";
import ImportRequestModel from "@/models/requests/base/ImportRequest.Model";

export default interface ImportApplicationDictionaryRequestModel extends ImportRequestModel {
  blobReferences: BlobReferenceModel[];
}
