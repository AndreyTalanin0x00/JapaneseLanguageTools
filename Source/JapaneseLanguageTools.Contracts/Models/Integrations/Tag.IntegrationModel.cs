using System;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class TagIntegrationModel
{
    public int Id { get; set; }

    public SnapshotObjectAction Action { get; set; }

    public string Caption { get; set; } = string.Empty;

    public string? PlaceholderMarker { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
}
