import { useMemo } from "react";
import { BrowserRouter } from "react-router-dom";

import ApplicationLayout from "@/ApplicationLayout";
import * as ApplicationSettings from "@/ApplicationSettings";
import * as ApplicationSettingsMobile from "@/ApplicationSettings.Mobile";
import isMobileBrowser from "@/isMobileBrowser";
import MobileBrowserContext from "@/contexts/MobileBrowserContext";

const applicationPageDescriptors = ApplicationSettings.applicationPageDescriptors.filter((descriptor) => !descriptor.disabled);
const applicationMenuItemDescriptors = ApplicationSettings.applicationMenuItemDescriptors.filter((descriptor) => !descriptor.disabled);
const applicationMenuItemDescriptorsMobile = ApplicationSettingsMobile.applicationMenuItemDescriptors.filter((descriptor) => !descriptor.disabled);
const applicationBreadcrumbItemDescriptors = ApplicationSettings.applicationBreadcrumbItemDescriptors;

const Application = () => {
  const mobileBrowserMode = useMemo(() => isMobileBrowser(), []);

  return (
    <BrowserRouter>
      <MobileBrowserContext.Provider value={mobileBrowserMode}>
        <ApplicationLayout
          applicationPageDescriptors={applicationPageDescriptors}
          applicationMenuItemDescriptors={mobileBrowserMode ? applicationMenuItemDescriptorsMobile : applicationMenuItemDescriptors}
          applicationBreadcrumbItemDescriptors={applicationBreadcrumbItemDescriptors}
        />
      </MobileBrowserContext.Provider>
    </BrowserRouter>
  );
};

export default Application;
