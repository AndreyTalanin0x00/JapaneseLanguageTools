namespace JapaneseLanguageTools.Data.Entities;

public class WordGroupHierarchyRecord
{
    public int WordGroupId { get; set; }

    public int NestedWordGroupId { get; set; }

    public bool PreventRecursiveIncludes { get; set; }

    public WordGroup? WordGroup { get; set; }

    public WordGroup? NestedWordGroup { get; set; }
}
