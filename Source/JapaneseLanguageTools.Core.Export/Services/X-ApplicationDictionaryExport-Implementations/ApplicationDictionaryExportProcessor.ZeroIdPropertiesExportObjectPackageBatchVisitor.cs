using System;
using System.Collections.Generic;
using System.Linq;
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

public partial class ApplicationDictionaryExportProcessor
{
    public class ZeroIdPropertiesExportObjectPackageBatchVisitor : IExportObjectPackageBatchVisitor
    {
        public Task VisitAsync(ExportObjectPackageBatch<Object, ApplicationDictionaryObjectPackageIntegrationModel> exportObjectPackageBatch, ApplicationDictionaryExportRequest applicationDictionaryExportRequest, CancellationToken cancellationToken = default)
        {
            if (!applicationDictionaryExportRequest.ZeroIdProperties)
                return Task.CompletedTask;

            foreach (ExportObjectPackageWrapper<ApplicationDictionaryObjectPackageIntegrationModel> exportObjectPackageWrapper in exportObjectPackageBatch.ExportObjectPackageWrappers)
            {
                ApplicationDictionaryObjectPackageIntegrationModel applicationDictionaryObjectPackageIntegrationModel = exportObjectPackageWrapper.ExportObjectPackage;

                ApplicationDictionaryIntegrationModel applicationDictionaryIntegrationModel = applicationDictionaryObjectPackageIntegrationModel.ApplicationDictionary;

                IEnumerable<CharacterIntegrationModel> characterIntegrationModels = applicationDictionaryIntegrationModel.Characters
                    .Concat(applicationDictionaryIntegrationModel.CharacterGroups.SelectMany(x => x.Characters));

                foreach (CharacterIntegrationModel characterIntegrationModel in characterIntegrationModels)
                {
                    characterIntegrationModel.Id = default(int);
                    characterIntegrationModel.CharacterGroupId = characterIntegrationModel.CharacterGroupId.HasValue
                        ? default(int)
                        : null;

                    ;
                }

                foreach (CharacterGroupIntegrationModel characterGroupIntegrationModel in applicationDictionaryIntegrationModel.CharacterGroups)
                {
                    characterGroupIntegrationModel.Id = default(int);

                    foreach (CharacterGroupHierarchyRecordIntegrationModel characterGroupHierarchyRecordIntegrationModel in characterGroupIntegrationModel.CharacterGroupHierarchyRecords)
                    {
                        characterGroupHierarchyRecordIntegrationModel.CharacterGroupId = default(int);
                        characterGroupHierarchyRecordIntegrationModel.NestedCharacterGroupId = default(int);
                    }

                    // foreach (CharacterIntegrationModel characterIntegrationModel in characterGroupIntegrationModel.Characters) { }
                    // These objects have been already processed above.
                }

                IEnumerable<WordIntegrationModel> wordIntegrationModels = applicationDictionaryIntegrationModel.Words
                    .Concat(applicationDictionaryIntegrationModel.WordGroups.SelectMany(x => x.Words));

                foreach (WordIntegrationModel wordIntegrationModel in wordIntegrationModels)
                {
                    wordIntegrationModel.Id = default(int);
                    wordIntegrationModel.WordGroupId = wordIntegrationModel.WordGroupId.HasValue
                        ? default(int)
                        : null;

                    ;
                }

                foreach (WordGroupIntegrationModel wordGroupIntegrationModel in applicationDictionaryIntegrationModel.WordGroups)
                {
                    wordGroupIntegrationModel.Id = default(int);

                    foreach (WordGroupHierarchyRecordIntegrationModel wordGroupHierarchyRecordIntegrationModel in wordGroupIntegrationModel.WordGroupHierarchyRecords)
                    {
                        wordGroupHierarchyRecordIntegrationModel.WordGroupId = default(int);
                        wordGroupHierarchyRecordIntegrationModel.NestedWordGroupId = default(int);
                    }

                    // foreach (WordIntegrationModel wordIntegrationModel in wordGroupIntegrationModel.Words) { }
                    // These objects have been already processed above.
                }

                foreach (TagIntegrationModel tagIntegrationModel in applicationDictionaryIntegrationModel.Tags)
                {
                    tagIntegrationModel.Id = default(int);
                }
            }

            return Task.CompletedTask;
        }
    }
}
