using AndreyTalanin0x00.Integrations.Import;
using AndreyTalanin0x00.Integrations.Import.Exceptions.Factories;
using AndreyTalanin0x00.Integrations.Import.Requests;
using AndreyTalanin0x00.Integrations.Import.Responses;
using AndreyTalanin0x00.Integrations.Import.Services.Abstractions;

using AutoMapper;

namespace JapaneseLanguageTools.Core.Import.Services.Base;

public abstract class ImportMapperBase<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage> :
    IImportMapper<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>
    where TImportRequest : ImportRequest
    where TImportResponse : ImportResponse
    where TImportIntermediateObjectPackageCurrent : class
    where TImportObjectPackage : class
{
    /// <inheritdoc />
    public ImportIntermediateObjectPackageBatch<TImportIntermediateObjectPackageCurrent, TImportObjectPackage> Map(ImportIntermediateObjectPackageBatch<TImportIntermediateObjectPackageCurrent, TImportObjectPackage> importIntermediateObjectPackageBatch)
    {
        int size = importIntermediateObjectPackageBatch.Size;

        ImportSource[] importSources = importIntermediateObjectPackageBatch.ImportSources;

        ImportIntermediateObjectPackageWrapper<TImportIntermediateObjectPackageCurrent>[] importIntermediateObjectPackageWrappers =
            importIntermediateObjectPackageBatch.ImportIntermediateObjectPackageWrappers;
        ImportObjectPackageWrapper<TImportObjectPackage>[] importObjectPackageWrappers =
            importIntermediateObjectPackageBatch.ImportObjectPackageWrappers;

        if (size != importIntermediateObjectPackageWrappers.Length)
            throw ImportExceptionFactory.CreateImportIntermediateObjectPackageWrapperCountMismatchException<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>();
        if (size != importObjectPackageWrappers.Length)
            throw ImportExceptionFactory.CreateImportObjectPackageWrapperCountMismatchException<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>();

        for (int index = 0; index < size; index++)
        {
            ImportSource importSource = importSources[index];

            // The importObjectPackageWrappers array is initialized with null references.
            importObjectPackageWrappers[index] ??= new ImportObjectPackageWrapper<TImportObjectPackage>()
            {
                // The ImportObjectPackage property is set at the end of the current iteration.
                ImportObjectPackage = null!,
            };

            ImportIntermediateObjectPackageWrapper<TImportIntermediateObjectPackageCurrent> importIntermediateObjectPackageWrapper =
                importIntermediateObjectPackageWrappers[index];
            ImportObjectPackageWrapper<TImportObjectPackage> importObjectPackageWrapper =
                importObjectPackageWrappers[index];

            TImportIntermediateObjectPackageCurrent? importIntermediateObjectPackage =
                importIntermediateObjectPackageWrapper.ImportIntermediateObjectPackage;

#pragma warning disable IDE0270 // Use coalesce expression
            if (importIntermediateObjectPackage is null)
                throw ImportExceptionFactory.CreateImportSourceDeserializedAsNullException<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>(importSource);
#pragma warning restore IDE0270 // Use coalesce expression

            TImportObjectPackage? importObjectPackage = Map(importIntermediateObjectPackage);

#pragma warning disable IDE0270 // Use coalesce expression
            if (importObjectPackage is null)
                throw ImportExceptionFactory.CreateImportSourceMappedAsNullException<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>(importSource);
#pragma warning restore IDE0270 // Use coalesce expression

            importObjectPackageWrapper.ImportObjectPackage = importObjectPackage;
        }

        return importIntermediateObjectPackageBatch;
    }

    protected abstract TImportObjectPackage? Map(TImportIntermediateObjectPackageCurrent importIntermediateObjectPackage);
}

public abstract class AutoMapperImportMapperBase<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage> :
    ImportMapperBase<TImportRequest, TImportResponse, TImportIntermediateObjectPackageCurrent, TImportObjectPackage>
    where TImportRequest : ImportRequest
    where TImportResponse : ImportResponse
    where TImportIntermediateObjectPackageCurrent : class
    where TImportObjectPackage : class
{
    private readonly IMapper m_mapper;

    protected AutoMapperImportMapperBase(IMapper mapper)
    {
        m_mapper = mapper;
    }

    /// <inheritdoc />
    protected override TImportObjectPackage? Map(TImportIntermediateObjectPackageCurrent importIntermediateObjectPackage)
    {
        TImportObjectPackage? importObjectPackage = m_mapper.Map<TImportIntermediateObjectPackageCurrent, TImportObjectPackage?>(importIntermediateObjectPackage);

        return importObjectPackage;
    }
}
