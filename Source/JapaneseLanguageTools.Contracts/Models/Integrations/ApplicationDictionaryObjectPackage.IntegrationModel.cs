using System;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class ApplicationDictionaryObjectPackageIntegrationModel
{
    public SnapshotType SnapshotType { get; set; }

    public DateTimeOffset SnapshotTime { get; set; }

    public string SnapshotHash { get; set; } = string.Empty;

    public ApplicationDictionaryIntegrationModel ApplicationDictionary { get; set; } = new();

    public bool ForceMode { get; set; }
}
