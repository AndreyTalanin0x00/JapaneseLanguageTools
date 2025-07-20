using System;
using System.Text.Json.Serialization;

using JapaneseLanguageTools.Contracts.Enumerations;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Json;

public class TagObjectPackageJsonModel
{
    [JsonPropertyName("snapshotType")]
    public SnapshotType SnapshotType { get; set; }

    [JsonIgnore]
    public DateTimeOffset SnapshotTime { get; set; }

    [JsonPropertyName("snapshotTime")]
    public string SnapshotTimeString
    {
        get => SnapshotTime.ToString("s");
        set => SnapshotTime = !string.IsNullOrEmpty(value) ? DateTimeOffset.Parse(value) : default(DateTimeOffset);
    }

    [JsonPropertyName("snapshotHash")]
    public string SnapshotHash { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public TagJsonModel[] Tags { get; set; } = [];

    [JsonPropertyName("forceMode")]
    public bool ForceMode { get; set; }
}
