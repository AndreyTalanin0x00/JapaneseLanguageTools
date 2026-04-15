import BlobMetadataModel from "@/models/blobs/BlobMetadata.Model";
import DownloadBlobRequestModel from "@/models/blobs/requests/DownloadBlobRequest.Model";

export default interface DownloadBlobResponseModel {
  blobMetadata: BlobMetadataModel;

  request: DownloadBlobRequestModel;
}
