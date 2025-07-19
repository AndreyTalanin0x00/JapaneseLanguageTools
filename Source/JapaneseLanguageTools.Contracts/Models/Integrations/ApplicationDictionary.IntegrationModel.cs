namespace JapaneseLanguageTools.Contracts.Models.Integrations;

public class ApplicationDictionaryIntegrationModel
{
    public CharacterIntegrationModel[] Characters { get; set; } = [];

    public CharacterGroupIntegrationModel[] CharacterGroups { get; set; } = [];

    public WordIntegrationModel[] Words { get; set; } = [];

    public WordGroupIntegrationModel[] WordGroups { get; set; } = [];

    public TagIntegrationModel[] Tags { get; set; } = [];
}
