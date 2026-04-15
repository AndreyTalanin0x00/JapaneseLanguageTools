import BlobReferenceModel from "@/models/blobs/BlobReference.Model";
import UploadBlobRequestModel from "@/models/blobs/requests/UploadBlobRequest.Model";

export default interface UploadBlobResponseModel {
  blobReference: BlobReferenceModel;
  blobUploadedOn: string;
  blobExpiresOn: string;

  request: UploadBlobRequestModel;
}
