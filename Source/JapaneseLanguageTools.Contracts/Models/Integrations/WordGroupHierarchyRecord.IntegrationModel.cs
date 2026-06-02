namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class WordGroupHierarchyRecordIntegrationModel
{
    public int WordGroupId { get; set; }

    public int NestedWordGroupId { get; set; }

    public string? NestedWordGroupCaption { get; set; }

    public bool PreventRecursiveIncludes { get; set; }
}
