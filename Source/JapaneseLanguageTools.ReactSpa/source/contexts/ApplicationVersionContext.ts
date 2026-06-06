import { createContext } from "react";

export interface ApplicationVersionContextValue {
  available: boolean;
  informationalVersion?: string;
  commitDate?: string;
}

export const applicationVersionContextDefaultValue: ApplicationVersionContextValue = {
  available: false,
};

const ApplicationVersionContext = createContext(applicationVersionContextDefaultValue);

export default ApplicationVersionContext;
