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
