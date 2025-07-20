using System;
using System.Xml.Serialization;

using JapaneseLanguageTools.Contracts.Enumerations;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Xml;

[XmlRoot(CharacterXmlElementName)]
public class CharacterXmlModel
{
    public const string CharacterXmlElementName = "Character";

    /// <remarks>See the <see cref="IdSpecified" /> related property.</remarks>
    [XmlAttribute]
    public int Id { get; set; }

    /// <remarks>Indicates whether the <see cref="Id" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool IdSpecified => Id > 0;

    [XmlAttribute]
    public SnapshotObjectAction Action { get; set; }

    /// <remarks>See the <see cref="CharacterGroupIdString" /> and <see cref="CharacterGroupIdStringSpecified" /> related properties.</remarks>
    [XmlIgnore]
    public int? CharacterGroupId { get; set; }

    /// <remarks>Provides a serializable string representation for the <see cref="Nullable{T}" /> value of the <see cref="CharacterGroupId" /> property.</remarks>
    [XmlAttribute(nameof(CharacterGroupId))]
    public string? CharacterGroupIdString
    {
        get => CharacterGroupId.ToString();
        set => CharacterGroupId = !string.IsNullOrEmpty(value) ? int.Parse(value) : null;
    }

    /// <remarks>Indicates whether the <see cref="CharacterGroupIdString" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool CharacterGroupIdStringSpecified => CharacterGroupId is not null && CharacterGroupId > 0;

    [XmlAttribute]
    public string Symbol { get; set; } = string.Empty;

    [XmlAttribute]
    public CharacterTypes Type { get; set; }

    /// <remarks>See the <see cref="PronunciationSpecified" /> and <see cref="PronunciationSpecifiedOverride" /> related properties.</remarks>
    [XmlAttribute]
    public string? Pronunciation { get; set; }

    /// <remarks>Indicates whether the <see cref="Pronunciation" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool PronunciationSpecified => PronunciationSpecifiedOverride ?? !string.IsNullOrEmpty(Pronunciation);

    /// <remarks>Allows to force the <see cref="Pronunciation" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? PronunciationSpecifiedOverride { get; set; }

    /// <remarks>See the <see cref="SyllableSpecified" /> and <see cref="SyllableSpecifiedOverride" /> related properties.</remarks>
    [XmlAttribute]
    public string? Syllable { get; set; }

    /// <remarks>Indicates whether the <see cref="Syllable" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool SyllableSpecified => SyllableSpecifiedOverride ?? !string.IsNullOrEmpty(Syllable);

    /// <remarks>Allows to force the <see cref="Syllable" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? SyllableSpecifiedOverride { get; set; }

    /// <remarks>See the <see cref="OnyomiSpecified" /> and <see cref="OnyomiSpecifiedOverride" /> related properties.</remarks>
    [XmlAttribute]
    public string? Onyomi { get; set; }

    /// <remarks>Indicates whether the <see cref="Onyomi" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool OnyomiSpecified => OnyomiSpecifiedOverride ?? !string.IsNullOrEmpty(Onyomi);

    /// <remarks>Allows to force the <see cref="Onyomi" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? OnyomiSpecifiedOverride { get; set; }

    /// <remarks>See the <see cref="KunyomiSpecified" /> and <see cref="KunyomiSpecifiedOverride" /> related properties.</remarks>
    [XmlAttribute]
    public string? Kunyomi { get; set; }

    /// <remarks>Indicates whether the <see cref="Kunyomi" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool KunyomiSpecified => KunyomiSpecifiedOverride ?? !string.IsNullOrEmpty(Kunyomi);

    /// <remarks>Allows to force the <see cref="Kunyomi" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? KunyomiSpecifiedOverride { get; set; }

    /// <remarks>See the <see cref="MeaningSpecified" /> and <see cref="MeaningSpecifiedOverride" /> related properties.</remarks>
    [XmlAttribute]
    public string? Meaning { get; set; }

    /// <remarks>Indicates whether the <see cref="Meaning" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool MeaningSpecified => MeaningSpecifiedOverride ?? !string.IsNullOrEmpty(Meaning);

    /// <remarks>Allows to force the <see cref="Meaning" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? MeaningSpecifiedOverride { get; set; }

    /// <remarks>
    /// <para>See the <see cref="TagsSpecified" /> and <see cref="TagsSpecifiedOverride" /> related properties.</para>
    /// <para>The <see cref="string" /> value contains a comma- or semicolon-separated list of tags.</para>
    /// </remarks>
    [XmlAttribute]
    public string? Tags { get; set; }

    /// <remarks>Indicates whether the <see cref="Tags" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool TagsSpecified => TagsSpecifiedOverride ?? !string.IsNullOrEmpty(Tags);

    /// <remarks>Allows to force the <see cref="Tags" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? TagsSpecifiedOverride { get; set; }

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
