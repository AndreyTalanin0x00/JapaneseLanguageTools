import { createContext } from "react";

import ApplicationPreferences from "@/entities/preferences/ApplicationPreferences";

export interface ApplicationPreferencesContextValue {
  applicationPreferences: ApplicationPreferences;
  setApplicationPreferences: (applicationPreferences: ApplicationPreferences) => void;
}

export const applicationPreferencesContextDefaultValue: ApplicationPreferencesContextValue = {
  applicationPreferences: {
    characterExercisePreferences: undefined,
    wordExercisePreferences: undefined,
  },
  setApplicationPreferences: () => void 0,
};

const ApplicationPreferencesContext = createContext(applicationPreferencesContextDefaultValue);

export default ApplicationPreferencesContext;
