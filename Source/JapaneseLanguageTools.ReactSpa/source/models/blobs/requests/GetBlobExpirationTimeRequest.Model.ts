import BlobReferenceModel from "@/models/blobs/BlobReference.Model";

export default interface GetBlobExpirationTimeRequestModel {
  blobReference: BlobReferenceModel;
}
