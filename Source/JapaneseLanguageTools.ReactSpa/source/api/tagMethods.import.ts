import axios, { AxiosRequestConfig } from "axios";

import * as FileConstants from "@/constants/FileConstants";
import UploadBlobResponseModel from "@/models/blobs/responses/UploadBlobResponse.Model";
import ImportTagsRequestModel from "@/models/requests/x-import/ImportTagsRequest.Model";
import ImportTagsResponseModel from "@/models/responses/x-import/ImportTagsResponse.Model";

export async function uploadImportBlob(file: File, onProgress?: (uploadProgressEvent: { percent?: number }) => void): Promise<UploadBlobResponseModel> {
  const formData = new FormData();

  formData.append("blob", file);

  const requestConfig: AxiosRequestConfig<FormData> = {
    headers: { "Content-Type": "multipart/form-data" },
    onUploadProgress: onProgress
      ? (progressEvent) => {
          onProgress({ percent: 100 * (progressEvent.loaded / (progressEvent.total ?? FileConstants.maxImportFileSizeBytes)) });
        }
      : undefined,
  };

  const { data: responseModel } = await axios.post<UploadBlobResponseModel>(`/api/TagImport/UploadImportBlob`, formData, requestConfig);
  return responseModel;
}

export async function importTags(requestModel: ImportTagsRequestModel): Promise<ImportTagsResponseModel> {
  const { data: responseModel } = await axios.put<ImportTagsResponseModel>(`/api/TagImport/ImportTags`, requestModel);
  return responseModel;
}
