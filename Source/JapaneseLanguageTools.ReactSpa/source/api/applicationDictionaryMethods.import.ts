import axios, { AxiosRequestConfig } from "axios";

import * as FileConstants from "@/constants/FileConstants";
import UploadBlobResponseModel from "@/models/blobs/responses/UploadBlobResponse.Model";
import ImportApplicationDictionaryRequestModel from "@/models/requests/x-import/ImportApplicationDictionaryRequest.Model";
import ImportApplicationDictionaryResponseModel from "@/models/responses/x-import/ImportApplicationDictionaryResponse.Model";

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

  const { data: responseModel } = await axios.post<UploadBlobResponseModel>(`/api/ApplicationDictionaryImport/UploadImportBlob`, formData, requestConfig);
  return responseModel;
}

export async function importApplicationDictionary(requestModel: ImportApplicationDictionaryRequestModel): Promise<ImportApplicationDictionaryResponseModel> {
  const { data: responseModel } = await axios.put<ImportApplicationDictionaryResponseModel>(`/api/ApplicationDictionaryImport/ImportApplicationDictionary`, requestModel);
  return responseModel;
}
