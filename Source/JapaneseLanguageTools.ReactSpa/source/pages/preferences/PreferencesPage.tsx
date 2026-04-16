import { Alert, Button, Checkbox, Collapse, Divider, Form, Input, InputNumber, message, Radio, Select, Space, Typography } from "antd";
import { useCallback, useContext, useEffect, useMemo, useState } from "react";
import { v4 as uuidv4 } from "uuid";

import { ControlOutlined, DeleteOutlined, FormOutlined, PlusOutlined, ProfileOutlined, RedoOutlined, SaveOutlined, UndoOutlined } from "@ant-design/icons";

import logApiError from "@/api/helpers/logApiError.antd";
import { displayUnsuccessfulRequestError } from "@/api/helpers/unsuccessfulRequestHelpers";
import { getAllTags } from "@/api/tagMethods";
import DelayInputNumber from "@/components/DelayInputNumber";
import ExerciseSizeInputNumber from "@/components/ExerciseSizeInputNumber";
import * as ApplicationPreferencesConstants from "@/constants/ApplicationPreferencesConstants";
import * as ObjectConstants from "@/constants/ObjectConstants";
import ApplicationPreferencesContext from "@/contexts/ApplicationPreferencesContext";
import MobileBrowserContext from "@/contexts/MobileBrowserContext";
import TagDistributionRule from "@/entities/TagDistributionRule";
import TagDistributionSettings from "@/entities/TagDistributionSettings";
import ApplicationPreferences from "@/entities/preferences/ApplicationPreferences";
import CharacterExercisePreferences from "@/entities/preferences/CharacterExercisePreferences";
import CharacterExerciseProfile from "@/entities/preferences/CharacterExerciseProfile";
import WordExercisePreferences from "@/entities/preferences/WordExercisePreferences";
import WordExerciseProfile from "@/entities/preferences/WordExerciseProfile";
import CharacterExerciseAlphabetMode from "@/enumerations/types/CharacterExerciseAlphabetMode";
import CharacterExerciseMode from "@/enumerations/types/CharacterExerciseMode";
import WordExerciseMode from "@/enumerations/types/WordExerciseMode";
import keyOf from "@/typescript/keyOf";
import valueOf from "@/typescript/valueOf";

import styles from "./PreferencesPage.module.css";

const { Text, Title, Paragraph } = Typography;

