import { useEffect, useMemo, useState } from "react";
import { BrowserRouter } from "react-router-dom";

import ApplicationLayout from "@/ApplicationLayout";
import * as ApplicationSettings from "@/ApplicationSettings";
import * as ApplicationSettingsMobile from "@/ApplicationSettings.Mobile";
import isMobileBrowser from "@/isMobileBrowser";
import * as ApplicationPreferencesConstants from "@/constants/ApplicationPreferencesConstants";
import * as ObjectConstants from "@/constants/ObjectConstants";
import MobileBrowserContext from "@/contexts/MobileBrowserContext";
import ApplicationPreferencesContext from "@/contexts/ApplicationPreferencesContext";
import ApplicationPreferences from "@/entities/preferences/ApplicationPreferences";

const applicationPageDescriptors = ApplicationSettings.applicationPageDescriptors.filter((descriptor) => !descriptor.disabled);
const applicationMenuItemDescriptors = ApplicationSettings.applicationMenuItemDescriptors.filter((descriptor) => !descriptor.disabled);
const applicationMenuItemDescriptorsMobile = ApplicationSettingsMobile.applicationMenuItemDescriptors.filter((descriptor) => !descriptor.disabled);
const applicationBreadcrumbItemDescriptors = ApplicationSettings.applicationBreadcrumbItemDescriptors;

const applicationPreferencesInitialValues = localStorage.getItem(ApplicationPreferencesConstants.applicationPreferencesLocalStorageKey)
  ? (JSON.parse(localStorage.getItem(ApplicationPreferencesConstants.applicationPreferencesLocalStorageKey) ?? ObjectConstants.emptyObjectString) as ApplicationPreferences)
  : ApplicationPreferencesConstants.applicationPreferencesDefaultValues;

const Application = () => {
  const mobileBrowserMode = useMemo(() => isMobileBrowser(), []);

  const [applicationPreferences, setApplicationPreferences] = useState<ApplicationPreferences>(applicationPreferencesInitialValues);

  useEffect(() => {
    localStorage.setItem(ApplicationPreferencesConstants.applicationPreferencesLocalStorageKey, JSON.stringify(applicationPreferences));
  }, [applicationPreferences]);

  useEffect(() => {
    if (applicationPreferences == ApplicationPreferencesConstants.applicationPreferencesDefaultValues) {
      // If the default object was used, there are no preferences in the local storage; save them once.
      setApplicationPreferences({ ...applicationPreferences });
    }
  }, [applicationPreferences]);

  // prettier-ignore
  return (
    <BrowserRouter>
      <MobileBrowserContext.Provider value={mobileBrowserMode}>
        <ApplicationPreferencesContext.Provider value={{ applicationPreferences, setApplicationPreferences }}>
          <ApplicationLayout
            applicationPageDescriptors={applicationPageDescriptors}
            applicationMenuItemDescriptors={mobileBrowserMode ? applicationMenuItemDescriptorsMobile : applicationMenuItemDescriptors}
            applicationBreadcrumbItemDescriptors={applicationBreadcrumbItemDescriptors}
          />
        </ApplicationPreferencesContext.Provider>
      </MobileBrowserContext.Provider>
    </BrowserRouter>
  );
};

export default Application;
