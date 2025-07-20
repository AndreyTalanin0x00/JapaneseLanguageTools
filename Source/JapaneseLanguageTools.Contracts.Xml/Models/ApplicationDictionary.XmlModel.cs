using System.Xml.Serialization;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Xml;

[XmlRoot(ApplicationDictionaryXmlElementName)]
public class ApplicationDictionaryXmlModel
{
    public const string ApplicationDictionaryXmlElementName = "ApplicationDictionary";

    [XmlArray]
    [XmlArrayItem(CharacterXmlModel.CharacterXmlElementName)]
    public CharacterXmlModel[] Characters { get; set; } = [];

    [XmlArray]
    [XmlArrayItem(CharacterGroupXmlModel.CharacterGroupXmlElementName)]
    public CharacterGroupXmlModel[] CharacterGroups { get; set; } = [];

    [XmlArray]
    [XmlArrayItem(WordXmlModel.WordXmlElementName)]
    public WordXmlModel[] Words { get; set; } = [];

    [XmlArray]
    [XmlArrayItem(WordGroupXmlModel.WordGroupXmlElementName)]
    public WordGroupXmlModel[] WordGroups { get; set; } = [];

    [XmlArray]
    [XmlArrayItem(TagXmlModel.TagXmlElementName)]
    public TagXmlModel[] Tags { get; set; } = [];
}
