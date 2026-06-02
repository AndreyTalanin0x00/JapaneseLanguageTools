namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class CharacterGroupHierarchyRecordIntegrationModel
{
    public int CharacterGroupId { get; set; }

    public int NestedCharacterGroupId { get; set; }

    public string? NestedCharacterGroupCaption { get; set; }

    public bool PreventRecursiveIncludes { get; set; }
}
