using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;
using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Enumerations;
using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
using JapaneseLanguageTools.Core.Export.Helpers;
using JapaneseLanguageTools.Core.Export.Requests;
using JapaneseLanguageTools.Core.Export.Responses;
using JapaneseLanguageTools.Core.Export.Services.Base;

using Microsoft.IO;

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public class ApplicationDictionaryJsonExportFormatter :
    JsonExportFormatterBase<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    public ApplicationDictionaryJsonExportFormatter()
    {
    }
}

public class ApplicationDictionaryXmlExportFormatter :
    XmlExportFormatterBase<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly SnapshotObjectAction[] s_snapshotObjectActions = new SnapshotObjectAction[]
    {
        SnapshotObjectAction.None,
        SnapshotObjectAction.Add,
        SnapshotObjectAction.Update,
        SnapshotObjectAction.ChangeState,
        SnapshotObjectAction.Remove,
    };

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly SnapshotObjectAction[] s_snapshotObjectActionsChangeState = new SnapshotObjectAction[]
    {
        SnapshotObjectAction.None,
        SnapshotObjectAction.ChangeState,
    };

    private readonly XCommentFactory m_xCommentFactory;

    public ApplicationDictionaryXmlExportFormatter(IBlobManager blobManager, IExportSerializer<TagExportRequest, TagExportResponse, TagObjectPackageXmlModel, TagObjectPackageIntegrationModel> exportSerializer, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        : base(blobManager, recyclableMemoryStreamManager)
    {
        m_xCommentFactory = new XCommentFactory(exportSerializer, recyclableMemoryStreamManager);
    }

    /// <inheritdoc />
    protected override Task ApplyDocumentChangesAsync(ApplyDocumentChangesParameters parameters, CancellationToken cancellationToken)
    {
        XDocument xDocument = parameters.XDocument;

        ExportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel> applicationDictionaryObjectPackageIntegrationModelWrapper = parameters.ExportObjectPackageWrapper;
        ApplicationDictionaryObjectPackageIntegrationModel applicationDictionaryObjectPackageIntegrationModel = applicationDictionaryObjectPackageIntegrationModelWrapper.ExportObjectPackage;

        SnapshotType snapshotType = applicationDictionaryObjectPackageIntegrationModel.SnapshotType;

        SnapshotObjectAction[] snapshotObjectActions = snapshotType == SnapshotType.ChangeState
            ? s_snapshotObjectActionsChangeState
            : s_snapshotObjectActions;

        XElement xRoot = xDocument.Root
            ?? throw new UnreachableException("The document does not contain a root XML element.");

        XElement xApplicationDictionaryElement = xRoot.Element(nameof(ApplicationDictionaryObjectPackageXmlModel.ApplicationDictionary))
            ?? throw new UnreachableException($"The document does not contain an {nameof(ApplicationDictionaryObjectPackageXmlModel.ApplicationDictionary)} XML element.");

        xApplicationDictionaryElement.AddFirst(m_xCommentFactory.CreateSnapshotObjectActionsHintComment(snapshotObjectActions));

        XElement xCharactersElement = xApplicationDictionaryElement.Element(nameof(ApplicationDictionaryXmlModel.Characters))
            ?? throw new UnreachableException($"The document does not contain a {nameof(ApplicationDictionaryXmlModel.Characters)} XML element.");
        XElement xCharacterGroupsElement = xApplicationDictionaryElement.Element(nameof(ApplicationDictionaryXmlModel.CharacterGroups))
            ?? throw new UnreachableException($"The document does not contain a {nameof(ApplicationDictionaryXmlModel.CharacterGroups)} XML element.");

        XElement xWordsElement = xApplicationDictionaryElement.Element(nameof(ApplicationDictionaryXmlModel.Words))
            ?? throw new UnreachableException($"The document does not contain a {nameof(ApplicationDictionaryXmlModel.Words)} XML element.");
        XElement xWordGroupsElement = xApplicationDictionaryElement.Element(nameof(ApplicationDictionaryXmlModel.WordGroups))
            ?? throw new UnreachableException($"The document does not contain a {nameof(ApplicationDictionaryXmlModel.WordGroups)} XML element.");

        XElement xTagsElement = xApplicationDictionaryElement.Element(nameof(ApplicationDictionaryXmlModel.Tags))
            ?? throw new UnreachableException($"The document does not contain a {nameof(ApplicationDictionaryXmlModel.Tags)} XML element.");

        xCharactersElement.Add(m_xCommentFactory.CreateKanaCharacterComment(CharacterTypes.Katakana));
        xCharactersElement.Add(m_xCommentFactory.CreateKanaCharacterComment(CharacterTypes.Hiragana));
        xCharactersElement.Add(m_xCommentFactory.CreateKanjiCharacterComment(displayTagsAttribute: true));
        xCharactersElement.Add(m_xCommentFactory.CreateKanjiCharacterComment());

        xCharacterGroupsElement.Add(m_xCommentFactory.CreateCharacterGroupComment());

        xWordsElement.Add(m_xCommentFactory.CreateKanaWordComment(CharacterTypes.Katakana));
        xWordsElement.Add(m_xCommentFactory.CreateKanaWordComment(CharacterTypes.Hiragana));
        xWordsElement.Add(m_xCommentFactory.CreateKanjiWordComment(displayTagsAttribute: true));
        xWordsElement.Add(m_xCommentFactory.CreateKanjiWordComment());

        xWordGroupsElement.Add(m_xCommentFactory.CreateWordGroupComment());

        xTagsElement.Add(m_xCommentFactory.CreateTagComment());

        return Task.CompletedTask;
    }
}
