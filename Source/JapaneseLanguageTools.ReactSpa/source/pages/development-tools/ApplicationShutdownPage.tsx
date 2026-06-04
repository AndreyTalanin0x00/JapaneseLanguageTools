import { Alert, Typography } from "antd";
import { useEffect, useMemo, useState } from "react";

import logApiError from "@/api/helpers/logApiError.antd";
import { displayUnsuccessfulRequestError } from "@/api/helpers/unsuccessfulRequestHelpers";
import { shutdown } from "@/api/applicationLifetimeMethods";

const { Paragraph, Title } = Typography;

const ApplicationShutdownPage = () => {
  const [shutdownRequestSuccess, setShutdownRequestSuccess] = useState<boolean>();

  useEffect(() => {
    shutdown()
      .then(() => {
        setShutdownRequestSuccess(true);
      })
      .catch((error: unknown) => {
        displayUnsuccessfulRequestError(error, logApiError);
        setShutdownRequestSuccess(false);
      });
  }, []);

  const shutdownRequestStatusAlertJsx = useMemo(() => {
    if (shutdownRequestSuccess !== undefined) {
      return shutdownRequestSuccess ? (
        <Alert message="Application shutdown is in progress." type="success" showIcon />
      ) : (
        <Alert message="The application shutdown request has failed (not allowed outside the Development environment)." type="error" showIcon />
      );
    } else {
      return undefined;
    }
  }, [shutdownRequestSuccess]);

  return (
    <>
      <Title level={4}>Application Shutdown</Title>
      <Paragraph>Trying to initiate application shutdown...</Paragraph>
      {shutdownRequestStatusAlertJsx}
    </>
  );
};

export default ApplicationShutdownPage;
