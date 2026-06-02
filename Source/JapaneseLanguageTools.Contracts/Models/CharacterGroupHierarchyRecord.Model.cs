namespace JapaneseLanguageTools.Contracts.Models;

public class CharacterGroupHierarchyRecordModel
{
    public int CharacterGroupId { get; set; }

    public int NestedCharacterGroupId { get; set; }

    public bool PreventRecursiveIncludes { get; set; }

    public CharacterGroupModel? CharacterGroup { get; set; }

    public CharacterGroupModel? NestedCharacterGroup { get; set; }
}
