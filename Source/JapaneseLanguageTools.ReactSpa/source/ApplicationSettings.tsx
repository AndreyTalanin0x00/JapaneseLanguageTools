import { BookOutlined, ExceptionOutlined, HomeOutlined, SettingOutlined, TagOutlined, ToolOutlined } from "@ant-design/icons";

import { isProductionMode } from "@/ApplicationEnvironment";
import type ApplicationBreadcrumbItemDescriptor from "@/entities/application/ApplicationBreadcrumbItemDescriptor";
import type ApplicationMenuItemDescriptor from "@/entities/application/ApplicationMenuItemDescriptor";
import type ApplicationPageDescriptor from "@/entities/application/ApplicationPageDescriptor";
import HomePage from "@/pages/application/HomePage";
import InvalidRoutePage from "@/pages/application/InvalidRoutePage";
import SwaggerRedirectPage from "@/pages/application/SwaggerRedirectPage";
import ApplicationDictionaryIntegrationPage from "./pages/integrations/ApplicationDictionaryIntegrationPage";
import TagIntegrationPage from "@/pages/integrations/TagIntegrationPage";
import PreferencesPage from "@/pages/preferences/PreferencesPage";

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
  { key: "application-dictionary-integration-page", path: "/integrations/application-dictionary", name: "Application Dictionary Integrations", icon: <BookOutlined />, component: <ApplicationDictionaryIntegrationPage /> },
  { key: "tag-integration-page", path: "/integrations/tags", name: "Tag Integrations", icon: <TagOutlined />, component: <TagIntegrationPage /> },
  { key: "preferences-page", path: "/preferences", name: "Preferences", icon: <SettingOutlined />, component: <PreferencesPage /> },
  // Placeholder pages not existing in the application and routed via redirects instead.
  ...redirectPageDescriptors,
  // Error pages not displayed during normal operation.
  ...errorPageDescriptors,
];

// prettier-ignore
export const applicationMenuItemDescriptors: ApplicationMenuItemDescriptor[] = [
  { key: "home-page", type: "item" },
  { key: "preferences-page", type: "item" },
  { key: "swagger-redirect-page", disabled: isProductionMode(), type: "item" },
];

// prettier-ignore
export const applicationBreadcrumbItemDescriptors: ApplicationBreadcrumbItemDescriptor[] = [
  { key: "home-page", useLink: true },
  { key: "application-dictionary-integration-page", useLink: true },
  { key: "tag-integration-page", useLink: true },
  { key: "preferences-page", useLink: true },
  { key: "swagger-redirect-page", useLink: true },
  { key: "invalid-route-page", useLink: true },
];
