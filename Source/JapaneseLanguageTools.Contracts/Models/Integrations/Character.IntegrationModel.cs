using System;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class CharacterIntegrationModel
{
    public int Id { get; set; }

    public SnapshotObjectAction Action { get; set; }

    public int? CharacterGroupId { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public CharacterTypes Type { get; set; }

    public string? Pronunciation { get; set; }

    public string? Syllable { get; set; }

    public string? Onyomi { get; set; }

    public string? Kunyomi { get; set; }

    public string? Meaning { get; set; }

    public string? Tags { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}
