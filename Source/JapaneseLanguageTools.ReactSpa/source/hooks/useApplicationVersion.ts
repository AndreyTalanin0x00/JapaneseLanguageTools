import { useEffect, useState } from "react";

import { getCommitDate, getInformationalVersion } from "@/api/applicationVersionMethods";
import ApplicationVersion from "@/entities/application/ApplicationVersion";

const useApplicationVersion = () => {
  const [applicationVersion, setApplicationVersion] = useState<ApplicationVersion>();

  useEffect(() => {
    Promise.all([getInformationalVersion(), getCommitDate()])
      .then((promiseResults) => {
        const [informationalVersion, commitDate] = promiseResults;
        setApplicationVersion({ informationalVersion, commitDate });
      })
      .catch((error: unknown) => {
        console.warn("Can not get application version (not available on Production environments).", error);
      });
  }, []);

  return applicationVersion;
};

export default useApplicationVersion;
