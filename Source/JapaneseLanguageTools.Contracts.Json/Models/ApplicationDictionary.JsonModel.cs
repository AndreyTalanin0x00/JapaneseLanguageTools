using System.Text.Json.Serialization;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Json;

public class ApplicationDictionaryJsonModel
{
    [JsonPropertyName("characters")]
    public CharacterJsonModel[] Characters { get; set; } = [];

    [JsonPropertyName("characterGroups")]
    public CharacterGroupJsonModel[] CharacterGroups { get; set; } = [];

    [JsonPropertyName("words")]
    public WordJsonModel[] Words { get; set; } = [];

    [JsonPropertyName("wordGroups")]
    public WordGroupJsonModel[] WordGroups { get; set; } = [];

    [JsonPropertyName("tags")]
    public TagJsonModel[] Tags { get; set; } = [];
}
