using System;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using FluentValidation;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Core.FluentValidation;
using JapaneseLanguageTools.Core.Import.Requests;
using JapaneseLanguageTools.Core.Import.Responses;

// Disable the IDE0049 (Simplify names) notification to make the System.Object type not stand out.
#pragma warning disable IDE0049

// Use the IDE0079 (Remove unnecessary suppression) suppression (a Visual Studio false positive).
// Disable the IDE0130 (Namespace does not match folder structure) notification to preserve namespace structure.
#pragma warning disable IDE0079
#pragma warning disable IDE0130
#pragma warning restore IDE0079

namespace JapaneseLanguageTools.Core.Import.Services;

public class ApplicationDictionaryImportValidator :
    IImportValidator<ApplicationDictionaryImportRequest, ApplicationDictionaryImportResponse, Object, ApplicationDictionaryObjectPackageIntegrationModel>
{
    /// <inheritdoc />
    public Task ValidateAsync(ImportObjectPackageBatch<Object, ApplicationDictionaryObjectPackageIntegrationModel>[] importObjectPackageBatches, CancellationToken cancellationToken = default)
    {
        ApplicationDictionaryObjectPackageIntegrationModelValidator applicationDictionaryObjectPackageIntegrationModelValidator = new();

        foreach (ImportObjectPackageBatch<Object, ApplicationDictionaryObjectPackageIntegrationModel> importObjectPackageBatch in importObjectPackageBatches)
        {
            ImportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel>[] importObjectPackageWrappers = importObjectPackageBatch.ImportObjectPackageWrappers;

            foreach (ImportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel> importObjectPackageWrapper in importObjectPackageWrappers)
            {
                ApplicationDictionaryObjectPackageIntegrationModel applicationDictionaryObjectPackageIntegrationModel = importObjectPackageWrapper.ImportObjectPackage;

                applicationDictionaryObjectPackageIntegrationModelValidator.ValidateAndThrow(applicationDictionaryObjectPackageIntegrationModel);
            }
        }

        return Task.CompletedTask;
    }
}
