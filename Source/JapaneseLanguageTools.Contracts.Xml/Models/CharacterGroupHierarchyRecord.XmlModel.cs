using System.Xml.Serialization;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Xml;

public class CharacterGroupHierarchyRecordXmlModel
{
    public const string CharacterGroupHierarchyRecordXmlElementName = "CharacterGroupHierarchyRecord";

    /// <remarks>See the <see cref="CharacterGroupIdSpecified" /> related property.</remarks>
    [XmlAttribute]
    public int CharacterGroupId { get; set; }

    /// <remarks>Indicates whether the <see cref="CharacterGroupId" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool CharacterGroupIdSpecified => CharacterGroupId > 0;

    /// <remarks>See the <see cref="NestedCharacterGroupIdSpecified" /> related property.</remarks>
    [XmlAttribute]
    public int NestedCharacterGroupId { get; set; }

    /// <remarks>Indicates whether the <see cref="NestedCharacterGroupId" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool NestedCharacterGroupIdSpecified => NestedCharacterGroupId > 0;

    [XmlAttribute]
    public string? NestedCharacterGroupCaption { get; set; }

    [XmlAttribute]
    public bool PreventRecursiveIncludes { get; set; }
}
