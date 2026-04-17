import BlobMetadataModel from "@/models/blobs/BlobMetadata.Model";
import GetBlobExpirationTimeRequestModel from "@/models/blobs/requests/GetBlobExpirationTimeRequest.Model";

export default interface GetBlobExpirationTimeResponseModel {
  blobMetadata: BlobMetadataModel;

  request: GetBlobExpirationTimeRequestModel;
}
