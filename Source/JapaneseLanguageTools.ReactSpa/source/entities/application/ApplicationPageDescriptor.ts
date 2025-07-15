export default interface ApplicationPageDescriptor {
  key: string;
  path: string;
  name: string;
  icon?: React.ReactNode;
  disabled?: boolean;
  component?: React.ReactNode;
}
