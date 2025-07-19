namespace JapaneseLanguageTools.Contracts.Models;

public class TagDistributionRuleModel
{
    public required string TagCaption { get; set; }

    public float ExerciseBatchFraction { get; set; }

    public int? MaxInclusions { get; set; }

    public int? MinInclusions { get; set; }
}
