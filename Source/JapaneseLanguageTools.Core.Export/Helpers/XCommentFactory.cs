using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Xml;

using Microsoft.IO;

// Disable the IDE0305 (Simplify collection initialization) notification to preserve the LINQ call chain.
#pragma warning disable IDE0305

namespace JapaneseLanguageTools.Core.Export.Helpers;

public class XCommentFactory
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly XName[] s_kanaCharacterXAttributesToRemove = new XName[]
    {
        nameof(CharacterXmlModel.CharacterGroupId),
        nameof(CharacterXmlModel.Onyomi),
        nameof(CharacterXmlModel.Kunyomi),
        nameof(CharacterXmlModel.Meaning),
        nameof(CharacterXmlModel.CreatedOn),
        nameof(CharacterXmlModel.UpdatedOn),
        nameof(CharacterXmlModel.Tags),
    };

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly XName[] s_kanjiTaggedCharacterXAttributesToRemove = new XName[]
    {
        nameof(CharacterXmlModel.CharacterGroupId),
        nameof(CharacterXmlModel.Pronunciation),
        nameof(CharacterXmlModel.Syllable),
        nameof(CharacterXmlModel.CreatedOn),
        nameof(CharacterXmlModel.UpdatedOn),
    };

    private static readonly XName[] s_kanjiCharacterXAttributesToRemove = [.. s_kanjiTaggedCharacterXAttributesToRemove, nameof(CharacterXmlModel.Tags),];

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly XName[] s_characterGroupXAttributesToRemove = new XName[]
    {
        nameof(CharacterGroupXmlModel.CreatedOn),
        nameof(CharacterGroupXmlModel.UpdatedOn),
    };

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly XName[] s_kanaWordXAttributesToRemove = new XName[]
    {
        nameof(WordXmlModel.WordGroupId),
        nameof(WordXmlModel.Furigana),
        nameof(WordXmlModel.CreatedOn),
        nameof(WordXmlModel.UpdatedOn),
        nameof(WordXmlModel.Tags),
    };

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly XName[] s_kanjiTaggedWordXAttributesToRemove = new XName[]
    {
        nameof(WordXmlModel.WordGroupId),
        nameof(WordXmlModel.CreatedOn),
        nameof(WordXmlModel.UpdatedOn),
    };

    private static readonly XName[] s_kanjiWordXAttributesToRemove = [.. s_kanjiTaggedWordXAttributesToRemove, nameof(WordXmlModel.Tags),];

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly XName[] s_wordGroupXAttributesToRemove = new XName[]
    {
        nameof(WordGroupXmlModel.CreatedOn),
        nameof(WordGroupXmlModel.UpdatedOn),
    };

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly XName[] s_tagXAttributesToRemove = new XName[]
    {
        nameof(TagXmlModel.CreatedOn),
        nameof(TagXmlModel.UpdatedOn),
    };

    private static readonly XmlWriterSettings s_xmlWriterSettingsWithIndentation = new()
    {
        Indent = true,
        OmitXmlDeclaration = true,
        CloseOutput = false,
    };
    private static readonly XmlWriterSettings s_xmlWriterSettingsWithoutIndentation = new()
    {
        Indent = false,
        OmitXmlDeclaration = true,
        CloseOutput = false,
    };

    private readonly IExportSerializer m_exportSerializer;
    private readonly RecyclableMemoryStreamManager m_recyclableMemoryStreamManager;

    public XCommentFactory(IExportSerializer exportSerializer, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
    {
        m_exportSerializer = exportSerializer;
        m_recyclableMemoryStreamManager = recyclableMemoryStreamManager;
    }

    public XComment CreateSnapshotObjectActionsHintComment(SnapshotObjectAction[] availableImportActions)
    {
        string[] availableImportActionNames = availableImportActions
            .Select(importAction => importAction.ToString())
            .ToArray();

        XComment xImportActionsHintComment = new(FormatCommentString($"Snapshot Object Actions: {string.Join(", ", availableImportActionNames)}."));

        return xImportActionsHintComment;
    }

    public XComment CreateKanaCharacterComment(CharacterTypes type)
    {
        CharacterXmlModel kanaCharacterXmlModel = new()
        {
            Id = 0,
            Action = SnapshotObjectAction.Add,
            Symbol = string.Empty,
            Type = type,
            Pronunciation = string.Empty,
            PronunciationSpecifiedOverride = true,
            Syllable = string.Empty,
            SyllableSpecifiedOverride = true,
        };

        XElement xKanaCharacterElement = SerializeAsElement(kanaCharacterXmlModel);

        xKanaCharacterElement = RemoveUnwantedAttributes(xKanaCharacterElement, s_kanaCharacterXAttributesToRemove);

        string xKanaCharacterElementString = SerializeAsString(xKanaCharacterElement);

        XComment xKanaCharacterComment = new(FormatTemplateString(xKanaCharacterElementString, "Kana Template"));

        return xKanaCharacterComment;
    }

    public XComment CreateKanjiCharacterComment(bool displayTagsAttribute = false)
    {
        CharacterXmlModel kanjiCharacterXmlModel = new()
        {
            Id = 0,
            Action = SnapshotObjectAction.Add,
            Symbol = string.Empty,
            Type = CharacterTypes.Kanji,
            Onyomi = string.Empty,
            OnyomiSpecifiedOverride = true,
            Kunyomi = string.Empty,
            KunyomiSpecifiedOverride = true,
            Meaning = string.Empty,
            MeaningSpecifiedOverride = true,
            Tags = string.Empty,
            TagsSpecifiedOverride = displayTagsAttribute,
        };

        XElement xKanjiCharacterElement = SerializeAsElement(kanjiCharacterXmlModel);

        xKanjiCharacterElement = displayTagsAttribute
            ? RemoveUnwantedAttributes(xKanjiCharacterElement, s_kanjiTaggedCharacterXAttributesToRemove)
            : RemoveUnwantedAttributes(xKanjiCharacterElement, s_kanjiCharacterXAttributesToRemove);

        string xKanjiCharacterElementString = SerializeAsString(xKanjiCharacterElement);

        XComment xKanjiCharacterComment = new(FormatTemplateString(xKanjiCharacterElementString, "Kanji Template"));

        return xKanjiCharacterComment;
    }

    public XComment CreateCharacterGroupComment()
    {
        CharacterGroupXmlModel characterGroupXmlModel = new()
        {
            Id = 0,
            Action = SnapshotObjectAction.Add,
            Caption = string.Empty,
            Comment = string.Empty,
            Enabled = true,
        };

        XElement xCharacterGroupElement = SerializeAsElement(characterGroupXmlModel);

        xCharacterGroupElement = RemoveUnwantedAttributes(xCharacterGroupElement, s_characterGroupXAttributesToRemove);

        string xCharacterGroupElementString = SerializeAsString(xCharacterGroupElement, resetIndentation: true);

        XComment xCharacterGroupComment = new(FormatTemplateString(xCharacterGroupElementString));

        return xCharacterGroupComment;
    }

    public XComment CreateKanaWordComment(CharacterTypes characterTypes)
    {
        WordXmlModel kanaWordXmlModel = new()
        {
            Id = 0,
            Action = SnapshotObjectAction.Add,
            Characters = string.Empty,
            CharacterTypes = characterTypes,
            Pronunciation = string.Empty,
            PronunciationSpecifiedOverride = true,
            Meaning = string.Empty,
            MeaningSpecifiedOverride = true,
        };

        XElement xKanaWordElement = SerializeAsElement(kanaWordXmlModel);

        xKanaWordElement = RemoveUnwantedAttributes(xKanaWordElement, s_kanaWordXAttributesToRemove);

        string xKanaWordElementString = SerializeAsString(xKanaWordElement);

        XComment xKanaCharacterComment = new(FormatTemplateString(xKanaWordElementString, "Kana Template"));

        return xKanaCharacterComment;
    }

    public XComment CreateKanjiWordComment(bool displayTagsAttribute = false)
    {
        WordXmlModel kanjiWordXmlModel = new()
        {
            Id = 0,
            Action = SnapshotObjectAction.Add,
            Characters = string.Empty,
            CharacterTypes = CharacterTypes.Kanji,
            Pronunciation = string.Empty,
            PronunciationSpecifiedOverride = true,
            Furigana = string.Empty,
            FuriganaSpecifiedOverride = true,
            Okurigana = string.Empty,
            OkuriganaSpecifiedOverride = true,
            Meaning = string.Empty,
            MeaningSpecifiedOverride = true,
            Tags = string.Empty,
            TagsSpecifiedOverride = displayTagsAttribute,
        };

        XElement xKanjiWordElement = SerializeAsElement(kanjiWordXmlModel);

        xKanjiWordElement = displayTagsAttribute
            ? RemoveUnwantedAttributes(xKanjiWordElement, s_kanjiTaggedWordXAttributesToRemove)
            : RemoveUnwantedAttributes(xKanjiWordElement, s_kanjiWordXAttributesToRemove);

        string xKanjiWordElementString = SerializeAsString(xKanjiWordElement);

        XComment xKanjiCharacterComment = new(FormatTemplateString(xKanjiWordElementString, "Kanji Template"));

        return xKanjiCharacterComment;
    }

    public XComment CreateWordGroupComment()
    {
        WordGroupXmlModel wordGroupXmlModel = new()
        {
            Id = 0,
            Action = SnapshotObjectAction.Add,
            Caption = string.Empty,
            Comment = string.Empty,
            Enabled = true,
        };

        XElement xWordGroupElement = SerializeAsElement(wordGroupXmlModel);

        xWordGroupElement = RemoveUnwantedAttributes(xWordGroupElement, s_wordGroupXAttributesToRemove);

        string xWordGroupElementString = SerializeAsString(xWordGroupElement, resetIndentation: true);

        XComment xWordGroupComment = new(FormatTemplateString(xWordGroupElementString));

        return xWordGroupComment;
    }

    public XComment CreateTagComment()
    {
        TagXmlModel tagXmlModel = new()
        {
            Id = 0,
            Action = SnapshotObjectAction.Add,
            Caption = string.Empty,
        };

        XElement xTagElement = SerializeAsElement(tagXmlModel);

        xTagElement = RemoveUnwantedAttributes(xTagElement, s_tagXAttributesToRemove);

        XComment xTagComment = new(FormatTemplateString(xTagElement.ToString()));

        return xTagComment;
    }

    private static XElement RemoveUnwantedAttributes(XElement xElement, params XName[] xAttributeNames)
    {
        XName xSchemaNamespaceXmlAttributeName = XNamespace.Xmlns + "xsi";
        XName xSchemaInstanceNamespaceXmlAttributeName = XNamespace.Xmlns + "xsd";

        foreach (XName xAttributeName in xAttributeNames)
            xElement.Attribute(xAttributeName)?.Remove();

        xElement.Attribute(xSchemaNamespaceXmlAttributeName)?.Remove();
        xElement.Attribute(xSchemaInstanceNamespaceXmlAttributeName)?.Remove();

        return xElement;
    }

    private static string FormatTemplateString(string template, string prefix = "Template")
    {
        return FormatCommentString($"{prefix}: {template}");
    }

    private static string FormatCommentString(string comment)
    {
        return " " + comment + " ";
    }

    private XElement SerializeAsElement<TExportIntermediateObject>(TExportIntermediateObject value)
        where TExportIntermediateObject : class
    {
        XElement xElement;
        using (MemoryStream memoryStream = m_recyclableMemoryStreamManager.GetStream())
        {
            m_exportSerializer.Serialize(memoryStream, value);

            memoryStream.Seek(0L, SeekOrigin.Begin);

            xElement = XElement.Load(memoryStream, LoadOptions.None);
        }

        return xElement;
    }

    private string SerializeAsString(XElement xElement, bool resetIndentation = false)
    {
        string result;
        using (MemoryStream memoryStream = m_recyclableMemoryStreamManager.GetStream())
        {
            XmlWriterSettings xmlWriterSettings = resetIndentation
                ? s_xmlWriterSettingsWithoutIndentation
                : s_xmlWriterSettingsWithIndentation;

            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                xElement.WriteTo(xmlWriter);

            memoryStream.Seek(0L, SeekOrigin.Begin);
            using (StreamReader stringReader = new(memoryStream, leaveOpen: true))
                result = stringReader.ReadToEnd();

            ;
        }

        return result;
    }
}
