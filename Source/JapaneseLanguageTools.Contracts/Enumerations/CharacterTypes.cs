using System;

namespace JapaneseLanguageTools.Contracts.Enumerations;

[Flags]
public enum CharacterTypes
{
    None = 0x00,

    Hiragana = 0x01,

    Katakana = 0x02,

    Kanji = 0x04,

    AllExceptKanji = Hiragana | Katakana,

    All = AllExceptKanji | Kanji,
}
