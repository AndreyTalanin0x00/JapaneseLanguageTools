namespace JapaneseLanguageTools.Data.Entities;

public class CharacterGroupHierarchyRecord
{
    public int CharacterGroupId { get; set; }

    public int NestedCharacterGroupId { get; set; }

    public bool PreventRecursiveIncludes { get; set; }

    public CharacterGroup? CharacterGroup { get; set; }

    public CharacterGroup? NestedCharacterGroup { get; set; }
}
