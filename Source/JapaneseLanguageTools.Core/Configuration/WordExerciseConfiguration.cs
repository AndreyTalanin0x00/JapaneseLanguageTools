using System;

namespace JapaneseLanguageTools.Core.Configuration;

public class WordExerciseConfiguration
{
    public static readonly string SectionName = "WordExerciseConfiguration";

    public TimeSpan RepeatedChallengeExpirationTimeout { get; set; }
}
