import { Alert, Button, Card, Divider, Empty, Form, message, Radio, Select, Space, Table, Typography } from "antd";
import { useCallback, useContext, useEffect, useMemo, useState } from "react";

import {
  ArrowRightOutlined,
  ArrowUpOutlined,
  CheckOutlined,
  ClockCircleOutlined,
  CloseOutlined,
  LeftOutlined,
  LoadingOutlined,
  MobileOutlined,
  ProfileOutlined,
  QuestionOutlined,
  RightOutlined,
  TableOutlined,
  UndoOutlined,
} from "@ant-design/icons";

import logApiError from "@/api/helpers/logApiError.antd";
import { displayUnsuccessfulRequestError } from "@/api/helpers/unsuccessfulRequestHelpers";
import { generateWordExerciseBatch, getWordExerciseBatch, submitCompletedWordExerciseDetails, submitFailedWordExerciseDetails } from "@/api/applicationDictionaryExerciseMethods";
import { getAllWordGroups } from "@/api/applicationDictionaryMethods";
import { wordExercisePreferencesDefaultValues } from "@/constants/ApplicationPreferencesConstants";
import ExerciseSizeInputNumber from "@/components/ExerciseSizeInputNumber";
import ApplicationPreferencesContext from "@/contexts/ApplicationPreferencesContext";
import MobileBrowserContext from "@/contexts/MobileBrowserContext";
import WordExercisePreferences from "@/entities/preferences/WordExercisePreferences";
import WordExerciseProfile from "@/entities/preferences/WordExerciseProfile";
import WordExerciseMode from "@/enumerations/types/WordExerciseMode";
import { CharacterTypesUtils } from "@/enumerations/CharacterTypes";
import WordProperties from "@/enumerations/WordProperties";
import WordExerciseBatchModel from "@/models/WordExerciseBatch.Model";
import WordGroupModel from "@/models/WordGroup.Model";
import GenerateWordExerciseBatchRequestModel from "@/models/requests/GenerateWordExerciseBatchRequest.Model";
import GetWordExerciseBatchRequestModel from "@/models/requests/GetWordExerciseBatchRequest.Model";
import SubmitCompletedWordExerciseDetailsRequestModel from "@/models/requests/SubmitCompletedWordExerciseDetailsRequest.Model";
import SubmitFailedWordExerciseDetailsRequestModel from "@/models/requests/SubmitFailedWordExerciseDetailsRequest.Model";
import GenerateWordExerciseBatchResponseModel from "@/models/responses/GenerateWordExerciseBatchResponse.Model";
import keyOf from "@/typescript/keyOf";
import valueOf from "@/typescript/valueOf";

import styles from "./WordExercisePage.module.css";

const { Text, Title, Paragraph } = Typography;

interface WordExerciseConfiguration {
  mode: WordExerciseMode;
  useWordGroupIds: number[];
  useWordExerciseProfile?: WordExerciseProfile;
  size: number;
}

interface WordExerciseSession {
  startedOn: Date;
  wordExerciseBatchModel: WordExerciseBatchModel;
  generateWordExerciseBatchResponseModel: GenerateWordExerciseBatchResponseModel;
  peekedIndexes: Set<number>;
  displayedIndexes: Set<number>;
  completedIndexes: Set<number>;
  completed: boolean;
  submitted: boolean;
}

interface WordRecord {
  id: number;
  wordExerciseId: number;
  index: number;
  characters: string;
  characterTypes: string;
  pronunciation?: string;
  furigana?: string;
  meaning?: string;
  peeked: boolean;
  displayed: boolean;
  completed: boolean;
}

type WordExerciseScheduledAction = () => void;

interface WordExerciseViewProps {
  wordRecords: WordRecord[];
  wordExerciseSession: WordExerciseSession;
  wordExerciseConfiguration: WordExerciseConfiguration;
  wordExercisePreferences?: WordExercisePreferences;
  onWordExercisePeek: (wordRecordIndex: number) => void;
  onWordExercisePeekRevert: (wordRecordIndex: number) => void;
  onWordExerciseDisplay: (wordRecordIndex: number) => void;
  onWordExerciseDisplayRevert: (wordRecordIndex: number) => void;
  onWordExerciseComplete: (wordRecordIndex: number) => void;
  onWordExerciseCompleteRevert: (wordRecordIndex: number) => void;
  runWordExerciseScheduledAction: (wordExerciseScheduledAction: WordExerciseScheduledAction, delayMilliseconds: number) => void;
}

