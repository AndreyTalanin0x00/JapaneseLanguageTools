import { Typography } from "antd";
import { useEffect } from "react";
import { useLocation } from "react-router-dom";

import { developmentServerPort } from "@/ApplicationEnvironment";

const { Paragraph, Title } = Typography;

const SwaggerRedirectPage = () => {
  const location = useLocation();

  useEffect(() => {
    if (window.location.port != developmentServerPort.toString()) {
      const swaggerLocation = `${window.location.origin}${location.pathname}`;
      window.location.replace(swaggerLocation);
    } else {
      console.error("You can only use the Swagger API Explorer from the ASP.NET Core application.");
    }
  }, [location]);

  return (
    <>
      <Title level={4}>Swagger API Explorer</Title>
      <Paragraph>Redirecting to the Swagger API Explorer page...</Paragraph>
    </>
  );
};

export default SwaggerRedirectPage;
