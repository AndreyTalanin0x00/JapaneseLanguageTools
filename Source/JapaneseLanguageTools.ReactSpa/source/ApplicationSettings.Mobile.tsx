import { isProductionMode } from "@/ApplicationEnvironment";
import type ApplicationMenuItemDescriptor from "@/entities/application/ApplicationMenuItemDescriptor";

// prettier-ignore
export const applicationMenuItemDescriptors: ApplicationMenuItemDescriptor[] = [
  { key: "home-page", type: "item" },
  { key: "swagger-redirect-page", disabled: isProductionMode(), type: "item" },
];
