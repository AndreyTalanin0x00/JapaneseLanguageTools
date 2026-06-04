import { Button, Card, Space, Typography } from "antd";
import { useNavigate } from "react-router-dom";

import { PoweroffOutlined, ToolOutlined } from "@ant-design/icons";

import styles from "./DevelopmentToolSelectionPage.module.css";

const { Text, Title } = Typography;

const DevelopmentToolSelectionPage = () => {
  const navigate = useNavigate();

  return (
    <>
      <Title level={4}>Development Tools</Title>
      <Space style={{ display: "flex" }} direction="vertical">
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                <ToolOutlined />
                <Text>Swagger API Explorer</Text>
              </Space>
              <Space wrap direction="horizontal" align="baseline">
                <Button type="primary" size="small" onClick={() => void navigate("/dev-tools/swagger-redirect")}>
                  Open
                </Button>
              </Space>
            </Space>
          }
        >
          The Swagger API Explorer allows the developer to browse and test available API endpoints.
        </Card>
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                <PoweroffOutlined />
                <Text>Application Shutdown</Text>
              </Space>
              <Space wrap direction="horizontal" align="baseline">
                <Button type="primary" size="small" danger onClick={() => void navigate("/dev-tools/application-shutdown")}>
                  Initiate Shutdown
                </Button>
              </Space>
            </Space>
          }
        >
          This option allows to gracefully shut down the application from the user interface.
        </Card>
      </Space>
    </>
  );
};

export default DevelopmentToolSelectionPage;
