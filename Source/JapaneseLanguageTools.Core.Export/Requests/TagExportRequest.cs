using AndreyTalanin0x00.Integrations.Export.Requests;

using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Core.Export.Requests;

public class TagExportRequest : ExportRequest
{
    public SnapshotType SnapshotType { get; set; }

    public SnapshotFileFormat SnapshotFileFormat { get; set; }
}
