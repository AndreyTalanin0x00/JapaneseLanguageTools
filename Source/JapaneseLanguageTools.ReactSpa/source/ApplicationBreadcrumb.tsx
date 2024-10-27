import { Breadcrumb, BreadcrumbProps } from "antd";
import { useCallback, useMemo } from "react";
import { Link, matchPath, useLocation } from "react-router-dom";

import type ApplicationBreadcrumbItemDescriptor from "@/entities/application/ApplicationBreadcrumbItemDescriptor";
import type ApplicationPageDescriptor from "@/entities/application/ApplicationPageDescriptor";

export interface ApplicationBreadcrumbProps extends BreadcrumbProps {
  applicationPageDescriptors: ApplicationPageDescriptor[];
  applicationBreadcrumbItemDescriptors: ApplicationBreadcrumbItemDescriptor[];
}

const ApplicationBreadcrumb = ({ applicationPageDescriptors, applicationBreadcrumbItemDescriptors, ...breadcrumbProperties }: ApplicationBreadcrumbProps) => {
  const location = useLocation();

  const path = useMemo(() => location.pathname, [location]);

  const applicationBreadcrumbItemDescriptorsByKey = useMemo(() => {
    const applicationBreadcrumbItemDescriptorsByKey = new Map<string, ApplicationBreadcrumbItemDescriptor>();
    for (const applicationBreadcrumbItemDescriptor of applicationBreadcrumbItemDescriptors) {
      applicationBreadcrumbItemDescriptorsByKey.set(applicationBreadcrumbItemDescriptor.key, applicationBreadcrumbItemDescriptor);
    }
    return applicationBreadcrumbItemDescriptorsByKey;
  }, [applicationBreadcrumbItemDescriptors]);

  const getApplicationBreadcrumbItemDescriptorByKey = useCallback(
    (key: string) => {
      const applicationBreadcrumbItemDescriptor = applicationBreadcrumbItemDescriptorsByKey.has(key) ? applicationBreadcrumbItemDescriptorsByKey.get(key) : undefined;
      if (!applicationBreadcrumbItemDescriptor) {
        console.error(`No application breadcrumb item descriptor exists for the ${key} key.`);
      }
      return applicationBreadcrumbItemDescriptor;
    },
    [applicationBreadcrumbItemDescriptorsByKey]
  );

  const breadcrumbItems = useMemo(() => {
    let matchedLeafRoute = false;
    let matchedApplicationPageDescriptors = applicationPageDescriptors.filter((route) => {
      const trimTrailingPathSeparator = (path: string): string => {
        return path.endsWith("/") ? path.substring(0, path.length - 1) : path;
      };

      if (route.path === "*") {
        return false;
      }

      if (matchPath(route.path, path)) {
        matchedLeafRoute ||= true;
        return true;
      } else if (matchPath(`${trimTrailingPathSeparator(route.path)}/*`, path)) {
        return true;
      }

      return false;
    });

    // The matchedLeafRoute variable can be set to true if any leaf application page's route was matched.
    // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
    if (!matchedLeafRoute) {
      matchedApplicationPageDescriptors = [...matchedApplicationPageDescriptors, ...applicationPageDescriptors.filter((route) => route.path === "*")];
    }

    type BreadcrumbItems = BreadcrumbProps["items"];
    const breadcrumbItems: BreadcrumbItems = matchedApplicationPageDescriptors.map((matchedApplicationPageDescriptor, index) => {
      let title: React.ReactNode = matchedApplicationPageDescriptor.name;

      const applicationBreadcrumbItemDescriptor = getApplicationBreadcrumbItemDescriptorByKey(matchedApplicationPageDescriptor.key);

      if (matchedApplicationPageDescriptor.icon) {
        title = (
          <span>
            {matchedApplicationPageDescriptor.icon} {title}
          </span>
        );
      }
      if (applicationBreadcrumbItemDescriptor?.useLink) {
        title = <Link to={matchedApplicationPageDescriptor.path}>{title}</Link>;
      }

      return {
        key: index,
        title: title,
      };
    });

    return breadcrumbItems;
  }, [applicationPageDescriptors, path, getApplicationBreadcrumbItemDescriptorByKey]);

  return <Breadcrumb items={breadcrumbItems} {...breadcrumbProperties} />;
};

export default ApplicationBreadcrumb;