const WordExercisePage = () => {
  const mobileBrowserMode = useContext(MobileBrowserContext);

  const {
    applicationPreferences: { wordExercisePreferences },
  } = useContext(ApplicationPreferencesContext);

  const [wordGroups, setWordGroups] = useState<WordGroupModel[]>([]);

  useEffect(() => {
    getAllWordGroups()
      .then((wordGroups) => setWordGroups(wordGroups.filter((wordGroup) => wordGroup.enabled && !wordGroup.hidden)))
      .catch((error: unknown) => displayUnsuccessfulRequestError(error, logApiError));
  }, []);

  const [wordExerciseConfiguration, setWordExerciseConfiguration] = useState<WordExerciseConfiguration | undefined>(undefined);

  interface WordExerciseConfigurationFormObject extends Omit<WordExerciseConfiguration, "useWordExerciseProfile"> {
    useWordExerciseProfileId?: string;
  }

  const initialWordExerciseConfigurationFormObject = useMemo<WordExerciseConfigurationFormObject>(
    () => ({
      mode: wordExercisePreferences?.defaultExerciseMode ?? wordExercisePreferencesDefaultValues.defaultExerciseMode,
      useWordExerciseProfileId: wordExercisePreferences?.defaultExerciseProfileId ?? wordExercisePreferencesDefaultValues.defaultExerciseProfileId,
      useWordGroupIds: wordExercisePreferences?.defaultWordGroupIds ?? wordExercisePreferencesDefaultValues.defaultWordGroupIds,
      size: wordExercisePreferences?.defaultExerciseSize ?? wordExercisePreferencesDefaultValues.defaultExerciseSize,
    }),
    [wordExercisePreferences]
  );

  const [wordExerciseConfigurationForm] = Form.useForm<WordExerciseConfigurationFormObject>();

  useEffect(() => wordExerciseConfigurationForm.resetFields(), [wordExerciseConfigurationForm, initialWordExerciseConfigurationFormObject]);

  const [wordExerciseSession, setWordExerciseSession] = useState<WordExerciseSession | undefined>(undefined);
  const [wordExerciseSessionLoading, setWordExerciseSessionLoading] = useState<boolean>(false);

  const wordRecords = useMemo(() => {
    const wordRecords: WordRecord[] = [];

    if (wordExerciseSession == undefined || wordExerciseConfiguration == undefined) {
      return wordRecords;
    }

    const {
      wordExerciseBatchModel: { items: wordExerciseModels },
      peekedIndexes,
      displayedIndexes,
      completedIndexes,
    } = wordExerciseSession;

    const { mode } = wordExerciseConfiguration;

    const getHiddenWordProperties = (mode: WordExerciseMode) => {
      if (mode === "characterTypes") {
        return WordProperties.CharacterTypes;
      } else if (mode === "characterTypes-pronunciation") {
        return WordProperties.CharacterTypes | WordProperties.Pronunciation | WordProperties.Furigana;
      } else if (mode === "characters") {
        return WordProperties.Characters;
      } else if (mode === "characters-pronunciation") {
        return WordProperties.Characters | WordProperties.Pronunciation | WordProperties.Furigana;
      } else {
        /* if (mode === "fullDescription") */
        return WordProperties.CharacterTypes | WordProperties.Pronunciation | WordProperties.Furigana | WordProperties.Meaning;
      }
    };

    const hiddenWordProperties = getHiddenWordProperties(mode);

    const hiddenWordPropertyPlaceholder = "???";

    for (const [index, { id: wordExerciseId, word }] of wordExerciseModels.entries()) {
      if (word == undefined) {
        continue;
      }

      const wordRecord: WordRecord = {
        id: word.id,
        wordExerciseId: wordExerciseId,
        index: index,
        characters: word.characters,
        characterTypes: CharacterTypesUtils.characterTypesToString(word.characterTypes),
        pronunciation: word.pronunciation,
        furigana: word.furigana,
        meaning: word.meaning,
        peeked: peekedIndexes.has(index),
        displayed: peekedIndexes.has(index) || displayedIndexes.has(index),
        completed: completedIndexes.has(index),
      };

      if (hiddenWordProperties & WordProperties.CharacterTypes && !(wordRecord.peeked || wordRecord.displayed)) {
        wordRecord.characterTypes = hiddenWordPropertyPlaceholder;
      }
      if (hiddenWordProperties & WordProperties.Characters && !(wordRecord.peeked || wordRecord.displayed)) {
        wordRecord.characters = hiddenWordPropertyPlaceholder;
      }
      if (hiddenWordProperties & WordProperties.Pronunciation && !(wordRecord.peeked || wordRecord.displayed)) {
        wordRecord.pronunciation = hiddenWordPropertyPlaceholder;
      }
      if (hiddenWordProperties & WordProperties.Furigana && !(wordRecord.peeked || wordRecord.displayed)) {
        wordRecord.furigana = hiddenWordPropertyPlaceholder;
      }
      if (hiddenWordProperties & WordProperties.Meaning && !(wordRecord.peeked || wordRecord.displayed)) {
        wordRecord.meaning = hiddenWordPropertyPlaceholder;
      }

      wordRecords.push(wordRecord);
    }

    return wordRecords;
  }, [wordExerciseSession, wordExerciseConfiguration]);

  interface WordExerciseResults {
    wordsTotal: number;
    wordsTotalDivisionSafe: number;
    wordsPeeked: number;
    wordsDisplayed: number;
    wordsCompleted: number;
  }

  const wordExerciseResults = useMemo(() => {
    if (wordExerciseSession == undefined) {
      return undefined;
    }

    const wordsTotal = wordExerciseSession.wordExerciseBatchModel.items.length;
    const wordsTotalDivisionSafe = wordsTotal > 0 ? wordsTotal : 1;
    const wordsPeeked = wordExerciseSession.peekedIndexes.size;
    const wordsDisplayed = wordExerciseSession.displayedIndexes.size;
    const wordsCompleted = wordExerciseSession.completedIndexes.size;

    const wordExerciseResults: WordExerciseResults = {
      wordsTotal,
      wordsTotalDivisionSafe,
      wordsPeeked,
      wordsDisplayed,
      wordsCompleted,
    };

    return wordExerciseResults;
  }, [wordExerciseSession]);

  const onGenerateButtonClick = useCallback(() => {
    wordExerciseConfigurationForm.submit();
  }, [wordExerciseConfigurationForm]);

  const onResetButtonClick = useCallback(() => {
    wordExerciseConfigurationForm.resetFields();

    setWordExerciseConfiguration(undefined);
    setWordExerciseSession(undefined);
  }, [wordExerciseConfigurationForm]);

  type WordExerciseViewMode = "table-view" | "cards-view";

  const [wordExerciseViewMode, setWordExerciseViewMode] = useState<WordExerciseViewMode>("table-view");

  const exerciseGenerationControlsJsx = useMemo(() => {
    return (
      <Space wrap direction="horizontal" align="baseline">
        <Button type="primary" size="small" icon={!wordExerciseSessionLoading ? <ProfileOutlined /> : <LoadingOutlined />} onClick={onGenerateButtonClick}>
          Generate
        </Button>
        <Button type="primary" size="small" icon={<UndoOutlined />} onClick={onResetButtonClick} danger>
          Reset
        </Button>
        {!mobileBrowserMode ? (
          <Radio.Group size="small" value={wordExerciseViewMode} onChange={(e) => setWordExerciseViewMode(e.target.value as WordExerciseViewMode)}>
            <Radio.Button value={valueOf<WordExerciseViewMode>("table-view")}>
              <TableOutlined /> Table View
            </Radio.Button>
            <Radio.Button value={valueOf<WordExerciseViewMode>("cards-view")}>
              <MobileOutlined /> Cards View
            </Radio.Button>
          </Radio.Group>
        ) : undefined}
      </Space>
    );
  }, [mobileBrowserMode, wordExerciseSessionLoading, onGenerateButtonClick, onResetButtonClick, wordExerciseViewMode]);

  const getWordExerciseProfile = useCallback(
    (wordExerciseProfileId?: string) => {
      if (!wordExerciseProfileId) {
        return undefined;
      }

      return (wordExercisePreferences?.exerciseProfiles ?? []).find((exerciseProfile) => exerciseProfile.id == wordExerciseProfileId);
    },
    [wordExercisePreferences]
  );

  const onWordExerciseConfigurationFormFinish = useCallback(
    (exerciseConfiguration: WordExerciseConfigurationFormObject) => {
      const { mode, useWordGroupIds, useWordExerciseProfileId, size } = exerciseConfiguration;

      const useWordExerciseProfile = getWordExerciseProfile(useWordExerciseProfileId);

      setWordExerciseConfiguration({ mode, useWordGroupIds, useWordExerciseProfile, size });

      setWordExerciseSessionLoading(true);

      const generateWordExerciseBatchRequestModel: GenerateWordExerciseBatchRequestModel = {
        size: size,
        useWordExerciseProfile: {
          repeatedChallengeCountProgression: useWordExerciseProfile?.repeatedChallengeCountProgression ?? [],
          tagDistributionPreferences: useWordExerciseProfile?.tagDistributionSettings,
        },
        useWordGroupIds: useWordGroupIds,
      };

      generateWordExerciseBatch(generateWordExerciseBatchRequestModel)
        .then((generateWordExerciseBatchResponseModel) => {
          const getWordExerciseBatchRequestModel: GetWordExerciseBatchRequestModel = {
            wordExerciseBatchId: generateWordExerciseBatchResponseModel.wordExerciseBatchId,
          };

          getWordExerciseBatch(getWordExerciseBatchRequestModel)
            .then((getWordExerciseBatchResponseModel) => {
              const { wordExerciseBatch: wordExerciseBatchModel } = getWordExerciseBatchResponseModel;

              const wordExerciseSession: WordExerciseSession = {
                startedOn: new Date(),
                wordExerciseBatchModel: wordExerciseBatchModel,
                generateWordExerciseBatchResponseModel: generateWordExerciseBatchResponseModel,
                peekedIndexes: new Set<number>(),
                displayedIndexes: new Set<number>(),
                completedIndexes: new Set<number>(),
                completed: false,
                submitted: false,
              };

              setWordExerciseSession(wordExerciseSession);
              setWordExerciseSessionLoading(false);
            })
            .catch((error: unknown) => {
              displayUnsuccessfulRequestError(error, logApiError);
              setWordExerciseSessionLoading(false);
            });
        })
        .catch((error: unknown) => {
          displayUnsuccessfulRequestError(error, logApiError);
          setWordExerciseSessionLoading(false);
        });
    },
    [getWordExerciseProfile]
  );

  const onWordExerciseConfigurationFormFinishFailed = useCallback(() => {
    message.error("Form validation failed.");
  }, []);

  const setWordExercisePeeked = useCallback((index: number, peeked: boolean) => {
    const setPeekedIndexes = (updatePeekedIndexesAction: (peekedIndexes: Set<number>) => Set<number>) => {
      setWordExerciseSession((wordExerciseSession) => (wordExerciseSession ? { ...wordExerciseSession, peekedIndexes: updatePeekedIndexesAction(wordExerciseSession.peekedIndexes) } : undefined));
    };

    if (peeked) {
      setPeekedIndexes((peekedIndexes) => new Set<number>([...Array.from(peekedIndexes.values()), index]));
    } else {
      setPeekedIndexes((peekedIndexes) => new Set<number>(Array.from(peekedIndexes.values()).filter((indexToCompare) => index !== indexToCompare)));
    }
  }, []);

  const setWordExerciseDisplayed = useCallback((index: number, displayed: boolean) => {
    const setDisplayedIndexes = (updateDisplayedIndexesAction: (displayedIndexes: Set<number>) => Set<number>) => {
      setWordExerciseSession((wordExerciseSession) => (wordExerciseSession ? { ...wordExerciseSession, displayedIndexes: updateDisplayedIndexesAction(wordExerciseSession.displayedIndexes) } : undefined));
    };

    if (displayed) {
      setDisplayedIndexes((displayedIndexes) => new Set<number>([...Array.from(displayedIndexes.values()), index]));
    } else {
      setDisplayedIndexes((displayedIndexes) => new Set<number>(Array.from(displayedIndexes.values()).filter((indexToCompare) => index !== indexToCompare)));
    }
  }, []);

  const setWordExerciseCompleted = useCallback((index: number, completed: boolean) => {
    const setCompletedIndexes = (updateCompletedIndexesAction: (completedIndexes: Set<number>) => Set<number>) => {
      setWordExerciseSession((wordExerciseSession) => (wordExerciseSession ? { ...wordExerciseSession, completedIndexes: updateCompletedIndexesAction(wordExerciseSession.completedIndexes) } : undefined));
    };

    if (completed) {
      setCompletedIndexes((completedIndexes) => new Set<number>([...Array.from(completedIndexes.values()), index]));
    } else {
      setCompletedIndexes((completedIndexes) => new Set<number>(Array.from(completedIndexes.values()).filter((indexToCompare) => index !== indexToCompare)));
    }
  }, []);

  const onWordExercisePeek = useCallback(
    (wordRecordIndex: number) => {
      setWordExercisePeeked(wordRecordIndex, true);
    },
    [setWordExercisePeeked]
  );

  const onWordExercisePeekRevert = useCallback(
    (wordRecordIndex: number) => {
      setWordExercisePeeked(wordRecordIndex, false);
    },
    [setWordExercisePeeked]
  );

  const onWordExerciseDisplay = useCallback(
    (wordRecordIndex: number) => {
      setWordExerciseDisplayed(wordRecordIndex, true);
    },
    [setWordExerciseDisplayed]
  );

  const onWordExerciseDisplayRevert = useCallback(
    (wordRecordIndex: number) => {
      setWordExerciseDisplayed(wordRecordIndex, false);
    },
    [setWordExerciseDisplayed]
  );

  const onWordExerciseComplete = useCallback(
    (wordRecordIndex: number) => {
      setWordExerciseCompleted(wordRecordIndex, true);
    },
    [setWordExerciseCompleted]
  );

  const onWordExerciseCompleteRevert = useCallback(
    (wordRecordIndex: number) => {
      setWordExerciseCompleted(wordRecordIndex, false);
    },
    [setWordExerciseCompleted]
  );

  const [wordExerciseScheduledAction, setWordExerciseScheduledAction] = useState<WordExerciseScheduledAction | undefined>(undefined);

  const runWordExerciseScheduledAction = useCallback((wordExerciseScheduledAction: WordExerciseScheduledAction, delayMilliseconds: number) => {
    setWordExerciseScheduledAction(() => wordExerciseScheduledAction);
    setTimeout(() => {
      wordExerciseScheduledAction();
      setWordExerciseScheduledAction((wordExerciseScheduledActionToClear) => (wordExerciseScheduledActionToClear != wordExerciseScheduledAction ? wordExerciseScheduledActionToClear : undefined));
    }, delayMilliseconds);
  }, []);

  const wordExerciseResultsElementId = "word-exercise-results";

  useEffect(() => {
    if (!wordExerciseSession || !wordExerciseResults) {
      return;
    }

    const {
      completed,
      submitted,
      wordExerciseBatchModel: { items: wordExerciseModels },
      generateWordExerciseBatchResponseModel,
      peekedIndexes,
    } = wordExerciseSession;

    const { wordsTotal, wordsCompleted } = wordExerciseResults;

    if (!completed && !wordExerciseScheduledAction && wordsTotal === wordsCompleted) {
      const setCompleted = () => {
        setWordExerciseSession(() => ({ ...wordExerciseSession, completed: true }));
      };

      setCompleted();
    }

    if (!submitted && completed) {
      const setSubmitted = () => {
        setWordExerciseSession(() => ({ ...wordExerciseSession, submitted: true }));
      };

      const failedWordExerciseIds = new Set(wordExerciseModels.filter((_wordExerciseModel, index) => peekedIndexes.has(index)).map((wordExerciseModel) => wordExerciseModel.id));

      const completedWordExerciseModels = wordExerciseModels.filter((wordExerciseModel) => !failedWordExerciseIds.has(wordExerciseModel.id));
      const failedWordExerciseModel = wordExerciseModels.filter((wordExerciseModel) => failedWordExerciseIds.has(wordExerciseModel.id));

      const onExerciseDetailsSubmitFinish = () => {
        window.location.href = `#${wordExerciseResultsElementId}`;
      };
      const onExerciseDetailsSubmitFinishFailed = () => {
        message.error("One or multiple requests submitting word exercise results have failed. Your progress could not be saved.");
      };

      const submitCompletedWordExerciseDetailsRequestModel: SubmitCompletedWordExerciseDetailsRequestModel = {
        completedItems: completedWordExerciseModels,
        originalResponse: generateWordExerciseBatchResponseModel,
      };
      const submitFailedWordExerciseDetailsRequestModel: SubmitFailedWordExerciseDetailsRequestModel = {
        failedItems: failedWordExerciseModel,
        originalResponse: generateWordExerciseBatchResponseModel,
      };

      Promise.all([submitCompletedWordExerciseDetails(submitCompletedWordExerciseDetailsRequestModel), submitFailedWordExerciseDetails(submitFailedWordExerciseDetailsRequestModel)])
        .then(() => {
          setSubmitted();
          onExerciseDetailsSubmitFinish();
        })
        .catch((error: unknown) => {
          displayUnsuccessfulRequestError(error, logApiError);
          onExerciseDetailsSubmitFinishFailed();
        });
    }
  }, [wordExerciseSession, wordExerciseResults, wordExerciseScheduledAction]);

  const onBackToTopButtonClick = useCallback(() => {
    window.scrollTo({ top: 0 });
  }, []);

  return (
    <>
      <Title level={4}>Word Exercise</Title>
      <Space style={{ display: "flex" }} direction="vertical">
        <Card
          size="small"
          title={
            <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
              <Space wrap direction="horizontal" align="baseline">
                Configuration
              </Space>
              {!mobileBrowserMode ? exerciseGenerationControlsJsx : undefined}
            </Space>
          }
        >
          <Form
            form={wordExerciseConfigurationForm}
            initialValues={initialWordExerciseConfigurationFormObject}
            onFinish={onWordExerciseConfigurationFormFinish}
            onFinishFailed={onWordExerciseConfigurationFormFinishFailed}
            size="small"
            labelCol={{ span: 8 }}
            wrapperCol={{ span: 16 }}
            className={mobileBrowserMode ? styles.formMobile : styles.form}
          >
            <Form.Item name={keyOf<WordExerciseConfigurationFormObject>("mode")} label="Mode" rules={[{ required: true }]}>
              <Select placeholder="Select a mode">
                <Select.Option value={valueOf<WordExerciseMode>("characterTypes")}>Character Types</Select.Option>
                <Select.Option value={valueOf<WordExerciseMode>("characterTypes-pronunciation")}>Character Types & Pronunciation</Select.Option>
                <Select.Option value={valueOf<WordExerciseMode>("characters")}>Characters</Select.Option>
                <Select.Option value={valueOf<WordExerciseMode>("characters-pronunciation")}>Characters & Pronunciation</Select.Option>
                <Select.Option value={valueOf<WordExerciseMode>("fullDescription")}>Full Description</Select.Option>
              </Select>
            </Form.Item>
            <Form.Item name={keyOf<WordExerciseConfigurationFormObject>("useWordExerciseProfileId")} label="Use Exercise Profile">
              <Select placeholder="Select an exercise profile" showSearch={false} allowClear>
                {(wordExercisePreferences?.exerciseProfiles ?? []).map((exerciseProfile) => (
                  <Select.Option key={exerciseProfile.id} value={exerciseProfile.id}>
                    {exerciseProfile.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item name={keyOf<WordExerciseConfigurationFormObject>("useWordGroupIds")} label="Use Groups">
              <Select placeholder="Select groups" mode="multiple" showSearch={false} allowClear>
                {wordGroups.map((wordGroup) => (
                  <Select.Option key={wordGroup.id} value={wordGroup.id}>
                    {wordGroup.caption}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item name={keyOf<WordExerciseConfigurationFormObject>("size")} label="Size" rules={[{ required: true }]}>
              <ExerciseSizeInputNumber />
            </Form.Item>
          </Form>
          {mobileBrowserMode ? exerciseGenerationControlsJsx : undefined}
        </Card>
        {wordExerciseSession && wordExerciseConfiguration ? (
          mobileBrowserMode || wordExerciseViewMode == "cards-view" ? (
            <WordExerciseCardSetView
              wordRecords={wordRecords}
              wordExerciseSession={wordExerciseSession}
              wordExerciseConfiguration={wordExerciseConfiguration}
              wordExercisePreferences={wordExercisePreferences}
              onWordExercisePeek={onWordExercisePeek}
              onWordExercisePeekRevert={onWordExercisePeekRevert}
              onWordExerciseDisplay={onWordExerciseDisplay}
              onWordExerciseDisplayRevert={onWordExerciseDisplayRevert}
              onWordExerciseComplete={onWordExerciseComplete}
              onWordExerciseCompleteRevert={onWordExerciseCompleteRevert}
              runWordExerciseScheduledAction={runWordExerciseScheduledAction}
            />
          ) : (
            <WordExerciseTableView
              wordRecords={wordRecords}
              wordExerciseSession={wordExerciseSession}
              wordExerciseConfiguration={wordExerciseConfiguration}
              wordExercisePreferences={wordExercisePreferences}
              onWordExercisePeek={onWordExercisePeek}
              onWordExercisePeekRevert={onWordExercisePeekRevert}
              onWordExerciseDisplay={onWordExerciseDisplay}
              onWordExerciseDisplayRevert={onWordExerciseDisplayRevert}
              onWordExerciseComplete={onWordExerciseComplete}
              onWordExerciseCompleteRevert={onWordExerciseCompleteRevert}
              runWordExerciseScheduledAction={runWordExerciseScheduledAction}
            />
          )
        ) : undefined}
        {wordExerciseSession ? (
          <Card
            id={wordExerciseResultsElementId}
            size="small"
            title={
              <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
                <Space wrap direction="horizontal" align="baseline">
                  Results
                </Space>
                {!mobileBrowserMode ? (
                  <Space wrap direction="horizontal" align="baseline">
                    <Button type="primary" size="small" icon={<ArrowUpOutlined />} onClick={onBackToTopButtonClick}>
                      Back To Top
                    </Button>
                  </Space>
                ) : undefined}
              </Space>
            }
          >
            {wordExerciseSession.submitted ? (
              <Paragraph>
                <Alert message="Exercise results have been successfully submitted to the database." type="success" showIcon closable />
              </Paragraph>
            ) : undefined}
            {wordExerciseResults ? (
              <>
                <Paragraph>
                  Words completed:{" "}
                  <Text style={{ display: "inline-block" }}>
                    {wordExerciseResults.wordsCompleted} of {wordExerciseResults.wordsTotal} ({((100 * wordExerciseResults.wordsCompleted) / wordExerciseResults.wordsTotalDivisionSafe).toFixed(2)} %)
                  </Text>
                  .
                </Paragraph>
                <Paragraph>
                  Words failed:{" "}
                  <Text style={{ display: "inline-block" }}>
                    {wordExerciseResults.wordsPeeked} of {wordExerciseResults.wordsTotal} ({((100 * wordExerciseResults.wordsPeeked) / wordExerciseResults.wordsTotalDivisionSafe).toFixed(2)} %)
                  </Text>
                  .
                </Paragraph>
              </>
            ) : undefined}
          </Card>
        ) : undefined}
      </Space>
    </>
  );
};

export default WordExercisePage;

const WordExerciseCardSetView = (props: WordExerciseViewProps) => {
  const {
    wordRecords,
    wordExerciseSession: {
      wordExerciseBatchModel: { id: wordExerciseBatchId },
      completed: wordExerciseSessionCompleted,
    },
    wordExercisePreferences,
  } = props;

  const { onWordExercisePeek, onWordExercisePeekRevert, onWordExerciseDisplay, onWordExerciseComplete, runWordExerciseScheduledAction } = props;

  const [currentWordRecordIndex, setCurrentWordRecordIndex] = useState<number>(0);

  const availablePendingWordRecordIndex = useMemo(() => {
    return wordRecords.findIndex((wordRecord) => !wordRecord.completed);
  }, [wordRecords]);

  useEffect(() => setCurrentWordRecordIndex(0), [wordExerciseBatchId]);

  const currentWordRecord = useMemo(() => {
    if (wordRecords.length > 0) {
      return wordRecords[currentWordRecordIndex];
    } else {
      return undefined;
    }
  }, [wordRecords, currentWordRecordIndex]);

  const isFirstWordRecord = useMemo(() => currentWordRecordIndex == 0, [currentWordRecordIndex]);
  const isLastWordRecord = useMemo(() => currentWordRecordIndex == wordRecords.length - 1, [currentWordRecordIndex, wordRecords]);
  const isPendingWordRecordAvailable = useMemo(() => availablePendingWordRecordIndex >= 0, [availablePendingWordRecordIndex]);

  const moveToNextWordRecord = () => setCurrentWordRecordIndex((currentWordRecordIndex) => currentWordRecordIndex + 1);
  const moveToPreviousWordRecord = () => setCurrentWordRecordIndex((currentWordRecordIndex) => currentWordRecordIndex - 1);
  const moveToAvailablePendingWordRecord = useCallback(() => setCurrentWordRecordIndex(availablePendingWordRecordIndex), [availablePendingWordRecordIndex]);

  const [completeButtonGracePeriodActive, setCompleteButtonGracePeriodActive] = useState<boolean>(false);

  const completeButtonIcon = useMemo(() => {
    if (!currentWordRecord) {
      return undefined;
    } else if (completeButtonGracePeriodActive) {
      return <ClockCircleOutlined />;
    } else {
      return <CheckOutlined />;
    }
  }, [currentWordRecord, completeButtonGracePeriodActive]);
  const completeButtonContent = useMemo(() => {
    if (!currentWordRecord) {
      return undefined;
    } else {
      return "Complete";
    }
  }, [currentWordRecord]);

  const onCompleteButtonClick = useCallback(
    (wordRecordIndex: number) => {
      onWordExerciseDisplay(wordRecordIndex);

      if (wordExercisePreferences?.navigateOnCompletion) {
        setCompleteButtonGracePeriodActive(true);

        runWordExerciseScheduledAction(() => {
          onWordExerciseComplete(wordRecordIndex);
          if (wordRecordIndex + 1 < wordRecords.length) {
            moveToNextWordRecord();
          }

          setCompleteButtonGracePeriodActive(false);
        }, wordExercisePreferences.navigateOnCompletionDelayMilliseconds ?? wordExercisePreferencesDefaultValues.navigateOnCompletionDelayMilliseconds);
      } else {
        onWordExerciseComplete(wordRecordIndex);
      }
    },
    [wordRecords, wordExercisePreferences, onWordExerciseDisplay, onWordExerciseComplete, runWordExerciseScheduledAction]
  );

  const [failButtonGracePeriodActive, setFailButtonGracePeriodActive] = useState<boolean>(false);

  const failButtonIcon = useMemo(() => {
    if (!currentWordRecord) {
      return undefined;
    } else if (failButtonGracePeriodActive) {
      return <ClockCircleOutlined />;
    } else if (completeButtonGracePeriodActive || currentWordRecord.completed) {
      return !currentWordRecord.peeked ? <CloseOutlined /> : <UndoOutlined />;
    } else {
      return !currentWordRecord.peeked ? <QuestionOutlined /> : <UndoOutlined />;
    }
  }, [currentWordRecord, completeButtonGracePeriodActive, failButtonGracePeriodActive]);
  const failButtonContent = useMemo(() => {
    if (!currentWordRecord) {
      return undefined;
    } else if (completeButtonGracePeriodActive || currentWordRecord.completed) {
      return !currentWordRecord.peeked ? "Fail" : "Revert Fail";
    } else {
      return !currentWordRecord.peeked ? "Peek" : "Revert Peek";
    }
  }, [currentWordRecord, completeButtonGracePeriodActive]);

  const onFailButtonClick = useCallback(
    (wordRecordIndex: number, completed: boolean, status: boolean) => {
      if (!completed && !completeButtonGracePeriodActive && status) {
        onWordExercisePeek(wordRecordIndex);
        onWordExerciseDisplay(wordRecordIndex);

        if (wordExercisePreferences?.navigateOnFailure) {
          setFailButtonGracePeriodActive(true);

          runWordExerciseScheduledAction(() => {
            onWordExerciseComplete(wordRecordIndex);
            if (wordRecordIndex + 1 < wordRecords.length) {
              moveToNextWordRecord();
            }

            setFailButtonGracePeriodActive(false);
          }, wordExercisePreferences.navigateOnFailureDelayMilliseconds ?? wordExercisePreferencesDefaultValues.navigateOnFailureDelayMilliseconds);
        } else {
          onWordExerciseComplete(wordRecordIndex);
        }
      } else if ((completed || completeButtonGracePeriodActive) && status) {
        onWordExercisePeek(wordRecordIndex);
      } else {
        onWordExercisePeekRevert(wordRecordIndex);
      }
    },
    [wordRecords, wordExercisePreferences, onWordExercisePeek, onWordExercisePeekRevert, onWordExerciseDisplay, onWordExerciseComplete, runWordExerciseScheduledAction, completeButtonGracePeriodActive]
  );

  return (
    <Space style={{ display: "flex" }} direction="vertical">
      <Card
        size="small"
        title={
          <Space wrap className={styles.cardHeader} direction="horizontal" align="baseline">
            <Space wrap direction="horizontal" align="baseline">
              Content
            </Space>
          </Space>
        }
      >
        {currentWordRecord ? (
          <Space direction="vertical">
            <Paragraph>
              Characters:{" "}
              <Text keyboard style={{ display: "inline-block" }}>
                {currentWordRecord.characters}
              </Text>
            </Paragraph>
            <Paragraph>
              Character Types:{" "}
              <Text keyboard style={{ display: "inline-block" }}>
                {currentWordRecord.characterTypes}
              </Text>
            </Paragraph>
            <Paragraph>
              Pronunciation:{" "}
              <Text keyboard style={{ display: "inline-block" }}>
                {currentWordRecord.pronunciation ?? <Typography.Text type="secondary">N/A</Typography.Text>}
              </Text>
            </Paragraph>
            <Paragraph>
              Furigana:{" "}
              <Text keyboard style={{ display: "inline-block" }}>
                {currentWordRecord.furigana ?? <Typography.Text type="secondary">N/A</Typography.Text>}
              </Text>
            </Paragraph>
            <Paragraph>
              Meaning:{" "}
              <Text keyboard style={{ display: "inline-block" }}>
                {currentWordRecord.meaning ?? <Typography.Text type="secondary">N/A</Typography.Text>}
              </Text>
            </Paragraph>
          </Space>
        ) : (
          <Empty />
        )}
      </Card>
      {currentWordRecord ? (
        <Card size="small" title="Controls">
          <Space wrap direction="horizontal" align="baseline">
            <Button
              type="default"
              size="small"
              icon={completeButtonIcon}
              disabled={wordExerciseSessionCompleted || completeButtonGracePeriodActive || failButtonGracePeriodActive || currentWordRecord.completed}
              onClick={() => onCompleteButtonClick(currentWordRecordIndex)}
            >
              {completeButtonContent}
            </Button>
            <Button
              type={!currentWordRecord.peeked ? "default" : "dashed"}
              size="small"
              icon={failButtonIcon}
              disabled={wordExerciseSessionCompleted || failButtonGracePeriodActive}
              onClick={() => onFailButtonClick(currentWordRecordIndex, currentWordRecord.completed, !currentWordRecord.peeked)}
              danger
            >
              {failButtonContent}
            </Button>
          </Space>
          <Divider style={{ margin: "12px 0px" }} />
          <Space wrap direction="horizontal" align="baseline">
            <Button size="small" icon={<LeftOutlined />} disabled={isFirstWordRecord} onClick={moveToPreviousWordRecord} />
            <Text style={{ fontWeight: "initial" }}>
              {1 + currentWordRecordIndex} / {wordRecords.length}
            </Text>
            <Button size="small" icon={<RightOutlined />} disabled={isLastWordRecord} onClick={moveToNextWordRecord} />
            <Button size="small" icon={<ArrowRightOutlined />} disabled={!isPendingWordRecordAvailable} onClick={moveToAvailablePendingWordRecord} />
          </Space>
        </Card>
      ) : undefined}
    </Space>
  );
};

const WordExerciseTableView = (props: WordExerciseViewProps) => {
  const {
    wordRecords,
    wordExerciseSession: {
      wordExerciseBatchModel: { id: wordExerciseBatchId, items: wordExerciseModels },
      completed: wordExerciseSessionCompleted,
    },
    wordExercisePreferences,
  } = props;

  const { onWordExercisePeek, onWordExercisePeekRevert, onWordExerciseDisplay, onWordExerciseComplete, runWordExerciseScheduledAction } = props;

  const [completeButtonGracePeriodActiveFlags, setCompleteButtonGracePeriodActiveFlags] = useState<boolean[]>([]);

  useEffect(() => setCompleteButtonGracePeriodActiveFlags(new Array(wordExerciseModels.length).map(() => false)), [wordExerciseModels, wordExerciseBatchId]);

  const setCompleteButtonGracePeriodActive = (wordRecordIndex: number, flag: boolean) => {
    setCompleteButtonGracePeriodActiveFlags((completeButtonGracePeriodActiveFlags) => {
      completeButtonGracePeriodActiveFlags[wordRecordIndex] = flag;
      return [...completeButtonGracePeriodActiveFlags];
    });
  };

  const onCompleteButtonClick = useCallback(
    (wordRecordIndex: number) => {
      onWordExerciseDisplay(wordRecordIndex);

      if (wordExercisePreferences?.navigateOnCompletion) {
        setCompleteButtonGracePeriodActive(wordRecordIndex, true);

        runWordExerciseScheduledAction(() => {
          onWordExerciseComplete(wordRecordIndex);

          setCompleteButtonGracePeriodActive(wordRecordIndex, false);
        }, wordExercisePreferences.navigateOnCompletionDelayMilliseconds ?? wordExercisePreferencesDefaultValues.navigateOnCompletionDelayMilliseconds);
      } else {
        onWordExerciseComplete(wordRecordIndex);
      }
    },
    [wordExercisePreferences, onWordExerciseDisplay, onWordExerciseComplete, runWordExerciseScheduledAction]
  );

  const [failButtonGracePeriodActiveFlags, setFailButtonGracePeriodActiveFlags] = useState<boolean[]>([]);

  useEffect(() => setFailButtonGracePeriodActiveFlags(new Array(wordExerciseModels.length).map(() => false)), [wordExerciseModels, wordExerciseBatchId]);

  const setFailButtonGracePeriodActive = (wordRecordIndex: number, flag: boolean) => {
    setFailButtonGracePeriodActiveFlags((failButtonGracePeriodActiveFlags) => {
      failButtonGracePeriodActiveFlags[wordRecordIndex] = flag;
      return [...failButtonGracePeriodActiveFlags];
    });
  };

  const onFailButtonClick = useCallback(
    (wordRecordIndex: number, completed: boolean, status: boolean) => {
      if (!completed && !completeButtonGracePeriodActiveFlags[wordRecordIndex] && status) {
        onWordExercisePeek(wordRecordIndex);
        onWordExerciseDisplay(wordRecordIndex);

        if (wordExercisePreferences?.navigateOnFailure) {
          setFailButtonGracePeriodActive(wordRecordIndex, true);

          runWordExerciseScheduledAction(() => {
            onWordExerciseComplete(wordRecordIndex);

            setFailButtonGracePeriodActive(wordRecordIndex, false);
          }, wordExercisePreferences.navigateOnFailureDelayMilliseconds ?? wordExercisePreferencesDefaultValues.navigateOnFailureDelayMilliseconds);
        } else {
          onWordExerciseComplete(wordRecordIndex);
        }
      } else if ((completed || completeButtonGracePeriodActiveFlags[wordRecordIndex]) && status) {
        onWordExercisePeek(wordRecordIndex);
      } else {
        onWordExercisePeekRevert(wordRecordIndex);
      }
    },
    [wordExercisePreferences, onWordExercisePeek, onWordExercisePeekRevert, onWordExerciseDisplay, onWordExerciseComplete, runWordExerciseScheduledAction, completeButtonGracePeriodActiveFlags]
  );

  const columns = useMemo(
    () => [
      {
        key: keyOf<WordRecord>("index"),
        dataIndex: keyOf<WordRecord>("index"),
        title: "#",
        render: (index: number) => 1 + index,
      },
      {
        key: keyOf<WordRecord>("characters"),
        dataIndex: keyOf<WordRecord>("characters"),
        title: "Characters",
      },
      {
        key: keyOf<WordRecord>("characterTypes"),
        dataIndex: keyOf<WordRecord>("characterTypes"),
        title: "Character Types",
      },
      {
        key: keyOf<WordRecord>("pronunciation"),
        dataIndex: keyOf<WordRecord>("pronunciation"),
        title: "Pronunciation",
      },
      {
        key: keyOf<WordRecord>("furigana"),
        dataIndex: keyOf<WordRecord>("furigana"),
        title: "Furigana",
        render: (furigana?: string) => furigana ?? <Typography.Text type="secondary">N/A</Typography.Text>,
      },
      {
        key: keyOf<WordRecord>("meaning"),
        dataIndex: keyOf<WordRecord>("meaning"),
        title: "Meaning",
      },
      {
        key: "action",
        title: "Action",
        width: "244px", // 8 + 100 + 8 + 120 + 8 px
        render: (_: object, wordRecord: WordRecord, wordRecordIndex: number) => {
          let completeButtonIcon;
          if (completeButtonGracePeriodActiveFlags[wordRecordIndex]) {
            completeButtonIcon = <ClockCircleOutlined />;
          } else {
            completeButtonIcon = <CheckOutlined />;
          }
          let completeButtonContent;
          if (completeButtonGracePeriodActiveFlags[wordRecordIndex]) {
            completeButtonContent = "Pending";
          } else {
            completeButtonContent = "Complete";
          }

          let failButtonIcon;
          if (failButtonGracePeriodActiveFlags[wordRecordIndex]) {
            failButtonIcon = <ClockCircleOutlined />;
          } else if (completeButtonGracePeriodActiveFlags[wordRecordIndex] || wordRecord.completed) {
            failButtonIcon = !wordRecord.peeked ? <CloseOutlined /> : <UndoOutlined />;
          } else {
            failButtonIcon = !wordRecord.peeked ? <QuestionOutlined /> : <UndoOutlined />;
          }
          let failButtonContent;
          if (completeButtonGracePeriodActiveFlags[wordRecordIndex] || wordRecord.completed) {
            failButtonContent = !wordRecord.peeked ? "Fail" : "Revert Fail";
          } else {
            failButtonContent = !wordRecord.peeked ? "Peek" : "Revert Peek";
          }

          return (
            <Space wrap direction="horizontal" align="baseline">
              <Button
                type="default"
                size="small"
                style={{ maxWidth: "100px" }}
                icon={completeButtonIcon}
                disabled={wordExerciseSessionCompleted || completeButtonGracePeriodActiveFlags[wordRecordIndex] || failButtonGracePeriodActiveFlags[wordRecordIndex] || wordRecord.completed}
                onClick={() => onCompleteButtonClick(wordRecordIndex)}
              >
                {completeButtonContent}
              </Button>
              <Button
                type={!wordRecord.peeked ? "default" : "dashed"}
                size="small"
                icon={failButtonIcon}
                style={{ maxWidth: "120px" }}
                disabled={wordExerciseSessionCompleted || failButtonGracePeriodActiveFlags[wordRecordIndex]}
                onClick={() => onFailButtonClick(wordRecordIndex, wordRecord.completed, !wordRecord.peeked)}
                danger
              >
                {failButtonContent}
              </Button>
            </Space>
          );
        },
      },
    ],
    [wordExerciseSessionCompleted, completeButtonGracePeriodActiveFlags, failButtonGracePeriodActiveFlags, onCompleteButtonClick, onFailButtonClick]
  );

  return <Table dataSource={wordRecords} rowKey="index" columns={columns} pagination={false} size="small" />;
};
