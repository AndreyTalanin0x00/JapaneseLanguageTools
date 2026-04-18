import { Alert, Button, Card, Form, message, Select, Space, Tabs, Typography, Upload, UploadFile } from "antd";
import { saveAs } from "file-saver";
import { useCallback, useContext, useEffect, useMemo, useState } from "react";

import { DownloadOutlined, ExportOutlined, ImportOutlined, InboxOutlined, LoadingOutlined, UndoOutlined, UploadOutlined } from "@ant-design/icons";

import logApiError from "@/api/helpers/logApiError.antd";
import { displayUnsuccessfulRequestError } from "@/api/helpers/unsuccessfulRequestHelpers";
import { downloadExportBlob, exportTags, getExportBlobMetadata } from "@/api/tagMethods.export";
import { importTags, uploadImportBlob } from "@/api/tagMethods.import";
import * as FileConstants from "@/constants/FileConstants";
import MobileBrowserContext from "@/contexts/MobileBrowserContext";
import SnapshotFileFormat from "@/enumerations/SnapshotFileFormat";
import SnapshotType from "@/enumerations/SnapshotType";
import { getUploadValueFromEvent } from "@/helpers/uploadHelpers";
import DownloadBlobRequestModel from "@/models/blobs/requests/DownloadBlobRequest.Model";
import GetBlobMetadataRequestModel from "@/models/blobs/requests/GetBlobMetadataRequest.Model";
import UploadBlobResponseModel from "@/models/blobs/responses/UploadBlobResponse.Model";
import ExportTagsRequestModel from "@/models/requests/x-export/ExportTagsRequest.Model";
import ImportTagsRequestModel from "@/models/requests/x-import/ImportTagsRequest.Model";
import keyOf from "@/typescript/keyOf";

import styles from "./TagIntegrationPage.module.css";

const { Title } = Typography;

