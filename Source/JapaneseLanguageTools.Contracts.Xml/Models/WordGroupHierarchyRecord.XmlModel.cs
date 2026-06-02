using System.Xml.Serialization;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Xml;

public class WordGroupHierarchyRecordXmlModel
{
    public const string WordGroupHierarchyRecordXmlElementName = "WordGroupHierarchyRecord";

    /// <remarks>See the <see cref="WordGroupIdSpecified" /> related property.</remarks>
    [XmlAttribute]
    public int WordGroupId { get; set; }

    /// <remarks>Indicates whether the <see cref="WordGroupId" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool WordGroupIdSpecified => WordGroupId > 0;

    /// <remarks>See the <see cref="NestedWordGroupIdSpecified" /> related property.</remarks>
    [XmlAttribute]
    public int NestedWordGroupId { get; set; }

    /// <remarks>Indicates whether the <see cref="NestedWordGroupId" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool NestedWordGroupIdSpecified => NestedWordGroupId > 0;

    [XmlAttribute]
    public string? NestedWordGroupCaption { get; set; }

    [XmlAttribute]
    public bool PreventRecursiveIncludes { get; set; }
}
