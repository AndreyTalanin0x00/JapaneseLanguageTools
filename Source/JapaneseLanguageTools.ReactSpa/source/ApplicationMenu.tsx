import { Menu } from "antd";
import { useCallback, useMemo } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import type ApplicationPageDescriptor from "@/entities/application/ApplicationPageDescriptor";
import type ApplicationMenuItemDescriptor from "@/entities/application/ApplicationMenuItemDescriptor";

const applicationMenuItemLabelPlaceholder = "Not-Yet-Configured Page";

interface ApplicationMenuItem {
  key: string;
  label: React.ReactNode;
  children?: ApplicationMenuItem[];
}

export interface ApplicationMenuProps {
  applicationPageDescriptors: ApplicationPageDescriptor[];
  applicationMenuItemDescriptors: ApplicationMenuItemDescriptor[];
}

const ApplicationMenu = ({ applicationPageDescriptors, applicationMenuItemDescriptors }: ApplicationMenuProps) => {
  const location = useLocation();
  const navigate = useNavigate();

  const path = useMemo(() => location.pathname, [location]);

  const applicationPageDescriptorsByKey = useMemo(() => {
    const applicationPageDescriptorsByKey = new Map<string, ApplicationPageDescriptor>();
    for (const applicationPageDescriptor of applicationPageDescriptors) {
      applicationPageDescriptorsByKey.set(applicationPageDescriptor.key, applicationPageDescriptor);
    }
    return applicationPageDescriptorsByKey;
  }, [applicationPageDescriptors]);

  const getApplicationPageDescriptorByKey = useCallback(
    (key: string) => {
      const applicationPageDescriptor = applicationPageDescriptorsByKey.has(key) ? applicationPageDescriptorsByKey.get(key) : undefined;
      if (!applicationPageDescriptor) {
        console.error(`No application page descriptor exists for the ${key} key.`);
      }
      return applicationPageDescriptor;
    },
    [applicationPageDescriptorsByKey]
  );

  const applicationMenuItems = useMemo(() => {
    const createApplicationMenuItem = ({ key, label: labelOverride, type, items }: ApplicationMenuItemDescriptor) => {
      const applicationPageDescriptor = getApplicationPageDescriptorByKey(key);

      let label: React.ReactNode = labelOverride ?? applicationPageDescriptor?.name ?? applicationMenuItemLabelPlaceholder;
      if (applicationPageDescriptor?.icon) {
        label = (
          <span>
            {applicationPageDescriptor.icon} {label}
          </span>
        );
      }

      let applicationMenuItem: ApplicationMenuItem;
      switch (type) {
        case "item":
          applicationMenuItem = { key: key, label: label };
          break;
        case "menu":
          applicationMenuItem = {
            key: key,
            label: label,
            children: items?.map((childApplicationMenuItemDescriptor) => createApplicationMenuItem(childApplicationMenuItemDescriptor)) ?? [],
          };
          break;
      }

      return applicationMenuItem;
    };

    return applicationMenuItemDescriptors.map((applicationMenuItemDescriptor) => createApplicationMenuItem(applicationMenuItemDescriptor));
  }, [applicationMenuItemDescriptors, getApplicationPageDescriptorByKey]);

  const selectedApplicationMenuItemKeys = useMemo(() => {
    let selectedApplicationMenuItemKey: string | undefined = undefined;

    const checkApplicationMenuItem = ({ key, children }: ApplicationMenuItem) => {
      const applicationPageDescriptor = getApplicationPageDescriptorByKey(key);
      if (applicationPageDescriptor) {
        const pathTerminator = "/";
        if ((path + pathTerminator).startsWith(applicationPageDescriptor.path + pathTerminator)) {
          // The current item belongs to the selected menu branch, but does not necessarily represents the leaf, continue searching.
          selectedApplicationMenuItemKey = applicationPageDescriptor.key;
        }
        if (path === applicationPageDescriptor.path) {
          // The current item represents the leaf, stop searching.
          return true;
        }
      }

      if (children) {
        for (const child of children) {
          if (checkApplicationMenuItem(child)) {
            // The child item contains the matched leaf, stop searching.
            return true;
          }
        }
      }

      // The current item does not contain a matched leaf.
      return false;
    };

    for (const applicationMenuItem of applicationMenuItems) {
      if (checkApplicationMenuItem(applicationMenuItem)) {
        break;
      }
    }

    // The selectedApplicationMenuItemKey variable can be set multiple times from the checkApplicationMenuItem function.
    // eslint-disable-next-line @typescript-eslint/no-unnecessary-condition
    return selectedApplicationMenuItemKey ? [selectedApplicationMenuItemKey] : [];
  }, [path, getApplicationPageDescriptorByKey, applicationMenuItems]);

  const onSelect = useCallback(
    (key: string) => {
      const applicationPageDescriptor = applicationPageDescriptors.find((applicationPageDescriptor) => applicationPageDescriptor.key === key);
      if (applicationPageDescriptor?.path) {
        void navigate(applicationPageDescriptor.path);
      }
    },
    [applicationPageDescriptors, navigate]
  );

  // prettier-ignore
  return (
    <Menu
      mode="horizontal"
      theme="dark"
      items={applicationMenuItems}
      selectedKeys={selectedApplicationMenuItemKeys}
      onSelect={({ key }) => onSelect(key)}
    />
  );
};

export default ApplicationMenu;
