using System;
using System.Xml.Serialization;

using JapaneseLanguageTools.Contracts.Enumerations;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Xml;

[XmlRoot(TagObjectPackageXmlElementName)]
public class TagObjectPackageXmlModel
{
    public const string TagObjectPackageXmlElementName = "TagObjectPackage";

    [XmlAttribute]
    public SnapshotType SnapshotType { get; set; }

    [XmlIgnore]
    public DateTimeOffset SnapshotTime { get; set; }

    [XmlElement(nameof(SnapshotTime))]
    public string? SnapshotTimeString
    {
        get => SnapshotTime.ToString("s");
        set => SnapshotTime = !string.IsNullOrEmpty(value) ? DateTimeOffset.Parse(value) : default(DateTimeOffset);
    }

    [XmlElement]
    public string SnapshotHash { get; set; } = string.Empty;

    [XmlArray]
    [XmlArrayItem(TagXmlModel.TagXmlElementName)]
    public TagXmlModel[] Tags { get; set; } = [];

    [XmlElement]
    public bool ForceMode { get; set; }
}
