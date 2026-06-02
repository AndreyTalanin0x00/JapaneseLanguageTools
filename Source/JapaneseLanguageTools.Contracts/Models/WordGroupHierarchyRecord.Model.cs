namespace JapaneseLanguageTools.Contracts.Models;

public class WordGroupHierarchyRecordModel
{
    public int WordGroupId { get; set; }

    public int NestedWordGroupId { get; set; }

    public bool PreventRecursiveIncludes { get; set; }

    public WordGroupModel? WordGroup { get; set; }

    public WordGroupModel? NestedWordGroup { get; set; }
}
