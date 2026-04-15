enum CharacterTypes {
  None = 0x00,
  Hiragana = 0x01,
  Katakana = 0x02,
  Kanji = 0x04,
  // eslint-disable-next-line @typescript-eslint/prefer-literal-enum-member
  AllExceptKanji = Hiragana | Katakana,
  // eslint-disable-next-line @typescript-eslint/prefer-literal-enum-member
  All = AllExceptKanji | Kanji,
}

export default CharacterTypes;

// eslint-disable-next-line @typescript-eslint/no-extraneous-class
export class CharacterTypesUtils {
  public static characterTypesToString(characterTypes: CharacterTypes): string {
    if (characterTypes == CharacterTypes.None) {
      return CharacterTypes[CharacterTypes.None];
    }

    const compositeValues = [CharacterTypes.AllExceptKanji, CharacterTypes.All];
    for (const compositeValue of compositeValues) {
      if (characterTypes == compositeValue) {
        return CharacterTypes[compositeValue];
      }
    }

    const resultValues = [];
    const atomicValues = [CharacterTypes.Hiragana, CharacterTypes.Katakana, CharacterTypes.Kanji];
    for (const atomicValue of atomicValues) {
      // eslint-disable-next-line @typescript-eslint/no-unsafe-enum-comparison
      if ((atomicValue & characterTypes) == atomicValue) {
        resultValues.push(CharacterTypes[atomicValue]);
      }
    }

    const resultString = resultValues.join(", ");
    return resultString;
  }
}
