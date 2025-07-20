using System;
using System.Xml.Serialization;

using JapaneseLanguageTools.Contracts.Enumerations;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Xml;

[XmlRoot(TagXmlElementName)]
public class TagXmlModel
{
    public const string TagXmlElementName = "Tag";

    /// <remarks>See the <see cref="IdSpecified" /> related property.</remarks>
    [XmlAttribute]
    public int Id { get; set; }

    /// <remarks>Indicates whether the <see cref="Id" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool IdSpecified => Id > 0;

    [XmlAttribute]
    public SnapshotObjectAction Action { get; set; }

    [XmlAttribute]
    public string Caption { get; set; } = string.Empty;

    /// <remarks>See the <see cref="PlaceholderMarkerSpecified" /> related property.</remarks>
    [XmlAttribute]
    public string? PlaceholderMarker { get; set; }

    /// <remarks>Indicates whether the <see cref="PlaceholderMarker" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool PlaceholderMarkerSpecified => !string.IsNullOrEmpty(PlaceholderMarker);

    /// <remarks>See the <see cref="CreatedOnString" /> and <see cref="CreatedOnStringSpecified" /> related properties.</remarks>
    [XmlIgnore]
    public DateTimeOffset CreatedOn { get; set; }

    /// <remarks>Provides a serializable string representation for the <see cref="DateTimeOffset" /> value of the <see cref="CreatedOn" /> property.</remarks>
    [XmlAttribute(nameof(CreatedOn))]
    public string? CreatedOnString
    {
        get => CreatedOn.ToString("u");
        set => CreatedOn = !string.IsNullOrEmpty(value) ? DateTimeOffset.Parse(value) : default(DateTimeOffset);
    }

    /// <remarks>Indicates whether the <see cref="CreatedOnString" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool CreatedOnStringSpecified => CreatedOn != default(DateTimeOffset);

    /// <remarks>See the <see cref="UpdatedOnString" /> and <see cref="UpdatedOnStringSpecified" /> related properties.</remarks>
    [XmlIgnore]
    public DateTimeOffset UpdatedOn { get; set; }

    /// <remarks>Provides a serializable string representation for the <see cref="DateTimeOffset" /> value of the <see cref="UpdatedOn" /> property.</remarks>
    [XmlAttribute(nameof(UpdatedOn))]
    public string? UpdatedOnString
    {
        get => UpdatedOn.ToString("u");
        set => UpdatedOn = !string.IsNullOrEmpty(value) ? DateTimeOffset.Parse(value) : default(DateTimeOffset);
    }

    /// <remarks>Indicates whether the <see cref="UpdatedOnString" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool UpdatedOnStringSpecified => UpdatedOn != default(DateTimeOffset);
}
