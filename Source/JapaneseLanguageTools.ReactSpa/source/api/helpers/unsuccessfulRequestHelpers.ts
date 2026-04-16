import { AxiosError, isAxiosError } from "axios";

import ApiErrorLogger from "@/api/helpers/ApiErrorLogger";
import ProblemDetails from "@/models/schema/ProblemDetails";

function onRequestServerError(apiErrorLogger: ApiErrorLogger, statusCode?: number): void {
  const message =
    statusCode !== undefined
      ? `A server-side application error occurred while sending an API request (HTTP status code ${statusCode.toString()}). Please check the browser's developer tools for details.`
      : "A server-side application error occurred while sending an API request. Please check the browser's developer tools for details.";

  apiErrorLogger(message);
}

function onRequestClientError(apiErrorLogger: ApiErrorLogger): void {
  apiErrorLogger("A client-side application error occurred while sending an API request. Please check the browser's developer tools for details.");
}

function onRequestUnknownError(apiErrorLogger: ApiErrorLogger): void {
  apiErrorLogger("An unknown application error occurred while sending an API request.");
}

export function displayUnsuccessfulRequestError(error: unknown, apiErrorLogger: ApiErrorLogger, rethrow?: boolean): void {
  console.error("Error:", error);

  if (isAxiosError<ProblemDetails>(error)) {
    const axiosError: AxiosError<ProblemDetails> = error;

    if (axiosError.request) {
      console.error("Failed API request:", axiosError.request);
    }
    if (axiosError.response) {
      console.error("Failed API request's response:", axiosError.response);
    }

    const statusCode = axiosError.response?.status;
    if (statusCode !== undefined) {
      onRequestServerError(apiErrorLogger, statusCode);
    } else if (axiosError.response) {
      onRequestServerError(apiErrorLogger);
    } else if (axiosError.request) {
      onRequestClientError(apiErrorLogger);
    } else {
      onRequestUnknownError(apiErrorLogger);
    }
  } else {
    onRequestUnknownError(apiErrorLogger);
  }

  if (rethrow) {
    throw error;
  }
}
