using System;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using AndreyTalanin0x00.Integrations.Blobs.Services.Abstractions;

using JapaneseLanguageTools.Contracts.Models.Integrations;
using JapaneseLanguageTools.Contracts.Models.Json;
using JapaneseLanguageTools.Contracts.Models.Xml;
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

public class ApplicationDictionaryJsonExportSerializer :
    JsonExportSerializerBase<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageJsonModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    private readonly TimeProvider m_timeProvider;

    public ApplicationDictionaryJsonExportSerializer(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager, TimeProvider timeProvider)
        : base(blobManager, recyclableMemoryStreamManager)
    {
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    protected override async Task SerializeObjectPackagePairAsync(SerializeObjectPackagePairParameters parameters, CancellationToken cancellationToken)
    {
        Stream stream = parameters.Stream;

        ApplicationDictionaryObjectPackageJsonModel applicationDictionaryObjectPackageJsonModel = parameters.ExportIntermediateObjectPackageCurrent;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        const string applicationDictionaryExportFileNameFormat = "JLT Application Dictionary Export {0}.json";

        string dateTimeString = utcNow.ToString("u")
            .Replace("Z", " " + "UTC")
            .Replace(':', '-');

        parameters.SetFileNameCallback(string.Format(applicationDictionaryExportFileNameFormat, dateTimeString));
        parameters.SetMimeTypeCallback(MediaTypeNames.Application.Json);

        await SerializeAsync(stream, applicationDictionaryObjectPackageJsonModel, cancellationToken);

        // Append the final new line.
#pragma warning disable IDE0063 // Use simple 'using' statement
        // Steams have to be closed immediately to flush the buffer. Simple using statements can be missed later.
        using (StreamWriter streamWriter = new(stream, encoding: Encoding, leaveOpen: true))
            await streamWriter.WriteLineAsync();
#pragma warning restore IDE0063 // Use simple 'using' statement

        return;
    }
}

public class ApplicationDictionaryXmlExportSerializer :
    XmlExportSerializerBase<ApplicationDictionaryExportRequest, ApplicationDictionaryExportResponse, ApplicationDictionaryObjectPackageXmlModel, ApplicationDictionaryObjectPackageIntegrationModel>
{
    private readonly TimeProvider m_timeProvider;

    public ApplicationDictionaryXmlExportSerializer(IBlobManager blobManager, RecyclableMemoryStreamManager recyclableMemoryStreamManager, TimeProvider timeProvider)
        : base(blobManager, recyclableMemoryStreamManager)
    {
        m_timeProvider = timeProvider;
    }

    /// <inheritdoc />
    protected override async Task SerializeObjectPackagePairAsync(SerializeObjectPackagePairParameters parameters, CancellationToken cancellationToken)
    {
        Stream stream = parameters.Stream;

        ApplicationDictionaryObjectPackageXmlModel applicationDictionaryObjectPackageXmlModel = parameters.ExportIntermediateObjectPackageCurrent;

        DateTimeOffset utcNow = m_timeProvider.GetUtcNow();

        const string applicationDictionaryExportFileNameFormat = "JLT Application Dictionary Export {0}.xml";

        string dateTimeString = utcNow.ToString("u")
            .Replace("Z", " " + "UTC")
            .Replace(':', '-');

        parameters.SetFileNameCallback(string.Format(applicationDictionaryExportFileNameFormat, dateTimeString));
        parameters.SetMimeTypeCallback(MediaTypeNames.Application.Xml);

        await SerializeAsync(stream, applicationDictionaryObjectPackageXmlModel, cancellationToken);

        // Append the final new line.
#pragma warning disable IDE0063 // Use simple 'using' statement
        // Steams have to be closed immediately to flush the buffer. Simple using statements can be missed later.
        using (StreamWriter streamWriter = new(stream, encoding: Encoding, leaveOpen: true))
            await streamWriter.WriteLineAsync();
#pragma warning restore IDE0063 // Use simple 'using' statement

        return;
    }
}
