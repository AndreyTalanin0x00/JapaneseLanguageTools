using System;
using System.Text.Json.Serialization;

using JapaneseLanguageTools.Contracts.Enumerations;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Json;

public class WordJsonModel
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("action")]
    public SnapshotObjectAction Action { get; set; }

    [JsonPropertyName("wordGroupId")]
    public int? WordGroupId { get; set; }

    [JsonPropertyName("characters")]
    public string Characters { get; set; } = string.Empty;

    [JsonPropertyName("characterTypes")]
    public CharacterTypes CharacterTypes { get; set; }

    [JsonPropertyName("pronunciation")]
    public string? Pronunciation { get; set; }

    [JsonPropertyName("furigana")]
    public string? Furigana { get; set; }

    [JsonPropertyName("okurigana")]
    public string? Okurigana { get; set; }

    [JsonPropertyName("meaning")]
    public string? Meaning { get; set; }

    /// <remarks>The <see cref="string" /> value contains a comma- or semicolon-separated list of tags.</remarks>
    [JsonPropertyName("tags")]
    public string? Tags { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTimeOffset CreatedOn { get; set; }

    [JsonPropertyName("updatedOn")]
    public DateTimeOffset UpdatedOn { get; set; }
}
