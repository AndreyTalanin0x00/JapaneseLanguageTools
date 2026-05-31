using System.Text.Json.Serialization;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Json;

public class CharacterGroupHierarchyRecordJsonModel
{
    [JsonPropertyName("characterGroupId")]
    public int CharacterGroupId { get; set; }

    [JsonPropertyName("nestedCharacterGroupId")]
    public int NestedCharacterGroupId { get; set; }

    [JsonPropertyName("nestedCharacterGroupCaption")]
    public string? NestedCharacterGroupCaption { get; set; }

    [JsonPropertyName("preventRecursiveIncludes")]
    public bool PreventRecursiveIncludes { get; set; }
}
