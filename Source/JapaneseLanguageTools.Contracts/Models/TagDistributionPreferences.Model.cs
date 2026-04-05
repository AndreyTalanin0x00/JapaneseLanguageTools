using System.Collections.Generic;

namespace JapaneseLanguageTools.Contracts.Models;

public class TagDistributionPreferencesModel
{
    public IList<TagDistributionRuleModel> Rules { get; set; } = [];
}
