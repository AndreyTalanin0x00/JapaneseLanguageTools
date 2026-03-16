using System;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class CharacterGroupIntegrationModel
{
    public int Id { get; set; }

    public SnapshotObjectAction Action { get; set; }

    public string Caption { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public bool Enabled { get; set; }

    public bool AlwaysUse { get; set; }

    public bool Hidden { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }

    public CharacterIntegrationModel[] Characters { get; set; } = [];
}
