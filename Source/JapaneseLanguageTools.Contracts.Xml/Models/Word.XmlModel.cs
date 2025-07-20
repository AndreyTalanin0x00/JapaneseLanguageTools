using System;
using System.Xml.Serialization;

using JapaneseLanguageTools.Contracts.Enumerations;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Contracts.Models.Xml;

[XmlRoot(WordXmlElementName)]
public class WordXmlModel
{
    public const string WordXmlElementName = "Word";

    /// <remarks>See the <see cref="IdSpecified" /> related property.</remarks>
    [XmlAttribute]
    public int Id { get; set; }

    /// <remarks>Indicates whether the <see cref="Id" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool IdSpecified => Id > 0;

    [XmlAttribute]
    public SnapshotObjectAction Action { get; set; }

    /// <remarks>See the <see cref="WordGroupIdString" /> and <see cref="WordGroupIdStringSpecified" /> related properties.</remarks>
    [XmlIgnore]
    public int? WordGroupId { get; set; }

    /// <remarks>Provides a serializable string representation for the <see cref="Nullable{T}" /> value of the <see cref="WordGroupId" /> property.</remarks>
    [XmlAttribute(nameof(WordGroupId))]
    public string? WordGroupIdString
    {
        get => WordGroupId.ToString();
        set => WordGroupId = !string.IsNullOrEmpty(value) ? int.Parse(value) : null;
    }

    /// <remarks>Indicates whether the <see cref="WordGroupIdString" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool WordGroupIdStringSpecified => WordGroupId is not null && WordGroupId > 0;

    [XmlAttribute]
    public string Characters { get; set; } = string.Empty;

    /// <remarks>See the <see cref="CharacterTypesString" /> related property.</remarks>
    [XmlIgnore]
    public CharacterTypes CharacterTypes { get; set; }

    /// <remarks>The <see cref="string" /> value contains a comma- or semicolon-separated list of enum values.</remarks>
    [XmlAttribute(nameof(CharacterTypes))]
    public string CharacterTypesString
    {
        get => CharacterTypes.ToString();
        set => CharacterTypes = !string.IsNullOrEmpty(value) ? Enum.Parse<CharacterTypes>(value.Replace(';', ',')) : CharacterTypes.None;
    }

    /// <remarks>See the <see cref="PronunciationSpecified" /> and <see cref="PronunciationSpecifiedOverride" /> related properties.</remarks>
    [XmlAttribute]
    public string? Pronunciation { get; set; }

    /// <remarks>Indicates whether the <see cref="Pronunciation" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool PronunciationSpecified => PronunciationSpecifiedOverride ?? !string.IsNullOrEmpty(Pronunciation);

    /// <remarks>Allows to force the <see cref="Pronunciation" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? PronunciationSpecifiedOverride { get; set; }

    /// <remarks>See the <see cref="FuriganaSpecified" /> and <see cref="FuriganaSpecifiedOverride" /> related properties.</remarks>
    [XmlAttribute]
    public string? Furigana { get; set; }

    /// <remarks>Indicates whether the <see cref="Furigana" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool FuriganaSpecified => FuriganaSpecifiedOverride ?? !string.IsNullOrEmpty(Furigana);

    /// <remarks>Allows to force the <see cref="Furigana" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? FuriganaSpecifiedOverride { get; set; }

    /// <remarks>See the <see cref="OkuriganaSpecified" /> and <see cref="OkuriganaSpecifiedOverride" /> related properties.</remarks>
    [XmlAttribute]
    public string? Okurigana { get; set; }

    /// <remarks>Indicates whether the <see cref="Okurigana" /> property will be serialized as an XML attribute or omitted from the document.</remarks>
    [XmlIgnore]
    public bool OkuriganaSpecified => OkuriganaSpecifiedOverride ?? !string.IsNullOrEmpty(Okurigana);

    /// <remarks>Allows to force the <see cref="Okurigana" /> property to be serialized as an XML attribute.</remarks>
    [XmlIgnore]
    public bool? OkuriganaSpecifiedOverride { get; set; }

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
