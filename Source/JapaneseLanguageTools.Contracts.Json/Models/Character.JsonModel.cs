using System;
using System.Text.Json.Serialization;

using JapaneseLanguageTools.Contracts.Enumerations;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Json;

public class CharacterJsonModel
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("action")]
    public SnapshotObjectAction Action { get; set; }

    [JsonPropertyName("characterGroupId")]
    public int? CharacterGroupId { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public CharacterTypes Type { get; set; }

    [JsonPropertyName("pronunciation")]
    public string? Pronunciation { get; set; }

    [JsonPropertyName("syllable")]
    public string? Syllable { get; set; }

    [JsonPropertyName("onyomi")]
    public string? Onyomi { get; set; }

    [JsonPropertyName("kunyomi")]
    public string? Kunyomi { get; set; }

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
