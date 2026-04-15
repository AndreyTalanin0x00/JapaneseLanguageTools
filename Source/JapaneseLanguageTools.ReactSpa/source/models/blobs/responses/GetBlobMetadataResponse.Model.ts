import BlobMetadataModel from "@/models/blobs/BlobMetadata.Model";
import GetBlobMetadataRequestModel from "@/models/blobs/requests/GetBlobMetadataRequest.Model";

export default interface GetBlobMetadataResponseModel {
  blobMetadata: BlobMetadataModel;

  request: GetBlobMetadataRequestModel;
}
