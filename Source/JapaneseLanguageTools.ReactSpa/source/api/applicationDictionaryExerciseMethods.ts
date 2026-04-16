import axios from "axios";
import queryString from "query-string";

import GetCharacterExerciseBatchRequestModel from "@/models/requests/GetCharacterExerciseBatchRequest.Model";
import GetCharacterExerciseBatchResponseModel from "@/models/responses/GetCharacterExerciseBatchResponse.Model";
import GenerateCharacterExerciseBatchRequestModel from "@/models/requests/GenerateCharacterExerciseBatchRequest.Model";
import GenerateCharacterExerciseBatchResponseModel from "@/models/responses/GenerateCharacterExerciseBatchResponse.Model";
import SubmitCompletedCharacterExerciseDetailsRequestModel from "@/models/requests/SubmitCompletedCharacterExerciseDetailsRequest.Model";
import SubmitCompletedCharacterExerciseDetailsResponseModel from "@/models/responses/SubmitCompletedCharacterExerciseDetailsResponse.Model";
import SubmitFailedCharacterExerciseDetailsRequestModel from "@/models/requests/SubmitFailedCharacterExerciseDetailsRequest.Model";
import SubmitFailedCharacterExerciseDetailsResponseModel from "@/models/responses/SubmitFailedCharacterExerciseDetailsResponse.Model";
import GetWordExerciseBatchRequestModel from "@/models/requests/GetWordExerciseBatchRequest.Model";
import GetWordExerciseBatchResponseModel from "@/models/responses/GetWordExerciseBatchResponse.Model";
import GenerateWordExerciseBatchRequestModel from "@/models/requests/GenerateWordExerciseBatchRequest.Model";
import GenerateWordExerciseBatchResponseModel from "@/models/responses/GenerateWordExerciseBatchResponse.Model";
import SubmitCompletedWordExerciseDetailsRequestModel from "@/models/requests/SubmitCompletedWordExerciseDetailsRequest.Model";
import SubmitCompletedWordExerciseDetailsResponseModel from "@/models/responses/SubmitCompletedWordExerciseDetailsResponse.Model";
import SubmitFailedWordExerciseDetailsRequestModel from "@/models/requests/SubmitFailedWordExerciseDetailsRequest.Model";
import SubmitFailedWordExerciseDetailsResponseModel from "@/models/responses/SubmitFailedWordExerciseDetailsResponse.Model";

export async function getCharacterExerciseBatch(requestModel: GetCharacterExerciseBatchRequestModel): Promise<GetCharacterExerciseBatchResponseModel> {
  const queryStringValues = queryString.stringify(requestModel);
  const { data: responseModel } = await axios.get<GetCharacterExerciseBatchResponseModel>(`/api/ApplicationDictionaryExercise/GetCharacterExerciseBatch?${queryStringValues}`);
  return responseModel;
}

export async function generateCharacterExerciseBatch(requestModel: GenerateCharacterExerciseBatchRequestModel): Promise<GenerateCharacterExerciseBatchResponseModel> {
  const { data: responseModel } = await axios.post<GenerateCharacterExerciseBatchResponseModel>(`/api/ApplicationDictionaryExercise/GenerateCharacterExerciseBatch`, requestModel);
  return responseModel;
}

export async function submitCompletedCharacterExerciseDetails(requestModel: SubmitCompletedCharacterExerciseDetailsRequestModel): Promise<SubmitCompletedCharacterExerciseDetailsResponseModel> {
  const { data: responseModel } = await axios.post<SubmitCompletedCharacterExerciseDetailsResponseModel>(`/api/ApplicationDictionaryExercise/SubmitCompletedCharacterExerciseDetails`, requestModel);
  return responseModel;
}

export async function submitFailedCharacterExerciseDetails(requestModel: SubmitFailedCharacterExerciseDetailsRequestModel): Promise<SubmitFailedCharacterExerciseDetailsResponseModel> {
  const { data: responseModel } = await axios.post<SubmitFailedCharacterExerciseDetailsResponseModel>(`/api/ApplicationDictionaryExercise/SubmitFailedCharacterExerciseDetails`, requestModel);
  return responseModel;
}

export async function getWordExerciseBatch(requestModel: GetWordExerciseBatchRequestModel): Promise<GetWordExerciseBatchResponseModel> {
  const queryStringValues = queryString.stringify(requestModel);
  const { data: responseModel } = await axios.get<GetWordExerciseBatchResponseModel>(`/api/ApplicationDictionaryExercise/GetWordExerciseBatch?${queryStringValues}`);
  return responseModel;
}

export async function generateWordExerciseBatch(requestModel: GenerateWordExerciseBatchRequestModel): Promise<GenerateWordExerciseBatchResponseModel> {
  const { data: responseModel } = await axios.post<GenerateWordExerciseBatchResponseModel>(`/api/ApplicationDictionaryExercise/GenerateWordExerciseBatch`, requestModel);
  return responseModel;
}

export async function submitCompletedWordExerciseDetails(requestModel: SubmitCompletedWordExerciseDetailsRequestModel): Promise<SubmitCompletedWordExerciseDetailsResponseModel> {
  const { data: responseModel } = await axios.post<SubmitCompletedWordExerciseDetailsResponseModel>(`/api/ApplicationDictionaryExercise/SubmitCompletedWordExerciseDetails`, requestModel);
  return responseModel;
}

export async function submitFailedWordExerciseDetails(requestModel: SubmitFailedWordExerciseDetailsRequestModel): Promise<SubmitFailedWordExerciseDetailsResponseModel> {
  const { data: responseModel } = await axios.post<SubmitFailedWordExerciseDetailsResponseModel>(`/api/ApplicationDictionaryExercise/SubmitFailedWordExerciseDetails`, requestModel);
  return responseModel;
}
