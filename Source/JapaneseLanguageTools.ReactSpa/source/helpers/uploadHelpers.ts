import { UploadFile, UploadProps } from "antd";

type CustomRequest = UploadProps["customRequest"];

export const dummyUploadRequest: CustomRequest = ({ onSuccess }) => {
  setTimeout(() => {
    if (onSuccess) {
      onSuccess("The file was uploaded successfully.");
    }
  }, 0);
};

export const getUploadValueFromEvent = (args: UploadFile[] | { fileList: UploadFile[] } | undefined): UploadFile[] | undefined => {
  if (args == undefined) {
    return args;
  } else if (Array.isArray(args)) {
    return args;
  } else {
    const { fileList } = args;
    return fileList;
  }
};
