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

public class TagJsonExportFormatter :
    JsonExportFormatterBase<TagExportRequest, TagExportResponse, TagObjectPackageJsonModel, TagObjectPackageIntegrationModel>
{
    public TagJsonExportFormatter()
    {
    }
}

public class TagXmlExportFormatter :
    XmlExportFormatterBase<TagExportRequest, TagExportResponse, TagObjectPackageXmlModel, TagObjectPackageIntegrationModel>
{
    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly SnapshotObjectAction[] s_snapshotObjectActions = new SnapshotObjectAction[]
    {
        SnapshotObjectAction.None,
        SnapshotObjectAction.Add,
        SnapshotObjectAction.Update,
        SnapshotObjectAction.Remove,
    };

    [SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Personal preference.")]
    private static readonly SnapshotObjectAction[] s_snapshotObjectActionsChangeState = new SnapshotObjectAction[]
    {
        SnapshotObjectAction.None,
    };

    private readonly XCommentFactory m_xCommentFactory;

    public TagXmlExportFormatter(IBlobManager blobManager, IExportSerializer<TagExportRequest, TagExportResponse, TagObjectPackageXmlModel, TagObjectPackageIntegrationModel> exportSerializer, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        : base(blobManager, recyclableMemoryStreamManager)
    {
        m_xCommentFactory = new XCommentFactory(exportSerializer, recyclableMemoryStreamManager);
    }

    /// <inheritdoc />
    protected override Task ApplyDocumentChangesAsync(ApplyDocumentChangesParameters parameters, CancellationToken cancellationToken)
    {
        XDocument xDocument = parameters.XDocument;

        ExportObjectPackageWrapper<TagObjectPackageIntegrationModel> tagObjectPackageIntegrationModelWrapper = parameters.ExportObjectPackageWrapper;
        TagObjectPackageIntegrationModel tagObjectPackageIntegrationModel = tagObjectPackageIntegrationModelWrapper.ExportObjectPackage;

        SnapshotType snapshotType = tagObjectPackageIntegrationModel.SnapshotType;

        SnapshotObjectAction[] snapshotObjectActions = snapshotType == SnapshotType.ChangeState
            ? s_snapshotObjectActionsChangeState
            : s_snapshotObjectActions;

        XElement xRoot = xDocument.Root
            ?? throw new UnreachableException("The document does not contain a root XML element.");

        XElement xTagsElement = xRoot.Element(nameof(TagObjectPackageXmlModel.Tags))
            ?? throw new UnreachableException($"The document does not contain a {nameof(TagObjectPackageXmlModel.Tags)} XML element.");

        xTagsElement.AddFirst(m_xCommentFactory.CreateSnapshotObjectActionsHintComment(snapshotObjectActions));

        xTagsElement.Add(m_xCommentFactory.CreateTagComment());

        return Task.CompletedTask;
    }
}
