using System;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class WordIntegrationModel
{
    public int Id { get; set; }

    public SnapshotObjectAction Action { get; set; }

    public int? WordGroupId { get; set; }

    public string Characters { get; set; } = string.Empty;

    public CharacterTypes CharacterTypes { get; set; }

    public string? Pronunciation { get; set; }

    public string? Furigana { get; set; }

    public string? Okurigana { get; set; }

    public string? Meaning { get; set; }

    public string? Tags { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}