const TagIntegrationPage = () => {
  const mobileBrowserMode = useContext(MobileBrowserContext);

  const [exportLoading, setExportLoading] = useState<boolean>(false);
  const [exportSuccessful, setExportSuccessful] = useState<boolean | undefined>(undefined);

  interface TagExportConfiguration {
    snapshotType: SnapshotType;
    snapshotFileFormat: SnapshotFileFormat;
  }

  const [exportConfigurationForm] = Form.useForm<TagExportConfiguration>();

  const initialExportConfiguration: TagExportConfiguration = useMemo(
    () => ({
      snapshotType: SnapshotType.General,
      snapshotFileFormat: SnapshotFileFormat.Json,
    }),
    []
  );

  useEffect(() => exportConfigurationForm.resetFields(), [exportConfigurationForm, initialExportConfiguration]);

  const onExportButtonClick = useCallback(() => {
    exportConfigurationForm.submit();
  }, [exportConfigurationForm]);

  const onExportResetButtonClick = useCallback(() => {
    setExportSuccessful(undefined);
    exportConfigurationForm.resetFields();
  }, [exportConfigurationForm]);

  const onExportFormFinish = useCallback((tagExportConfiguration: TagExportConfiguration) => {
    const { snapshotType, snapshotFileFormat } = tagExportConfiguration;

    const exportTagsRequestModel: ExportTagsRequestModel = { snapshotType, snapshotFileFormat };

    setExportLoading(true);
    exportTags(exportTagsRequestModel)
      .then((exportTagsResponseModel) => {
        const { blobReference } = exportTagsResponseModel;

        const downloadExportBlobRequestModel: DownloadBlobRequestModel = { blobReference };
        const getExportBlobMetadataRequestModel: GetBlobMetadataRequestModel = { blobReference };

        Promise.all([downloadExportBlob(downloadExportBlobRequestModel), getExportBlobMetadata(getExportBlobMetadataRequestModel)])
          .then((promiseResults) => {
            const [blob, { blobMetadata }] = promiseResults;

            saveAs(blob, blobMetadata.fileName);

            setExportLoading(false);
            setExportSuccessful(true);
          })
          .catch((error: unknown) => {
            displayUnsuccessfulRequestError(error, logApiError);

            setExportLoading(false);
            setExportSuccessful(false);
          });
      })
      .catch((error: unknown) => {
        displayUnsuccessfulRequestError(error, logApiError);

        setExportLoading(false);
        setExportSuccessful(false);
      });
  }, []);

  const onExportFormFinishFailure = useCallback(() => {
    console.error("Form validation failed.");
  }, []);

  const exportControlsJsx = useMemo(() => {
    return (
      <Space wrap direction="horizontal" align="baseline">
        <Button type="primary" size="small" icon={exportLoading ? <LoadingOutlined /> : <DownloadOutlined />} onClick={onExportButtonClick}>
          Export
        </Button>
        <Button type="primary" size="small" icon={<UndoOutlined />} onClick={onExportResetButtonClick} danger>
          Reset
        </Button>
      </Space>
    );
  }, [exportLoading, onExportButtonClick, onExportResetButtonClick]);

  const exportTabContent = useMemo(() => {
    return (
      <Space style={{ display: "flex" }} direction="vertical" className={mobileBrowserMode ? styles.exportTabContentMobile : styles.exportTabContent}>
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                Configuration
              </Space>
              {!mobileBrowserMode ? exportControlsJsx : undefined}
            </Space>
          }
        >
          <Space style={{ display: "flex" }} direction="vertical">
            <Form form={exportConfigurationForm} initialValues={initialExportConfiguration} onFinish={onExportFormFinish} onFinishFailed={onExportFormFinishFailure} size="small" labelCol={{ span: 8 }} wrapperCol={{ span: 16 }}>
              <Form.Item name={keyOf<TagExportConfiguration>("snapshotType")} label="Snapshot Type">
                <Select placeholder="Select the snapshot type">
                  <Select.Option value={SnapshotType.General}>General</Select.Option>
                  <Select.Option value={SnapshotType.GeneralNoAction}>General (No Default Action)</Select.Option>
                  {/* The SnapshotType.ChangeState option does not make sense for the tag export/import. */}
                  {/* <Select.Option value={SnapshotType.ChangeState}>Change State (Enable / Disable)</Select.Option> */}
                  <Select.Option value={SnapshotType.Patch}>Patch</Select.Option>
                </Select>
              </Form.Item>
              <Form.Item name={keyOf<TagExportConfiguration>("snapshotFileFormat")} label="Snapshot File Format">
                <Select placeholder="Select the snapshot file format">
                  <Select.Option value={SnapshotFileFormat.Json}>JSON</Select.Option>
                  <Select.Option value={SnapshotFileFormat.Xml}>XML</Select.Option>
                </Select>
              </Form.Item>
            </Form>
            {mobileBrowserMode ? exportControlsJsx : undefined}
          </Space>
        </Card>
        {exportSuccessful !== undefined ? <Alert message={exportSuccessful ? "The export was successful." : "The export was unsuccessful."} type={exportSuccessful ? "success" : "error"} showIcon closable /> : undefined}
      </Space>
    );
  }, [mobileBrowserMode, exportSuccessful, exportConfigurationForm, initialExportConfiguration, exportControlsJsx, onExportFormFinish, onExportFormFinishFailure]);

  const [importLoading, setImportLoading] = useState<boolean>(false);
  const [importSuccessful, setImportSuccessful] = useState<boolean | undefined>(undefined);

  interface TagImportConfiguration {
    uploadFiles: UploadFile[];
  }

  const [importConfigurationForm] = Form.useForm<TagImportConfiguration>();

  const initialImportConfiguration: TagImportConfiguration = useMemo(
    () => ({
      uploadFiles: [],
    }),
    []
  );

  useEffect(() => importConfigurationForm.resetFields(), [importConfigurationForm, initialImportConfiguration]);

  const onImportButtonClick = useCallback(() => {
    importConfigurationForm.submit();
  }, [importConfigurationForm]);

  const onImportResetButtonClick = useCallback(() => {
    setImportSuccessful(undefined);
    importConfigurationForm.resetFields();
  }, [importConfigurationForm]);

  const onImportFormFinish = useCallback((tagImportConfiguration: TagImportConfiguration) => {
    const { uploadFiles } = tagImportConfiguration;

    const blobReferences = uploadFiles.filter((uploadFile) => uploadFile.status == "done").map((uploadFile) => (uploadFile.response as UploadBlobResponseModel).blobReference);

    const importTagsRequestModel: ImportTagsRequestModel = {
      blobReferences,
    };

    setImportLoading(true);
    importTags(importTagsRequestModel)
      .then(() => {
        setImportLoading(false);
        setImportSuccessful(true);
      })
      .catch((error: unknown) => {
        displayUnsuccessfulRequestError(error, logApiError);

        setImportLoading(false);
        setImportSuccessful(false);
      });
  }, []);

  const onImportFormFinishFailure = useCallback(() => {
    console.error("Form validation failed.");
  }, []);

  const importControlsJsx = useMemo(() => {
    return (
      <Space wrap direction="horizontal" align="baseline">
        <Button type="primary" size="small" icon={importLoading ? <LoadingOutlined /> : <UploadOutlined />} onClick={onImportButtonClick}>
          Import
        </Button>
        <Button type="primary" size="small" icon={<UndoOutlined />} onClick={onImportResetButtonClick} danger>
          Reset
        </Button>
      </Space>
    );
  }, [importLoading, onImportButtonClick, onImportResetButtonClick]);

  const importTabContent = useMemo(() => {
    return (
      <Space style={{ display: "flex" }} direction="vertical" className={mobileBrowserMode ? styles.importTabContentMobile : styles.importTabContent}>
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                Configuration
              </Space>
              {!mobileBrowserMode ? importControlsJsx : undefined}
            </Space>
          }
        >
          <Space style={{ display: "flex" }} direction="vertical">
            <Form form={importConfigurationForm} initialValues={initialImportConfiguration} onFinish={onImportFormFinish} onFinishFailed={onImportFormFinishFailure} size="small" labelCol={{ span: 8 }} wrapperCol={{ span: 16 }}>
              <Form.Item name={keyOf<TagImportConfiguration>("uploadFiles")} label="Upload Files" valuePropName="fileList" getValueFromEvent={getUploadValueFromEvent} noStyle>
                <Upload.Dragger
                  multiple={true}
                  accept={FileConstants.applicationDictionaryImportUploadAccept}
                  showUploadList={{
                    showRemoveIcon: true,
                  }}
                  onChange={(info) => {
                    const { status } = info.file;
                    if (status == "done") {
                      message.success(`${info.file.name} file uploaded successfully.`);
                    } else if (status === "error") {
                      message.error(`${info.file.name} file upload failed.`);
                    }
                  }}
                  customRequest={(uploadRequestOption) => {
                    const { file, onSuccess, onProgress, onError } = uploadRequestOption;

                    uploadImportBlob(file as File, onProgress)
                      .then((uploadBlobResponseModel) => {
                        if (onSuccess) {
                          onSuccess(uploadBlobResponseModel);
                        }
                      })
                      .catch((error: unknown) => {
                        if (onError) {
                          onError(error as Error);
                        }
                      });
                  }}
                >
                  <p className="ant-upload-drag-icon">
                    <InboxOutlined />
                  </p>
                  <p className="ant-upload-text">Click or drag files to this area to upload</p>
                  <p className="ant-upload-hint">Accepted file extensions: {FileConstants.tagImportUploadAccept.split(",").join(", ")}. Please, note that files will be processed in the order they are uploaded.</p>
                </Upload.Dragger>
              </Form.Item>
            </Form>
            {mobileBrowserMode ? importControlsJsx : undefined}
          </Space>
        </Card>
        {importSuccessful !== undefined ? <Alert message={importSuccessful ? "The import was successful." : "The import was unsuccessful."} type={importSuccessful ? "success" : "error"} showIcon closable /> : undefined}
      </Space>
    );
  }, [mobileBrowserMode, importSuccessful, importConfigurationForm, initialImportConfiguration, onImportFormFinish, onImportFormFinishFailure, importControlsJsx]);

  return (
    <>
      <Title level={4}>Tag Integrations</Title>
      <Tabs
        defaultActiveKey="export"
        items={[
          { key: "export", label: "Export", icon: <ExportOutlined />, children: exportTabContent, forceRender: true },
          { key: "import", label: "Import", icon: <ImportOutlined />, children: importTabContent, forceRender: true },
        ]}
      />
    </>
  );
};

export default TagIntegrationPage;
