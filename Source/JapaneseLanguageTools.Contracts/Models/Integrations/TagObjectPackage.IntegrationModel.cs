using System;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class TagObjectPackageIntegrationModel
{
    public SnapshotType SnapshotType { get; set; }

    public DateTimeOffset SnapshotTime { get; set; }

    public string SnapshotHash { get; set; } = string.Empty;

    public TagIntegrationModel[] Tags { get; set; } = [];

    public bool ForceMode { get; set; }
}
