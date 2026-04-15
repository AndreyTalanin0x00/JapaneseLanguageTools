import SnapshotFileFormat from "@/enumerations/SnapshotFileFormat";
import SnapshotType from "@/enumerations/SnapshotType";

export default interface ExportRequestModel {
  snapshotType: SnapshotType;
  snapshotFileFormat: SnapshotFileFormat;
}
