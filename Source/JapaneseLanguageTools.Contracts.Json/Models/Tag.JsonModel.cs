using System;
using System.Text.Json.Serialization;

using JapaneseLanguageTools.Contracts.Enumerations;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Json;

public class TagJsonModel
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("action")]
    public SnapshotObjectAction Action { get; set; }

    [JsonPropertyName("caption")]
    public string Caption { get; set; } = string.Empty;

    [JsonPropertyName("placeholderMarker")]
    public string? PlaceholderMarker { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTimeOffset CreatedOn { get; set; }

    [JsonPropertyName("updatedOn")]
    public DateTimeOffset UpdatedOn { get; set; }
}
