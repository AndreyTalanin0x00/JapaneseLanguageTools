import { Button, Card, Space, Typography } from "antd";
import { useNavigate } from "react-router-dom";

import { FormOutlined, ProfileOutlined } from "@ant-design/icons";

import styles from "./ExerciseSelectionPage.module.css";

const { Text, Title } = Typography;

const ExerciseSelectionPage = () => {
  const navigate = useNavigate();

  return (
    <>
      <Title level={4}>Exercises</Title>
      <Space style={{ display: "flex" }} direction="vertical">
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                <FormOutlined />
                <Text>Character Exercise</Text>
              </Space>
              <Space wrap direction="horizontal" align="baseline">
                <Button type="primary" size="small" onClick={() => void navigate("/exercises/characters")}>
                  Select
                </Button>
              </Space>
            </Space>
          }
        >
          The character exercises allow the user to practice kanji, hiragana and katakana characters, both writing and pronunciation.
        </Card>
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                <ProfileOutlined />
                <Text>Word Exercise</Text>
              </Space>
              <Space wrap direction="horizontal" align="baseline">
                <Button type="primary" size="small" onClick={() => void navigate("/exercises/words")}>
                  Select
                </Button>
              </Space>
            </Space>
          }
        >
          The word exercises allow the user to practice words (composed of kanji, hiragana and katakana characters), paying attention to all aspects: writing, pronunciation and meaning. Furigana is also available for words written in kanji.
        </Card>
      </Space>
    </>
  );
};

export default ExerciseSelectionPage;
