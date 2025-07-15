import { ExceptionOutlined, HomeOutlined, ToolOutlined } from "@ant-design/icons";

import { isProductionMode } from "@/ApplicationEnvironment";
import type ApplicationBreadcrumbItemDescriptor from "@/entities/application/ApplicationBreadcrumbItemDescriptor";
import type ApplicationMenuItemDescriptor from "@/entities/application/ApplicationMenuItemDescriptor";
import type ApplicationPageDescriptor from "@/entities/application/ApplicationPageDescriptor";
import HomePage from "@/pages/application/HomePage";
import InvalidRoutePage from "@/pages/application/InvalidRoutePage";
import SwaggerRedirectPage from "@/pages/application/SwaggerRedirectPage";

// prettier-ignore
const redirectPageDescriptors: ApplicationPageDescriptor[] = [
  { key: "swagger-redirect-page", path: "/swagger", name: "Swagger API Explorer", icon: <ToolOutlined />, disabled: isProductionMode(), component: <SwaggerRedirectPage /> },
];

// prettier-ignore
const errorPageDescriptors: ApplicationPageDescriptor[] = [
  { key: "invalid-route-page", path: "*", name: "Invalid Route", icon: <ExceptionOutlined />, component: <InvalidRoutePage /> },
];

// prettier-ignore
export const applicationPageDescriptors: ApplicationPageDescriptor[] = [
  { key: "home-page", path: "/home", name: "Home", icon: <HomeOutlined />, component: <HomePage /> },
  // Placeholder pages not existing in the application and routed via redirects instead.
  ...redirectPageDescriptors,
  // Error pages not displayed during normal operation.
  ...errorPageDescriptors,
];

// prettier-ignore
export const applicationMenuItemDescriptors: ApplicationMenuItemDescriptor[] = [
  { key: "home-page", type: "item" },
  { key: "swagger-redirect-page", disabled: isProductionMode(), type: "item" },
];

// prettier-ignore
export const applicationBreadcrumbItemDescriptors: ApplicationBreadcrumbItemDescriptor[] = [
  { key: "home-page", useLink: true },
  { key: "swagger-redirect-page", useLink: true },
  { key: "invalid-route-page", useLink: true },
];
