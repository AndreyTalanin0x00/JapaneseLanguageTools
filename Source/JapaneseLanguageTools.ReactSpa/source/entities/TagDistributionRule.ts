export default interface TagDistributionRule {
  tagCaption: string;
  exerciseBatchFraction: number;
  maxInclusions?: number;
  minInclusions?: number;
}
