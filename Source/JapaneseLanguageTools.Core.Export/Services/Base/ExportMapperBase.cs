using AndreyTalanin0x00.Integrations.Export;
using AndreyTalanin0x00.Integrations.Export.Exceptions.Factories;
using AndreyTalanin0x00.Integrations.Export.Requests;
using AndreyTalanin0x00.Integrations.Export.Responses;
using AndreyTalanin0x00.Integrations.Export.Services.Abstractions;

using AutoMapper;

namespace JapaneseLanguageTools.Core.Export.Services.Base;

public abstract class ExportMapperBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage> :
    IExportMapper<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
    where TExportIntermediateObjectPackageCurrent : class
    where TExportObjectPackage : class
{
    /// <inheritdoc />
    public ExportIntermediateObjectPackageBatch<TExportIntermediateObjectPackageCurrent, TExportObjectPackage> Map(ExportIntermediateObjectPackageBatch<TExportIntermediateObjectPackageCurrent, TExportObjectPackage> exportIntermediateObjectPackageBatch)
    {
        int size = exportIntermediateObjectPackageBatch.Size;

        ExportObjectPackageWrapper<TExportObjectPackage>[] exportObjectPackageWrappers =
            exportIntermediateObjectPackageBatch.ExportObjectPackageWrappers;
        ExportIntermediateObjectPackageWrapper<TExportIntermediateObjectPackageCurrent>[] exportIntermediateObjectPackageWrappers =
            exportIntermediateObjectPackageBatch.ExportIntermediateObjectPackageWrappers;

        if (size != exportObjectPackageWrappers.Length)
            throw ExportExceptionFactory.CreateExportObjectPackageWrapperCountMismatchException<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>();
        if (size != exportIntermediateObjectPackageWrappers.Length)
            throw ExportExceptionFactory.CreateExportIntermediateObjectPackageWrapperCountMismatchException<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>();

        for (int index = 0; index < size; index++)
        {
            ExportObjectPackageWrapper<TExportObjectPackage> exportObjectPackageWrapper = exportObjectPackageWrappers[index];

            // The exportIntermediateObjectPackageWrappers array is initialized with null references.
            exportIntermediateObjectPackageWrappers[index] ??= new ExportIntermediateObjectPackageWrapper<TExportIntermediateObjectPackageCurrent>()
            {
                // The ExportIntermediateObjectPackage property is set at the end of the current iteration.
                ExportIntermediateObjectPackage = null!,
            };

            ExportIntermediateObjectPackageWrapper<TExportIntermediateObjectPackageCurrent> exportIntermediateObjectPackageWrapper =
                exportIntermediateObjectPackageWrappers[index];

            TExportObjectPackage exportObjectPackage = exportObjectPackageWrapper.ExportObjectPackage;

            TExportIntermediateObjectPackageCurrent exportIntermediateObjectPackage = Map(exportObjectPackage);

            exportIntermediateObjectPackageWrapper.ExportIntermediateObjectPackage = exportIntermediateObjectPackage;
        }

        return exportIntermediateObjectPackageBatch;
    }

    protected abstract TExportIntermediateObjectPackageCurrent Map(TExportObjectPackage exportObjectPackage);
}

public abstract class AutoMapperExportMapperBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage> :
    ExportMapperBase<TExportRequest, TExportResponse, TExportIntermediateObjectPackageCurrent, TExportObjectPackage>
    where TExportRequest : ExportRequest
    where TExportResponse : ExportResponse
    where TExportIntermediateObjectPackageCurrent : class
    where TExportObjectPackage : class
{
    private readonly IMapper m_mapper;

    protected AutoMapperExportMapperBase(IMapper mapper)
    {
        m_mapper = mapper;
    }

    /// <inheritdoc />
    protected override TExportIntermediateObjectPackageCurrent Map(TExportObjectPackage exportObjectPackage)
    {
        TExportIntermediateObjectPackageCurrent exportIntermediateObjectPackage =
            m_mapper.Map<TExportObjectPackage, TExportIntermediateObjectPackageCurrent>(exportObjectPackage);

        return exportIntermediateObjectPackage;
    }
}
