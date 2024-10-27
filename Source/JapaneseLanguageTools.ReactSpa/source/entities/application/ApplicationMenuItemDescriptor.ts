export default interface ApplicationMenuItemDescriptor {
  key: string;
  label?: string;
  disabled?: boolean;
  items?: ApplicationMenuItemDescriptor[];
  type: "item" | "menu";
}
