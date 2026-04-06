using System;

namespace JapaneseLanguageTools.Core.Configuration;

public class CharacterExerciseConfiguration
{
    public static readonly string SectionName = "CharacterExerciseConfiguration";

    public TimeSpan RepeatedChallengeExpirationTimeout { get; set; }
}