const PreferencesPage = () => {
  const mobileBrowserMode = useContext(MobileBrowserContext);

  const { applicationPreferences, setApplicationPreferences } = useContext(ApplicationPreferencesContext);

  const [applicationPreferencesChanged, setApplicationPreferencesChanged] = useState<boolean>(false);
  const [applicationPreferencesImported, setApplicationPreferencesImported] = useState<boolean>(false);

  const [tagCaptions, setTagCaptions] = useState<string[]>([]);

  useEffect(() => {
    getAllTags()
      .then((tagModels) => {
        const tagCaptions = tagModels.map((tagModel) => tagModel.caption);
        setTagCaptions(tagCaptions);
      })
      .catch((error: unknown) => displayUnsuccessfulRequestError(error, logApiError));
  }, []);

  const characterExercisePreferences = useMemo(() => {
    return applicationPreferences.characterExercisePreferences ?? ApplicationPreferencesConstants.characterExercisePreferencesDefaultValues;
  }, [applicationPreferences]);
  const setCharacterExercisePreferences = useCallback(
    (updatedCharacterExercisePreferences: CharacterExercisePreferences) => {
      for (const characterExerciseProfile of updatedCharacterExercisePreferences.exerciseProfiles ?? []) {
        characterExerciseProfile.repeatedChallengeCountProgression = (characterExerciseProfile.repeatedChallengeCountProgression as number[] | undefined)?.length
          ? characterExerciseProfile.repeatedChallengeCountProgression.map((item) => Number(item))
          : [];
      }

      setApplicationPreferences({ ...applicationPreferences, characterExercisePreferences: updatedCharacterExercisePreferences });
      setApplicationPreferencesChanged(true);

      message.success("Character exercise preferences were saved.");
    },
    [applicationPreferences, setApplicationPreferences]
  );

  const [characterExercisePreferencesForm] = Form.useForm<CharacterExercisePreferences>();

  const characterExerciseProfiles = Form.useWatch<CharacterExerciseProfile[] | undefined>(keyOf<CharacterExercisePreferences>("exerciseProfiles"), characterExercisePreferencesForm);

  const createCharacterExerciseProfile = () => {
    const characterExerciseProfile: CharacterExerciseProfile = {
      id: uuidv4(),
      name: "",
      repeatedChallengeCountProgression: [],
    };
    return characterExerciseProfile;
  };

  const characterExercisePreferencesJsx = useMemo(
    () => (
      <>
        <Form
          size="small"
          layout="vertical"
          form={characterExercisePreferencesForm}
          initialValues={characterExercisePreferences}
          onFinish={setCharacterExercisePreferences}
          onFinishFailed={(errorInfo) => {
            void message.error("You need to fix the form validation errors before character exercise preferences can be saved.");
            console.error(errorInfo);
          }}
          /* eslint-disable-next-line @typescript-eslint/dot-notation */
          className={mobileBrowserMode ? styles["formMobile"] : styles["form"]}
        >
          <Form.Item name={keyOf<CharacterExercisePreferences>("defaultExerciseSize")} label="Default Exercise Size" rules={[{ required: true, type: "integer", min: 0 }]}>
            <ExerciseSizeInputNumber />
          </Form.Item>
          <Form.Item name={keyOf<CharacterExercisePreferences>("defaultExerciseMode")} label="Default Exercise Mode">
            <Select placeholder="Select a mode">
              <Select.Option value={valueOf<CharacterExerciseMode>("type")}>Type</Select.Option>
              <Select.Option value={valueOf<CharacterExerciseMode>("type-pronunciation")}>Type & Pronunciation</Select.Option>
              <Select.Option value={valueOf<CharacterExerciseMode>("character")}>Character</Select.Option>
              <Select.Option value={valueOf<CharacterExerciseMode>("character-pronunciation")}>Character & Pronunciation</Select.Option>
              <Select.Option value={valueOf<CharacterExerciseMode>("fullDescription")}>Full Description</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item name={keyOf<CharacterExercisePreferences>("defaultExerciseAlphabetMode")} label="Default Exercise Alphabet Mode">
            <Radio.Group>
              <Radio.Button value={valueOf<CharacterExerciseAlphabetMode>("kana")}>Kana</Radio.Button>
              <Radio.Button value={valueOf<CharacterExerciseAlphabetMode>("kanji")}>Kanji</Radio.Button>
              <Radio.Button value={valueOf<CharacterExerciseAlphabetMode>("mixed")}>Mixed</Radio.Button>
            </Radio.Group>
          </Form.Item>
          <Divider className={styles.formDivider} />
          <Paragraph>Exercise Profiles</Paragraph>
          <Form.List
            name={keyOf<CharacterExercisePreferences>("exerciseProfiles")}
            rules={[
              {
                validator: (_, characterExerciseProfiles: (CharacterExerciseProfile | undefined)[]) => {
                  if (Array.isArray(characterExerciseProfiles) && characterExerciseProfiles.every((characterExerciseProfile) => characterExerciseProfile)) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("Please fill the character exercise profile form"));
                },
              },
            ]}
          >
            {(characterExerciseProfileFields, { add: addCharacterExerciseProfileField, remove: removeCharacterExerciseProfileField }) => (
              <Space direction="vertical">
                {characterExerciseProfileFields.map((characterExerciseProfileField) => {
                  const characterExerciseProfileKey = characterExerciseProfileField.key;

                  const characterExerciseProfileName =
                    (characterExercisePreferencesForm.getFieldValue([keyOf<CharacterExercisePreferences>("exerciseProfiles"), characterExerciseProfileField.name, keyOf<CharacterExerciseProfile>("name")]) as string | undefined) ??
                    `Exercise Profile #${characterExerciseProfileField.name.toString()}`;

                  const collapseLabel = (
                    <Space direction="horizontal" align="baseline" className={styles.exerciseProfileCollapseLabel}>
                      <Text>{characterExerciseProfileName}</Text>
                      <DeleteOutlined
                        onClick={() => {
                          removeCharacterExerciseProfileField(characterExerciseProfileField.name);
                        }}
                      />
                    </Space>
                  );

                  const collapseChildren = (
                    <>
                      <Form.Item label="UUID" name={[characterExerciseProfileField.name, keyOf<CharacterExerciseProfile>("id")]} hidden>
                        <Input />
                      </Form.Item>
                      <Form.Item label="Name" name={[characterExerciseProfileField.name, keyOf<CharacterExerciseProfile>("name")]} rules={[{ required: true, type: "string" }]}>
                        <Input />
                      </Form.Item>
                      <Form.Item<number[]>
                        label="Repeated Challenge Count Progression"
                        name={[characterExerciseProfileField.name, keyOf<CharacterExerciseProfile>("repeatedChallengeCountProgression")]}
                        rules={[
                          {
                            transform: (repeatedChallengeCountProgression?: string[]) => repeatedChallengeCountProgression?.map((item) => Number(item)),
                            validator: (_, repeatedChallengeCountProgression?: number[]) => {
                              if (repeatedChallengeCountProgression && Array.isArray(repeatedChallengeCountProgression) && repeatedChallengeCountProgression.every((number) => !isNaN(number))) {
                                return Promise.resolve();
                              }
                              return Promise.reject(new Error("Please enter an array of numbers"));
                            },
                          },
                        ]}
                      >
                        <Select placeholder="Enter progression numbers" mode="tags" showSearch={false} allowClear />
                      </Form.Item>
                      <Form.Item label="Tag Distribution Rules">
                        <Form.List name={[characterExerciseProfileField.name, keyOf<CharacterExerciseProfile>("tagDistributionSettings"), keyOf<TagDistributionSettings>("rules")]}>
                          {(tagDistributionRuleFields, { add: addTagDistributionRuleField, remove: removeTagDistributionRuleField }) => {
                            return (
                              <Space direction="vertical">
                                {tagDistributionRuleFields.map((tagDistributionRuleField) => (
                                  <Space direction="horizontal" key={tagDistributionRuleField.key}>
                                    <Form.Item noStyle name={[tagDistributionRuleField.name, keyOf<TagDistributionRule>("tagCaption")]}>
                                      <Select placeholder="Tag Caption">
                                        {tagCaptions.map((tagCaption) => (
                                          <Select.Option value={tagCaption} key={tagCaption}>
                                            {tagCaption}
                                          </Select.Option>
                                        ))}
                                      </Select>
                                    </Form.Item>
                                    <Form.Item noStyle name={[tagDistributionRuleField.name, keyOf<TagDistributionRule>("exerciseBatchFraction")]} rules={[{ required: true, type: "number" }]}>
                                      <InputNumber placeholder="Fraction" />
                                    </Form.Item>
                                    <Form.Item noStyle name={[tagDistributionRuleField.name, keyOf<TagDistributionRule>("minInclusions")]} rules={[{ required: false, type: "integer" }]}>
                                      <InputNumber placeholder="Min" />
                                    </Form.Item>
                                    <Form.Item noStyle name={[tagDistributionRuleField.name, keyOf<TagDistributionRule>("maxInclusions")]} rules={[{ required: false, type: "integer" }]}>
                                      <InputNumber placeholder="Max" />
                                    </Form.Item>
                                    <DeleteOutlined onClick={() => removeTagDistributionRuleField(tagDistributionRuleField.name)} />
                                  </Space>
                                ))}
                                <Button type="dashed" icon={<PlusOutlined />} onClick={() => addTagDistributionRuleField()} block>
                                  Add Tag Distribution Rule
                                </Button>
                              </Space>
                            );
                          }}
                        </Form.List>
                      </Form.Item>
                    </>
                  );

                  return <Collapse key={characterExerciseProfileKey} size="small" items={[{ key: "0", label: collapseLabel, children: collapseChildren }]} />;
                })}
                <Button type="dashed" icon={<PlusOutlined />} onClick={() => addCharacterExerciseProfileField(createCharacterExerciseProfile())} block>
                  Add Exercise Profile
                </Button>
              </Space>
            )}
          </Form.List>
          <Form.Item name={keyOf<CharacterExercisePreferences>("defaultExerciseProfileId")} label="Default Exercise Profile">
            <Select placeholder="Select an exercise profile" allowClear>
              {characterExerciseProfiles?.map((characterExerciseProfile) => (
                <Select.Option value={characterExerciseProfile.id} key={characterExerciseProfile.id}>
                  {characterExerciseProfile.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Divider className={styles.formDivider} />
          <Paragraph>Automatically switch to the next exercise if the current one has been completed:</Paragraph>
          <Form.Item name={keyOf<CharacterExercisePreferences>("navigateOnCompletion")} valuePropName="checked" label="Enable">
            <Checkbox />
          </Form.Item>
          <Form.Item name={keyOf<CharacterExercisePreferences>("navigateOnCompletionDelayMilliseconds")} label="Delay (Milliseconds)">
            <DelayInputNumber />
          </Form.Item>
          <Divider className={styles.formDivider} />
          <Paragraph>Automatically switch to the next exercise if the current one has been failed:</Paragraph>
          <Form.Item name={keyOf<CharacterExercisePreferences>("navigateOnFailure")} valuePropName="checked" label="Enable">
            <Checkbox />
          </Form.Item>
          <Form.Item name={keyOf<CharacterExercisePreferences>("navigateOnFailureDelayMilliseconds")} label="Delay (Milliseconds)">
            <DelayInputNumber />
          </Form.Item>
          <Divider className={styles.formDivider} />
          <Form.Item>
            <Space direction="horizontal">
              <Button type="primary" htmlType="submit" icon={<SaveOutlined />}>
                Save
              </Button>
              <Button type="primary" htmlType="reset" icon={<UndoOutlined />} danger>
                Reset
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </>
    ),
    [mobileBrowserMode, tagCaptions, characterExercisePreferences, setCharacterExercisePreferences, characterExercisePreferencesForm, characterExerciseProfiles]
  );

  useEffect(() => characterExercisePreferencesForm.resetFields(), [characterExercisePreferencesForm]);

  const wordExercisePreferences = useMemo(() => {
    return applicationPreferences.wordExercisePreferences ?? ApplicationPreferencesConstants.wordExercisePreferencesDefaultValues;
  }, [applicationPreferences]);
  const setWordExercisePreferences = useCallback(
    (updatedWordExercisePreferences: WordExercisePreferences) => {
      for (const wordExerciseProfile of updatedWordExercisePreferences.exerciseProfiles ?? []) {
        wordExerciseProfile.repeatedChallengeCountProgression = (wordExerciseProfile.repeatedChallengeCountProgression as number[] | undefined)?.length
          ? wordExerciseProfile.repeatedChallengeCountProgression.map((item) => Number(item))
          : [];
      }

      setApplicationPreferences({ ...applicationPreferences, wordExercisePreferences: updatedWordExercisePreferences });
      setApplicationPreferencesChanged(true);

      message.success("Word exercise preferences were saved.");
    },
    [applicationPreferences, setApplicationPreferences]
  );

  const [wordExercisePreferencesForm] = Form.useForm<WordExercisePreferences>();

  const wordExerciseProfiles = Form.useWatch<WordExerciseProfile[] | undefined>(keyOf<WordExercisePreferences>("exerciseProfiles"), wordExercisePreferencesForm);

  const createWordExerciseProfile = () => {
    const wordExerciseProfile: WordExerciseProfile = {
      id: uuidv4(),
      name: "",
      repeatedChallengeCountProgression: [],
    };
    return wordExerciseProfile;
  };

  const wordExercisePreferencesJsx = useMemo(
    () => (
      <>
        <Form
          size="small"
          layout="vertical"
          form={wordExercisePreferencesForm}
          initialValues={wordExercisePreferences}
          onFinish={setWordExercisePreferences}
          onFinishFailed={(errorInfo) => {
            void message.error("You need to fix the form validation errors before word exercise preferences can be saved.");
            console.error(errorInfo);
          }}
          /* eslint-disable-next-line @typescript-eslint/dot-notation */
          className={mobileBrowserMode ? styles["formMobile"] : styles["form"]}
        >
          <Form.Item name={keyOf<WordExercisePreferences>("defaultExerciseSize")} label="Default Exercise Size">
            <ExerciseSizeInputNumber />
          </Form.Item>
          <Form.Item name={keyOf<WordExercisePreferences>("defaultExerciseMode")} label="Default Exercise Mode">
            <Select placeholder="Select a mode">
              <Select.Option value={valueOf<WordExerciseMode>("characterTypes")}>Character Types</Select.Option>
              <Select.Option value={valueOf<WordExerciseMode>("characterTypes-pronunciation")}>Character Types & Pronunciation</Select.Option>
              <Select.Option value={valueOf<WordExerciseMode>("characters")}>Characters</Select.Option>
              <Select.Option value={valueOf<WordExerciseMode>("characters-pronunciation")}>Characters & Pronunciation</Select.Option>
              <Select.Option value={valueOf<WordExerciseMode>("fullDescription")}>Full Description</Select.Option>
            </Select>
          </Form.Item>
          <Divider className={styles.formDivider} />
          <Paragraph>Exercise Profiles</Paragraph>
          <Form.List
            name={keyOf<WordExercisePreferences>("exerciseProfiles")}
            rules={[
              {
                validator: (_, wordExerciseProfiles: (WordExerciseProfile | undefined)[]) => {
                  if (Array.isArray(wordExerciseProfiles) && wordExerciseProfiles.every((wordExerciseProfile) => wordExerciseProfile)) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("Please fill the word exercise profile form"));
                },
              },
            ]}
          >
            {(wordExerciseProfileFields, { add: addWordExerciseProfileField, remove: removeWordExerciseProfileField }) => (
              <Space direction="vertical">
                {wordExerciseProfileFields.map((wordExerciseProfileField) => {
                  const wordExerciseProfileKey = wordExerciseProfileField.key;

                  const wordExerciseProfileName =
                    (wordExercisePreferencesForm.getFieldValue([keyOf<WordExercisePreferences>("exerciseProfiles"), wordExerciseProfileField.name, keyOf<WordExerciseProfile>("name")]) as string | undefined) ??
                    `Exercise Profile #${wordExerciseProfileField.name.toString()}`;

                  const collapseLabel = (
                    <Space direction="horizontal" align="baseline" className={styles.exerciseProfileCollapseLabel}>
                      <Text>{wordExerciseProfileName}</Text>
                      <DeleteOutlined
                        onClick={() => {
                          removeWordExerciseProfileField(wordExerciseProfileField.name);
                        }}
                      />
                    </Space>
                  );

                  const collapseChildren = (
                    <>
                      <Form.Item label="UUID" name={[wordExerciseProfileField.name, keyOf<WordExerciseProfile>("id")]} hidden>
                        <Input />
                      </Form.Item>
                      <Form.Item label="Name" name={[wordExerciseProfileField.name, keyOf<WordExerciseProfile>("name")]} rules={[{ required: true, type: "string" }]}>
                        <Input />
                      </Form.Item>
                      <Form.Item<number[]>
                        label="Repeated Challenge Count Progression"
                        name={[wordExerciseProfileField.name, keyOf<WordExerciseProfile>("repeatedChallengeCountProgression")]}
                        rules={[
                          {
                            transform: (repeatedChallengeCountProgression?: string[]) => repeatedChallengeCountProgression?.map((item) => Number(item)),
                            validator: (_, repeatedChallengeCountProgression?: number[]) => {
                              if (repeatedChallengeCountProgression && Array.isArray(repeatedChallengeCountProgression) && repeatedChallengeCountProgression.every((number) => !isNaN(number))) {
                                return Promise.resolve();
                              }
                              return Promise.reject(new Error("Please enter an array of numbers"));
                            },
                          },
                        ]}
                      >
                        <Select placeholder="Enter progression numbers" mode="tags" showSearch={false} allowClear />
                      </Form.Item>
                      <Form.Item label="Tag Distribution Rules">
                        <Form.List name={[wordExerciseProfileField.name, keyOf<WordExerciseProfile>("tagDistributionSettings"), keyOf<TagDistributionSettings>("rules")]}>
                          {(tagDistributionRuleFields, { add: addTagDistributionRuleField, remove: removeTagDistributionRuleField }) => {
                            return (
                              <Space direction="vertical">
                                {tagDistributionRuleFields.map((tagDistributionRuleField) => (
                                  <Space direction="horizontal" key={tagDistributionRuleField.key}>
                                    <Form.Item noStyle name={[tagDistributionRuleField.name, keyOf<TagDistributionRule>("tagCaption")]}>
                                      <Select placeholder="Tag Caption">
                                        {tagCaptions.map((tagCaption) => (
                                          <Select.Option value={tagCaption} key={tagCaption}>
                                            {tagCaption}
                                          </Select.Option>
                                        ))}
                                      </Select>
                                    </Form.Item>
                                    <Form.Item noStyle name={[tagDistributionRuleField.name, keyOf<TagDistributionRule>("exerciseBatchFraction")]} rules={[{ required: true, type: "number" }]}>
                                      <InputNumber placeholder="Fraction" />
                                    </Form.Item>
                                    <Form.Item noStyle name={[tagDistributionRuleField.name, keyOf<TagDistributionRule>("minInclusions")]} rules={[{ required: false, type: "integer" }]}>
                                      <InputNumber placeholder="Min" />
                                    </Form.Item>
                                    <Form.Item noStyle name={[tagDistributionRuleField.name, keyOf<TagDistributionRule>("maxInclusions")]} rules={[{ required: false, type: "integer" }]}>
                                      <InputNumber placeholder="Max" />
                                    </Form.Item>
                                    <DeleteOutlined onClick={() => removeTagDistributionRuleField(tagDistributionRuleField.name)} />
                                  </Space>
                                ))}
                                <Button type="dashed" icon={<PlusOutlined />} onClick={() => addTagDistributionRuleField()} block>
                                  Add Tag Distribution Rule
                                </Button>
                              </Space>
                            );
                          }}
                        </Form.List>
                      </Form.Item>
                    </>
                  );

                  return <Collapse key={wordExerciseProfileKey} size="small" items={[{ key: "0", label: collapseLabel, children: collapseChildren }]} />;
                })}
                <Button type="dashed" icon={<PlusOutlined />} onClick={() => addWordExerciseProfileField(createWordExerciseProfile())} block>
                  Add Exercise Profile
                </Button>
              </Space>
            )}
          </Form.List>
          <Form.Item name={keyOf<WordExercisePreferences>("defaultExerciseProfileId")} label="Default Exercise Profile">
            <Select placeholder="Select an exercise profile" allowClear>
              {wordExerciseProfiles?.map((wordExerciseProfile) => (
                <Select.Option value={wordExerciseProfile.id} key={wordExerciseProfile.id}>
                  {wordExerciseProfile.name}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Divider className={styles.formDivider} />
          <Paragraph>Automatically switch to the next exercise if the current one has been completed:</Paragraph>
          <Form.Item name={keyOf<WordExercisePreferences>("navigateOnCompletion")} valuePropName="checked" label="Enable">
            <Checkbox />
          </Form.Item>
          <Form.Item name={keyOf<WordExercisePreferences>("navigateOnCompletionDelayMilliseconds")} label="Delay (Milliseconds)">
            <DelayInputNumber />
          </Form.Item>
          <Divider className={styles.formDivider} />
          <Paragraph>Automatically switch to the next exercise if the current one has been failed:</Paragraph>
          <Form.Item name={keyOf<WordExercisePreferences>("navigateOnFailure")} valuePropName="checked" label="Enable">
            <Checkbox />
          </Form.Item>
          <Form.Item name={keyOf<WordExercisePreferences>("navigateOnFailureDelayMilliseconds")} label="Delay (Milliseconds)">
            <DelayInputNumber />
          </Form.Item>
          <Divider className={styles.formDivider} />
          <Form.Item>
            <Space direction="horizontal">
              <Button type="primary" htmlType="submit" icon={<SaveOutlined />}>
                Save
              </Button>
              <Button type="primary" htmlType="reset" icon={<UndoOutlined />} danger>
                Reset
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </>
    ),
    [mobileBrowserMode, tagCaptions, wordExercisePreferences, setWordExercisePreferences, wordExercisePreferencesForm, wordExerciseProfiles]
  );

  useEffect(() => wordExercisePreferencesForm.resetFields(), [wordExercisePreferencesForm]);

  const [applicationPreferencesJsonText, setApplicationPreferencesJsonText] = useState<string>(ObjectConstants.emptyObjectString);

  useEffect(() => setApplicationPreferencesJsonText(JSON.stringify(applicationPreferences)), [applicationPreferences]);

  const [replaceApplicationPreferencesButtonEnabled, setReplaceApplicationPreferencesButtonEnabled] = useState<boolean>(false);

  const replaceApplicationPreferences = useCallback(() => {
    if (!applicationPreferencesJsonText) {
      return;
    }

    const applicationPreferences: ApplicationPreferences = JSON.parse(applicationPreferencesJsonText) as ApplicationPreferences;

    setApplicationPreferencesImported(true);
    setApplicationPreferences(applicationPreferences);
  }, [applicationPreferencesJsonText, setApplicationPreferences]);

  const [resetSuccessfulAlertVisible, setResetSuccessfulAlertVisible] = useState<boolean>(false);

  const [resetApplicationPreferencesButtonEnabled, setResetApplicationPreferencesButtonEnabled] = useState<boolean>(false);

  const resetApplicationPreferences = useCallback(() => {
    setResetSuccessfulAlertVisible(true);
    setApplicationPreferences(ApplicationPreferencesConstants.applicationPreferencesDefaultValues);
  }, [setApplicationPreferences]);

  const additionalOptionsJsx = useMemo(
    () => (
      <Space style={{ display: "flex" }} direction="vertical">
        <Paragraph>You can copy the current configuration below and transfer it to another browser:</Paragraph>
        {applicationPreferencesChanged && <Alert message="These settings have just been changed. Please, reload the page before copying them." type="warning" showIcon />}
        {applicationPreferencesImported && <Alert message="These settings have just been imported. Please, reload the page before modifying them." type="warning" showIcon />}
        <Input.TextArea rows={4} value={applicationPreferencesJsonText} onChange={(e) => setApplicationPreferencesJsonText(e.target.value)} style={{ fontFamily: "monospace" }} placeholder="Paste your JSON configuration here." />
        <Space direction="horizontal">
          <Button type="primary" size="small" disabled={!replaceApplicationPreferencesButtonEnabled} icon={<RedoOutlined />} onClick={replaceApplicationPreferences} danger>
            Replace Current Preferences
          </Button>
          <Checkbox onChange={(e) => setReplaceApplicationPreferencesButtonEnabled(e.target.checked)}>Check to confirm</Checkbox>
        </Space>
        <Divider className={styles.standaloneDivider} />
        <Paragraph>Use the button below to reset all preferences to their default values. This may resolve issues caused by an application upgrade.</Paragraph>
        {resetSuccessfulAlertVisible ? <Alert message="The reset was successful, refresh the page to see the changes." type="warning" showIcon /> : undefined}
        <Space direction="horizontal">
          <Button type="primary" size="small" disabled={!resetApplicationPreferencesButtonEnabled} icon={<UndoOutlined />} onClick={resetApplicationPreferences} danger>
            Reset All Preferences
          </Button>
          <Checkbox onChange={(e) => setResetApplicationPreferencesButtonEnabled(e.target.checked)}>Check to confirm</Checkbox>
        </Space>
      </Space>
    ),
    [
      applicationPreferencesChanged,
      applicationPreferencesImported,
      applicationPreferencesJsonText,
      replaceApplicationPreferencesButtonEnabled,
      replaceApplicationPreferences,
      resetSuccessfulAlertVisible,
      resetApplicationPreferencesButtonEnabled,
      resetApplicationPreferences,
    ]
  );

  return (
    <>
      <Title level={4}>Preferences</Title>
      <Space style={{ display: "flex" }} direction="vertical">
        <Collapse
          size="small"
          items={[
            {
              key: "character-exercise-preferences",
              label: (
                <Space>
                  <FormOutlined />
                  Character Exercises
                </Space>
              ),
              children: characterExercisePreferencesJsx,
              forceRender: true,
            },
            {
              key: "word-exercise-preferences",
              label: (
                <Space>
                  <ProfileOutlined />
                  Word Exercises
                </Space>
              ),
              children: wordExercisePreferencesJsx,
              forceRender: true,
            },
            {
              key: "additional-options",
              label: (
                <Space>
                  <ControlOutlined />
                  Miscellaneous
                </Space>
              ),
              children: additionalOptionsJsx,
              forceRender: true,
            },
          ]}
        />
        <Alert message="Please note that these settings are browser-specific and are stored locally." type="info" showIcon />
      </Space>
    </>
  );
};

export default PreferencesPage;
