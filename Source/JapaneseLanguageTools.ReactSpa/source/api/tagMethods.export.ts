import axios from "axios";
import queryString from "query-string";

import DownloadBlobRequestModel from "@/models/blobs/requests/DownloadBlobRequest.Model";
import GetBlobMetadataRequestModel from "@/models/blobs/requests/GetBlobMetadataRequest.Model";
import GetBlobExpirationTimeRequestModel from "@/models/blobs/requests/GetBlobExpirationTimeRequest.Model";
import GetBlobMetadataResponseModel from "@/models/blobs/responses/GetBlobMetadataResponse.Model";
import GetBlobExpirationTimeResponseModel from "@/models/blobs/responses/GetBlobExpirationTimeResponse.Model";
import ExportTagsRequestModel from "@/models/requests/x-export/ExportTagsRequest.Model";
import ExportTagsResponseModel from "@/models/responses/x-export/ExportTagsResponse.Model";

export async function exportTags(requestModel: ExportTagsRequestModel): Promise<ExportTagsResponseModel> {
  const { data: responseModel } = await axios.post<ExportTagsResponseModel>(`/api/TagExport/ExportTags`, requestModel);
  return responseModel;
}

export async function getExportBlobMetadata(requestModel: GetBlobMetadataRequestModel): Promise<GetBlobMetadataResponseModel> {
  const { blobReference } = requestModel;
  const queryStringValues = queryString.stringify(blobReference);
  const { data: responseModel } = await axios.get<GetBlobMetadataResponseModel>(`/api/TagExport/GetExportBlobMetadata?${queryStringValues}`);
  return responseModel;
}

export async function getExportBlobExpirationTime(requestModel: GetBlobExpirationTimeRequestModel): Promise<GetBlobExpirationTimeResponseModel> {
  const { blobReference } = requestModel;
  const queryStringValues = queryString.stringify(blobReference);
  const { data: responseModel } = await axios.get<GetBlobExpirationTimeResponseModel>(`/api/TagExport/GetExportBlobExpirationTime?${queryStringValues}`);
  return responseModel;
}

export async function downloadExportBlob(requestModel: DownloadBlobRequestModel): Promise<Blob> {
  const { blobReference } = requestModel;
  const queryStringValues = queryString.stringify(blobReference);
  const { data: blob } = await axios.get<Blob>(`/api/TagExport/DownloadExportBlob?${queryStringValues}`, { responseType: "blob" });
  return blob;
}
