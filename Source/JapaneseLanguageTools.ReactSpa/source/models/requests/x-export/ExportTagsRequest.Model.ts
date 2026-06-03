import SnapshotFileFormat from "@/enumerations/SnapshotFileFormat";
import SnapshotType from "@/enumerations/SnapshotType";
import ExportRequestModel from "@/models/requests/base/ExportRequest.Model";

export default interface ExportTagsRequestModel extends ExportRequestModel {
  snapshotType: SnapshotType;
  snapshotFileFormat: SnapshotFileFormat;
  zeroIdProperties: boolean;
}
