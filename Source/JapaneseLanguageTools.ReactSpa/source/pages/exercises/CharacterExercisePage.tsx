import { Alert, Button, Card, Form, message, Radio, Select, Space, Typography } from "antd";
import { useCallback, useContext, useEffect, useMemo, useState } from "react";

import { ArrowUpOutlined, LoadingOutlined, MobileOutlined, ProfileOutlined, TableOutlined, UndoOutlined } from "@ant-design/icons";

import logApiError from "@/api/helpers/logApiError.antd";
import { displayUnsuccessfulRequestError } from "@/api/helpers/unsuccessfulRequestHelpers";
import { generateCharacterExerciseBatch, getCharacterExerciseBatch, submitCompletedCharacterExerciseDetails, submitFailedCharacterExerciseDetails } from "@/api/applicationDictionaryExerciseMethods";
import { getAllCharacterGroups } from "@/api/applicationDictionaryMethods";
import { characterExercisePreferencesDefaultValues } from "@/constants/ApplicationPreferencesConstants";
import ExerciseSizeInputNumber from "@/components/ExerciseSizeInputNumber";
import ApplicationPreferencesContext from "@/contexts/ApplicationPreferencesContext";
import MobileBrowserContext from "@/contexts/MobileBrowserContext";
import CharacterExercisePreferences from "@/entities/preferences/CharacterExercisePreferences";
import CharacterExerciseProfile from "@/entities/preferences/CharacterExerciseProfile";
import CharacterExerciseAlphabetMode from "@/enumerations/types/CharacterExerciseAlphabetMode";
import CharacterExerciseMode from "@/enumerations/types/CharacterExerciseMode";
import CharacterProperties from "@/enumerations/CharacterProperties";
import CharacterTypes, { CharacterTypesUtils } from "@/enumerations/CharacterTypes";
import CharacterExerciseBatchModel from "@/models/CharacterExerciseBatch.Model";
import CharacterGroupModel from "@/models/CharacterGroup.Model";
import GenerateCharacterExerciseBatchRequestModel from "@/models/requests/GenerateCharacterExerciseBatchRequest.Model";
import GetCharacterExerciseBatchRequestModel from "@/models/requests/GetCharacterExerciseBatchRequest.Model";
import SubmitCompletedCharacterExerciseDetailsRequestModel from "@/models/requests/SubmitCompletedCharacterExerciseDetailsRequest.Model";
import SubmitFailedCharacterExerciseDetailsRequestModel from "@/models/requests/SubmitFailedCharacterExerciseDetailsRequest.Model";
import GenerateCharacterExerciseBatchResponseModel from "@/models/responses/GenerateCharacterExerciseBatchResponse.Model";
import keyOf from "@/typescript/keyOf";
import valueOf from "@/typescript/valueOf";

import styles from "./CharacterExercisePage.module.css";

const { Text, Title, Paragraph } = Typography;

interface CharacterExerciseConfiguration {
  mode: CharacterExerciseMode;
  alphabetMode: CharacterExerciseAlphabetMode;
  useCharacterGroupIds: number[];
  useCharacterExerciseProfile?: CharacterExerciseProfile;
  size: number;
}

interface CharacterExerciseSession {
  startedOn: Date;
  characterProperties: CharacterProperties;
  characterExerciseBatchModel: CharacterExerciseBatchModel;
  generateCharacterExerciseBatchResponseModel: GenerateCharacterExerciseBatchResponseModel;
  peekedIndexes: Set<number>;
  displayedIndexes: Set<number>;
  completedIndexes: Set<number>;
  completed: boolean;
  submitted: boolean;
}

interface CharacterRecord {
  id: number;
  characterExerciseId: number;
  index: number;
  symbol: string;
  type: string;
  pronunciation?: string;
  syllable?: string;
  onyomi?: string;
  kunyomi?: string;
  meaning?: string;
  peeked: boolean;
  displayed: boolean;
  completed: boolean;
}

