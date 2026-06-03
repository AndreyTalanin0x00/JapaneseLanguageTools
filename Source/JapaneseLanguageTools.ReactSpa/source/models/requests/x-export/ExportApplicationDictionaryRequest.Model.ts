import SnapshotFileFormat from "@/enumerations/SnapshotFileFormat";
import SnapshotType from "@/enumerations/SnapshotType";
import ExportRequestModel from "@/models/requests/base/ExportRequest.Model";

export default interface ExportApplicationDictionaryRequestModel extends ExportRequestModel {
  snapshotType: SnapshotType;
  snapshotFileFormat: SnapshotFileFormat;
  zeroIdProperties: boolean;
}
