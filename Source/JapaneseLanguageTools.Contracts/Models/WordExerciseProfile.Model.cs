using System.Collections.Generic;

namespace JapaneseLanguageTools.Contracts.Models;

public class WordExerciseProfileModel
{
    public IList<int> RepeatedChallengeCountProgression { get; set; } = [];

    public TagDistributionPreferencesModel? TagDistributionPreferences { get; set; }
}