type CharacterExerciseScheduledAction = () => void;

interface CharacterExerciseViewProps {
  characterRecords: CharacterRecord[];
  characterExerciseSession: CharacterExerciseSession;
  characterExerciseConfiguration: CharacterExerciseConfiguration;
  characterExercisePreferences?: CharacterExercisePreferences;
  onCharacterExercisePeek: (characterRecordIndex: number) => void;
  onCharacterExercisePeekRevert: (characterRecordIndex: number) => void;
  onCharacterExerciseDisplay: (characterRecordIndex: number) => void;
  onCharacterExerciseDisplayRevert: (characterRecordIndex: number) => void;
  onCharacterExerciseComplete: (characterRecordIndex: number) => void;
  onCharacterExerciseCompleteRevert: (characterRecordIndex: number) => void;
  runCharacterExerciseScheduledAction: (characterExerciseScheduledAction: CharacterExerciseScheduledAction, delayMilliseconds: number) => void;
}

const CharacterExercisePage = () => {
  const mobileBrowserMode = useContext(MobileBrowserContext);

  const {
    applicationPreferences: { characterExercisePreferences },
  } = useContext(ApplicationPreferencesContext);

  const [characterGroups, setCharacterGroups] = useState<CharacterGroupModel[]>([]);

  useEffect(() => {
    getAllCharacterGroups()
      .then((characterGroups) => setCharacterGroups(characterGroups.filter((characterGroup) => characterGroup.enabled && !characterGroup.hidden)))
      .catch((error: unknown) => displayUnsuccessfulRequestError(error, logApiError));
  }, []);

  const [characterExerciseConfiguration, setCharacterExerciseConfiguration] = useState<CharacterExerciseConfiguration | undefined>(undefined);

  interface CharacterExerciseConfigurationFormObject extends Omit<CharacterExerciseConfiguration, "useCharacterExerciseProfile"> {
    useCharacterExerciseProfileId?: string;
  }

  const initialCharacterExerciseConfigurationFormObject = useMemo<CharacterExerciseConfigurationFormObject>(
    () => ({
      mode: characterExercisePreferences?.defaultExerciseMode ?? characterExercisePreferencesDefaultValues.defaultExerciseMode,
      alphabetMode: characterExercisePreferences?.defaultExerciseAlphabetMode ?? characterExercisePreferencesDefaultValues.defaultExerciseAlphabetMode,
      useCharacterExerciseProfileId: characterExercisePreferences?.defaultExerciseProfileId ?? characterExercisePreferencesDefaultValues.defaultExerciseProfileId,
      useCharacterGroupIds: characterExercisePreferences?.defaultCharacterGroupIds ?? characterExercisePreferencesDefaultValues.defaultCharacterGroupIds,
      size: characterExercisePreferences?.defaultExerciseSize ?? characterExercisePreferencesDefaultValues.defaultExerciseSize,
    }),
    [characterExercisePreferences]
  );

  const [characterExerciseConfigurationForm] = Form.useForm<CharacterExerciseConfigurationFormObject>();

  useEffect(() => characterExerciseConfigurationForm.resetFields(), [characterExerciseConfigurationForm, initialCharacterExerciseConfigurationFormObject]);

  const [characterExerciseSession, setCharacterExerciseSession] = useState<CharacterExerciseSession | undefined>(undefined);
  const [characterExerciseSessionLoading, setCharacterExerciseSessionLoading] = useState<boolean>(false);

  const characterRecords = useMemo(() => {
    const characterRecords: CharacterRecord[] = [];

    if (characterExerciseSession == undefined || characterExerciseConfiguration == undefined) {
      return characterRecords;
    }

    const {
      characterExerciseBatchModel: { items: characterExerciseModels },
      peekedIndexes,
      displayedIndexes,
      completedIndexes,
    } = characterExerciseSession;

    const { mode } = characterExerciseConfiguration;

    const getHiddenCharacterProperties = (mode: CharacterExerciseMode) => {
      if (mode === "type") {
        return CharacterProperties.Type;
      } else if (mode === "type-pronunciation") {
        return CharacterProperties.Type | CharacterProperties.Onyomi | CharacterProperties.Kunyomi | CharacterProperties.Pronunciation;
      } else if (mode === "character") {
        return CharacterProperties.Symbol;
      } else if (mode === "character-pronunciation") {
        return CharacterProperties.Symbol | CharacterProperties.Onyomi | CharacterProperties.Kunyomi | CharacterProperties.Pronunciation;
      } else {
        /* if (mode === "fullDescription") */
        return CharacterProperties.Type | CharacterProperties.Onyomi | CharacterProperties.Kunyomi | CharacterProperties.Pronunciation | CharacterProperties.Syllable | CharacterProperties.Meaning;
      }
    };

    const hiddenCharacterProperties = getHiddenCharacterProperties(mode);

    const hiddenCharacterPropertyPlaceholder = "???";

    for (const [index, { id: characterExerciseId, character }] of characterExerciseModels.entries()) {
      if (character == undefined) {
        continue;
      }

      const characterRecord: CharacterRecord = {
        id: character.id,
        characterExerciseId: characterExerciseId,
        index: index,
        symbol: character.symbol,
        type: CharacterTypesUtils.characterTypesToString(character.type),
        pronunciation: character.pronunciation,
        syllable: character.syllable,
        onyomi: character.onyomi,
        kunyomi: character.kunyomi,
        meaning: character.meaning,
        peeked: peekedIndexes.has(index),
        displayed: displayedIndexes.has(index),
        completed: completedIndexes.has(index),
      };

      if (hiddenCharacterProperties & CharacterProperties.Type && !(characterRecord.peeked || characterRecord.displayed)) {
        characterRecord.type = hiddenCharacterPropertyPlaceholder;
      }
      if (hiddenCharacterProperties & CharacterProperties.Symbol && !(characterRecord.peeked || characterRecord.displayed)) {
        characterRecord.symbol = hiddenCharacterPropertyPlaceholder;
      }
      if (hiddenCharacterProperties & CharacterProperties.Pronunciation && !(characterRecord.peeked || characterRecord.displayed)) {
        characterRecord.pronunciation = hiddenCharacterPropertyPlaceholder;
      }
      if (hiddenCharacterProperties & CharacterProperties.Onyomi && !(characterRecord.peeked || characterRecord.displayed)) {
        characterRecord.onyomi = hiddenCharacterPropertyPlaceholder;
      }
      if (hiddenCharacterProperties & CharacterProperties.Kunyomi && !(characterRecord.peeked || characterRecord.displayed)) {
        characterRecord.kunyomi = hiddenCharacterPropertyPlaceholder;
      }
      if (hiddenCharacterProperties & CharacterProperties.Syllable && !(characterRecord.peeked || characterRecord.displayed)) {
        characterRecord.syllable = hiddenCharacterPropertyPlaceholder;
      }
      if (hiddenCharacterProperties & CharacterProperties.Meaning && !(characterRecord.peeked || characterRecord.displayed)) {
        characterRecord.meaning = hiddenCharacterPropertyPlaceholder;
      }

      characterRecords.push(characterRecord);
    }

    return characterRecords;
  }, [characterExerciseSession, characterExerciseConfiguration]);

  interface CharacterExerciseResults {
    charactersTotal: number;
    charactersTotalDivisionSafe: number;
    charactersPeeked: number;
    charactersDisplayed: number;
    charactersCompleted: number;
  }

  const characterExerciseResults = useMemo(() => {
    if (characterExerciseSession == undefined) {
      return undefined;
    }

    const charactersTotal = characterExerciseSession.characterExerciseBatchModel.items.length;
    const charactersTotalDivisionSafe = charactersTotal > 0 ? charactersTotal : 1;
    const charactersPeeked = characterExerciseSession.peekedIndexes.size;
    const charactersDisplayed = characterExerciseSession.displayedIndexes.size;
    const charactersCompleted = characterExerciseSession.completedIndexes.size;

    const characterExerciseResults: CharacterExerciseResults = {
      charactersTotal,
      charactersTotalDivisionSafe,
      charactersPeeked,
      charactersDisplayed,
      charactersCompleted,
    };

    return characterExerciseResults;
  }, [characterExerciseSession]);

  const onGenerateButtonClick = useCallback(() => {
    characterExerciseConfigurationForm.submit();
  }, [characterExerciseConfigurationForm]);

  const onResetButtonClick = useCallback(() => {
    characterExerciseConfigurationForm.resetFields();

    setCharacterExerciseConfiguration(undefined);
    setCharacterExerciseSession(undefined);
  }, [characterExerciseConfigurationForm]);

  type CharacterExerciseViewMode = "table-view" | "cards-view";

  const [characterExerciseViewMode, setCharacterExerciseViewMode] = useState<CharacterExerciseViewMode>("table-view");

  const exerciseGenerationControlsJsx = useMemo(() => {
    return (
      <Space wrap direction="horizontal" align="baseline">
        <Button type="primary" size="small" icon={!characterExerciseSessionLoading ? <ProfileOutlined /> : <LoadingOutlined />} onClick={onGenerateButtonClick}>
          Generate
        </Button>
        <Button type="primary" size="small" icon={<UndoOutlined />} onClick={onResetButtonClick} danger>
          Reset
        </Button>
        {!mobileBrowserMode ? (
          <Radio.Group size="small" value={characterExerciseViewMode} onChange={(e) => setCharacterExerciseViewMode(e.target.value as CharacterExerciseViewMode)}>
            <Radio.Button value={valueOf<CharacterExerciseViewMode>("table-view")}>
              <TableOutlined /> Table View
            </Radio.Button>
            <Radio.Button value={valueOf<CharacterExerciseViewMode>("cards-view")}>
              <MobileOutlined /> Cards View
            </Radio.Button>
          </Radio.Group>
        ) : undefined}
      </Space>
    );
  }, [mobileBrowserMode, characterExerciseSessionLoading, onGenerateButtonClick, onResetButtonClick, characterExerciseViewMode]);

  const getCharacterExerciseProfile = useCallback(
    (characterExerciseProfileId?: string) => {
      if (!characterExerciseProfileId) {
        return undefined;
      }

      return (characterExercisePreferences?.exerciseProfiles ?? []).find((exerciseProfile) => exerciseProfile.id == characterExerciseProfileId);
    },
    [characterExercisePreferences]
  );

  const onCharacterExerciseConfigurationFormFinish = useCallback(
    (exerciseConfiguration: CharacterExerciseConfigurationFormObject) => {
      const { mode, alphabetMode, useCharacterGroupIds, useCharacterExerciseProfileId, size } = exerciseConfiguration;

      const useCharacterExerciseProfile = getCharacterExerciseProfile(useCharacterExerciseProfileId);

      const useCharacterTypes = alphabetMode == "kanji" ? CharacterTypes.Kanji : alphabetMode == "mixed" ? CharacterTypes.All : CharacterTypes.AllExceptKanji;

      setCharacterExerciseConfiguration({ mode, alphabetMode, useCharacterGroupIds, useCharacterExerciseProfile, size });

      setCharacterExerciseSessionLoading(true);

      const generateCharacterExerciseBatchRequestModel: GenerateCharacterExerciseBatchRequestModel = {
        size: size,
        useCharacterExerciseProfile: {
          repeatedChallengeCountProgression: useCharacterExerciseProfile?.repeatedChallengeCountProgression ?? [],
          tagDistributionPreferences: useCharacterExerciseProfile?.tagDistributionSettings,
        },
        useCharacterGroupIds: useCharacterGroupIds,
        useCharacterTypes: useCharacterTypes,
      };

      generateCharacterExerciseBatch(generateCharacterExerciseBatchRequestModel)
        .then((generateCharacterExerciseBatchResponseModel) => {
          const getCharacterExerciseBatchRequestModel: GetCharacterExerciseBatchRequestModel = {
            characterExerciseBatchId: generateCharacterExerciseBatchResponseModel.characterExerciseBatchId,
          };

          getCharacterExerciseBatch(getCharacterExerciseBatchRequestModel)
            .then((getCharacterExerciseBatchResponseModel) => {
              const { characterExerciseBatch: characterExerciseBatchModel } = getCharacterExerciseBatchResponseModel;

              const characterProperties =
                CharacterProperties.None |
                (alphabetMode == "mixed" || alphabetMode == "kanji" ? CharacterProperties.Onyomi | CharacterProperties.Kunyomi | CharacterProperties.Meaning : CharacterProperties.None) |
                (alphabetMode == "mixed" || alphabetMode == "kana" ? CharacterProperties.Pronunciation | CharacterProperties.Syllable : CharacterProperties.None);

              const characterExerciseSession: CharacterExerciseSession = {
                startedOn: new Date(),
                characterProperties: characterProperties,
                characterExerciseBatchModel: characterExerciseBatchModel,
                generateCharacterExerciseBatchResponseModel: generateCharacterExerciseBatchResponseModel,
                peekedIndexes: new Set<number>(),
                displayedIndexes: new Set<number>(),
                completedIndexes: new Set<number>(),
                completed: false,
                submitted: false,
              };

              setCharacterExerciseSession(characterExerciseSession);
              setCharacterExerciseSessionLoading(false);
            })
            .catch((error: unknown) => {
              displayUnsuccessfulRequestError(error, logApiError);
              setCharacterExerciseSessionLoading(false);
            });
        })
        .catch((error: unknown) => {
          displayUnsuccessfulRequestError(error, logApiError);
          setCharacterExerciseSessionLoading(false);
        });
    },
    [getCharacterExerciseProfile]
  );

  const onCharacterExerciseConfigurationFormFinishFailed = useCallback(() => {
    message.error("Form validation failed.");
  }, []);

  const setCharacterExercisePeeked = useCallback((index: number, peeked: boolean) => {
    const setPeekedIndexes = (updatePeekedIndexesAction: (peekedIndexes: Set<number>) => Set<number>) => {
      setCharacterExerciseSession((characterExerciseSession) => (characterExerciseSession ? { ...characterExerciseSession, peekedIndexes: updatePeekedIndexesAction(characterExerciseSession.peekedIndexes) } : undefined));
    };

    if (peeked) {
      setPeekedIndexes((peekedIndexes) => new Set<number>([...Array.from(peekedIndexes.values()), index]));
    } else {
      setPeekedIndexes((peekedIndexes) => new Set<number>(Array.from(peekedIndexes.values()).filter((indexToCompare) => index !== indexToCompare)));
    }
  }, []);

  const setCharacterExerciseDisplayed = useCallback((index: number, displayed: boolean) => {
    const setDisplayedIndexes = (updateDisplayedIndexesAction: (displayedIndexes: Set<number>) => Set<number>) => {
      setCharacterExerciseSession((characterExerciseSession) => (characterExerciseSession ? { ...characterExerciseSession, displayedIndexes: updateDisplayedIndexesAction(characterExerciseSession.displayedIndexes) } : undefined));
    };

    if (displayed) {
      setDisplayedIndexes((displayedIndexes) => new Set<number>([...Array.from(displayedIndexes.values()), index]));
    } else {
      setDisplayedIndexes((displayedIndexes) => new Set<number>(Array.from(displayedIndexes.values()).filter((indexToCompare) => index !== indexToCompare)));
    }
  }, []);

  const setCharacterExerciseCompleted = useCallback((index: number, completed: boolean) => {
    const setCompletedIndexes = (updateCompletedIndexesAction: (completedIndexes: Set<number>) => Set<number>) => {
      setCharacterExerciseSession((characterExerciseSession) => (characterExerciseSession ? { ...characterExerciseSession, completedIndexes: updateCompletedIndexesAction(characterExerciseSession.completedIndexes) } : undefined));
    };

    if (completed) {
      setCompletedIndexes((completedIndexes) => new Set<number>([...Array.from(completedIndexes.values()), index]));
    } else {
      setCompletedIndexes((completedIndexes) => new Set<number>(Array.from(completedIndexes.values()).filter((indexToCompare) => index !== indexToCompare)));
    }
  }, []);

  const onCharacterExercisePeek = useCallback(
    (characterRecordIndex: number) => {
      setCharacterExercisePeeked(characterRecordIndex, true);
    },
    [setCharacterExercisePeeked]
  );

  const onCharacterExercisePeekRevert = useCallback(
    (characterRecordIndex: number) => {
      setCharacterExercisePeeked(characterRecordIndex, false);
    },
    [setCharacterExercisePeeked]
  );

  const onCharacterExerciseDisplay = useCallback(
    (characterRecordIndex: number) => {
      setCharacterExerciseDisplayed(characterRecordIndex, true);
    },
    [setCharacterExerciseDisplayed]
  );

  const onCharacterExerciseDisplayRevert = useCallback(
    (characterRecordIndex: number) => {
      setCharacterExerciseDisplayed(characterRecordIndex, false);
    },
    [setCharacterExerciseDisplayed]
  );

  const onCharacterExerciseComplete = useCallback(
    (characterRecordIndex: number) => {
      setCharacterExerciseCompleted(characterRecordIndex, true);
    },
    [setCharacterExerciseCompleted]
  );

  const onCharacterExerciseCompleteRevert = useCallback(
    (characterRecordIndex: number) => {
      setCharacterExerciseCompleted(characterRecordIndex, false);
    },
    [setCharacterExerciseCompleted]
  );

  const [characterExerciseScheduledAction, setCharacterExerciseScheduledAction] = useState<CharacterExerciseScheduledAction | undefined>(undefined);

  const runCharacterExerciseScheduledAction = useCallback((characterExerciseScheduledAction: CharacterExerciseScheduledAction, delayMilliseconds: number) => {
    setCharacterExerciseScheduledAction(() => characterExerciseScheduledAction);
    setTimeout(() => {
      characterExerciseScheduledAction();
      setCharacterExerciseScheduledAction((characterExerciseScheduledActionToClear) => (characterExerciseScheduledActionToClear != characterExerciseScheduledAction ? characterExerciseScheduledActionToClear : undefined));
    }, delayMilliseconds);
  }, []);

  const characterExerciseResultsElementId = "character-exercise-results";

  useEffect(() => {
    if (!characterExerciseSession || !characterExerciseResults) {
      return;
    }

    const {
      completed,
      submitted,
      characterExerciseBatchModel: { items: characterExerciseModels },
      generateCharacterExerciseBatchResponseModel,
      peekedIndexes,
    } = characterExerciseSession;

    const { charactersTotal, charactersCompleted } = characterExerciseResults;

    if (!completed && !characterExerciseScheduledAction && charactersTotal === charactersCompleted) {
      const setCompleted = () => {
        setCharacterExerciseSession(() => ({ ...characterExerciseSession, completed: true }));
      };

      setCompleted();
    }

    if (!submitted && completed) {
      const setSubmitted = () => {
        setCharacterExerciseSession(() => ({ ...characterExerciseSession, submitted: true }));
      };

      const failedCharacterExerciseIds = new Set(characterExerciseModels.filter((_characterExerciseModel, index) => peekedIndexes.has(index)).map((characterExerciseModel) => characterExerciseModel.id));

      const completedCharacterExerciseModels = characterExerciseModels.filter((characterExerciseModel) => !failedCharacterExerciseIds.has(characterExerciseModel.id));
      const failedCharacterExerciseModel = characterExerciseModels.filter((characterExerciseModel) => failedCharacterExerciseIds.has(characterExerciseModel.id));

      const onExerciseDetailsSubmitFinish = () => {
        window.location.href = `#${characterExerciseResultsElementId}`;
      };
      const onExerciseDetailsSubmitFinishFailed = () => {
        message.error("One or multiple requests submitting character exercise results have failed. Your progress could not be saved.");
      };

      const submitCompletedCharacterExerciseDetailsRequestModel: SubmitCompletedCharacterExerciseDetailsRequestModel = {
        completedItems: completedCharacterExerciseModels,
        originalResponse: generateCharacterExerciseBatchResponseModel,
      };
      const submitFailedCharacterExerciseDetailsRequestModel: SubmitFailedCharacterExerciseDetailsRequestModel = {
        failedItems: failedCharacterExerciseModel,
        originalResponse: generateCharacterExerciseBatchResponseModel,
      };

      Promise.all([submitCompletedCharacterExerciseDetails(submitCompletedCharacterExerciseDetailsRequestModel), submitFailedCharacterExerciseDetails(submitFailedCharacterExerciseDetailsRequestModel)])
        .then(() => {
          setSubmitted();
          onExerciseDetailsSubmitFinish();
        })
        .catch((error: unknown) => {
          displayUnsuccessfulRequestError(error, logApiError);
          onExerciseDetailsSubmitFinishFailed();
        });
    }
  }, [characterExerciseSession, characterExerciseResults, characterExerciseScheduledAction]);

  const onBackToTopButtonClick = useCallback(() => {
    window.scrollTo({ top: 0 });
  }, []);

  return (
    <>
      <Title level={4}>Character Exercise</Title>
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
            form={characterExerciseConfigurationForm}
            initialValues={initialCharacterExerciseConfigurationFormObject}
            onFinish={onCharacterExerciseConfigurationFormFinish}
            onFinishFailed={onCharacterExerciseConfigurationFormFinishFailed}
            size="small"
            labelCol={{ span: 8 }}
            wrapperCol={{ span: 16 }}
            className={mobileBrowserMode ? styles.formMobile : styles.form}
          >
            <Form.Item name={keyOf<CharacterExerciseConfigurationFormObject>("mode")} label="Mode" rules={[{ required: true }]}>
              <Select placeholder="Select a mode">
                <Select.Option value={valueOf<CharacterExerciseMode>("type")}>Type</Select.Option>
                <Select.Option value={valueOf<CharacterExerciseMode>("type-pronunciation")}>Type & Pronunciation</Select.Option>
                <Select.Option value={valueOf<CharacterExerciseMode>("character")}>Character</Select.Option>
                <Select.Option value={valueOf<CharacterExerciseMode>("character-pronunciation")}>Character & Pronunciation</Select.Option>
                <Select.Option value={valueOf<CharacterExerciseMode>("fullDescription")}>Full Description</Select.Option>
              </Select>
            </Form.Item>
            <Form.Item name={keyOf<CharacterExerciseConfigurationFormObject>("alphabetMode")} label="Alphabet Mode" rules={[{ required: true }]}>
              <Radio.Group>
                <Radio.Button value={valueOf<CharacterExerciseAlphabetMode>("kana")}>Kana</Radio.Button>
                <Radio.Button value={valueOf<CharacterExerciseAlphabetMode>("kanji")}>Kanji</Radio.Button>
                <Radio.Button value={valueOf<CharacterExerciseAlphabetMode>("mixed")}>Mixed</Radio.Button>
              </Radio.Group>
            </Form.Item>
            <Form.Item name={keyOf<CharacterExerciseConfigurationFormObject>("useCharacterExerciseProfileId")} label="Use Exercise Profile">
              <Select placeholder="Select an exercise profile" showSearch={false} allowClear>
                {(characterExercisePreferences?.exerciseProfiles ?? []).map((exerciseProfile) => (
                  <Select.Option key={exerciseProfile.id} value={exerciseProfile.id}>
                    {exerciseProfile.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item name={keyOf<CharacterExerciseConfigurationFormObject>("useCharacterGroupIds")} label="Use Groups">
              <Select placeholder="Select groups" mode="multiple" showSearch={false} allowClear>
                {characterGroups.map((characterGroup) => (
                  <Select.Option key={characterGroup.id} value={characterGroup.id}>
                    {characterGroup.caption}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item name={keyOf<CharacterExerciseConfigurationFormObject>("size")} label="Size" rules={[{ required: true }]}>
              <ExerciseSizeInputNumber />
            </Form.Item>
          </Form>
          {mobileBrowserMode ? exerciseGenerationControlsJsx : undefined}
        </Card>
        {characterExerciseSession && characterExerciseConfiguration ? (
          mobileBrowserMode || characterExerciseViewMode == "cards-view" ? (
            <CharacterExerciseCardSetView
              characterRecords={characterRecords}
              characterExerciseSession={characterExerciseSession}
              characterExerciseConfiguration={characterExerciseConfiguration}
              characterExercisePreferences={characterExercisePreferences}
              onCharacterExercisePeek={onCharacterExercisePeek}
              onCharacterExercisePeekRevert={onCharacterExercisePeekRevert}
              onCharacterExerciseDisplay={onCharacterExerciseDisplay}
              onCharacterExerciseDisplayRevert={onCharacterExerciseDisplayRevert}
              onCharacterExerciseComplete={onCharacterExerciseComplete}
              onCharacterExerciseCompleteRevert={onCharacterExerciseCompleteRevert}
              runCharacterExerciseScheduledAction={runCharacterExerciseScheduledAction}
            />
          ) : (
            <CharacterExerciseTableView
              characterRecords={characterRecords}
              characterExerciseSession={characterExerciseSession}
              characterExerciseConfiguration={characterExerciseConfiguration}
              characterExercisePreferences={characterExercisePreferences}
              onCharacterExercisePeek={onCharacterExercisePeek}
              onCharacterExercisePeekRevert={onCharacterExercisePeekRevert}
              onCharacterExerciseDisplay={onCharacterExerciseDisplay}
              onCharacterExerciseDisplayRevert={onCharacterExerciseDisplayRevert}
              onCharacterExerciseComplete={onCharacterExerciseComplete}
              onCharacterExerciseCompleteRevert={onCharacterExerciseCompleteRevert}
              runCharacterExerciseScheduledAction={runCharacterExerciseScheduledAction}
            />
          )
        ) : undefined}
        {characterExerciseSession ? (
          <Card
            id={characterExerciseResultsElementId}
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
            {characterExerciseSession.submitted ? (
              <Paragraph>
                <Alert message="Exercise results have been successfully submitted to the database." type="success" showIcon closable />
              </Paragraph>
            ) : undefined}
            {characterExerciseResults ? (
              <>
                <Paragraph>
                  Characters completed:{" "}
                  <Text style={{ display: "inline-block" }}>
                    {characterExerciseResults.charactersCompleted} of {characterExerciseResults.charactersTotal} ({((100 * characterExerciseResults.charactersCompleted) / characterExerciseResults.charactersTotalDivisionSafe).toFixed(2)} %)
                  </Text>
                  .
                </Paragraph>
                <Paragraph>
                  Characters failed:{" "}
                  <Text style={{ display: "inline-block" }}>
                    {characterExerciseResults.charactersPeeked} of {characterExerciseResults.charactersTotal} ({((100 * characterExerciseResults.charactersPeeked) / characterExerciseResults.charactersTotalDivisionSafe).toFixed(2)} %)
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

export default CharacterExercisePage;

const CharacterExerciseCardSetView = (_props: CharacterExerciseViewProps) => {
  return <></>;
};

const CharacterExerciseTableView = (_props: CharacterExerciseViewProps) => {
  return <></>;
};
