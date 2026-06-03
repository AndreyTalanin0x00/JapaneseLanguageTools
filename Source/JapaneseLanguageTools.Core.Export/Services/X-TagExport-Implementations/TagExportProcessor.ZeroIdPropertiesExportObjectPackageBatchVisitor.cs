using System;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Export;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Core.Export.Requests;

// Disable the IDE0049 (Simplify names) notification to make the System.Object type not stand out.
#pragma warning disable IDE0049

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Export.Services;

public partial class TagExportProcessor
{
    public class ZeroIdPropertiesExportObjectPackageBatchVisitor : IExportObjectPackageBatchVisitor
    {
        /// <inheritdoc />
        public Task VisitAsync(ExportObjectPackageBatch<Object, TagObjectPackageIntegrationModel> exportObjectPackageBatch, TagExportRequest tagExportRequest, CancellationToken cancellationToken = default)
        {
            if (!tagExportRequest.ZeroIdProperties)
                return Task.CompletedTask;

            foreach (ExportObjectPackageWrapper<TagObjectPackageIntegrationModel> exportObjectPackageWrapper in exportObjectPackageBatch.ExportObjectPackageWrappers)
            {
                TagObjectPackageIntegrationModel tagObjectPackageIntegrationModel = exportObjectPackageWrapper.ExportObjectPackage;

                foreach (TagIntegrationModel tagIntegrationModel in tagObjectPackageIntegrationModel.Tags)
                {
                    tagIntegrationModel.Id = default(int);
                }
            }

            return Task.CompletedTask;
        }
    }
}
