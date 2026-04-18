import { Button, Card, Space, Typography } from "antd";
import { useNavigate } from "react-router-dom";

import { BookOutlined, TagOutlined } from "@ant-design/icons";

import styles from "./IntegrationSelectionPage.module.css";

const { Text, Title } = Typography;

const IntegrationSelectionPage = () => {
  const navigate = useNavigate();

  return (
    <>
      <Title level={4}>Integrations</Title>
      <Space style={{ display: "flex" }} direction="vertical">
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                <BookOutlined />
                <Text>Application Dictionary</Text>
              </Space>
              <Space wrap direction="horizontal" align="baseline">
                <Button type="primary" size="small" onClick={() => void navigate("/integrations/application-dictionary")}>
                  Select
                </Button>
              </Space>
            </Space>
          }
        >
          The application dictionary integrations allow the user to export & import characters, character groups, words and word groups.
        </Card>
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                <TagOutlined />
                <Text>Tags</Text>
              </Space>
              <Space wrap direction="horizontal" align="baseline">
                <Button type="primary" size="small" onClick={() => void navigate("/integrations/tags")}>
                  Select
                </Button>
              </Space>
            </Space>
          }
        >
          The tags integrations allow the user to export & import tags, both system-reserved and user-defined.
        </Card>
      </Space>
    </>
  );
};

export default IntegrationSelectionPage;
