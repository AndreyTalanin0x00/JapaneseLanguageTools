import { isProductionMode } from "@/ApplicationEnvironment";
import type ApplicationMenuItemDescriptor from "@/entities/application/ApplicationMenuItemDescriptor";

// prettier-ignore
export const applicationMenuItemDescriptors: ApplicationMenuItemDescriptor[] = [
  { key: "home-page", type: "item" },
  { key: "exercises-page", type: "item" },
  { key: "integrations-page", type: "item" },
  { key: "preferences-page", type: "item" },
  { key: "swagger-redirect-page", disabled: isProductionMode(), type: "item" },
];
