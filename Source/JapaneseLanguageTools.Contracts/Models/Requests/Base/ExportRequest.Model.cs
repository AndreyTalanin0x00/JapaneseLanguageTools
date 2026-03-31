using JapaneseLanguageTools.Contracts.Enumerations;

namespace JapaneseLanguageTools.Contracts.Models.Requests.Base;

public class ExportRequestModel
{
    public required SnapshotType SnapshotType { get; set; }

    public required SnapshotFileFormat SnapshotFileFormat { get; set; }
}
