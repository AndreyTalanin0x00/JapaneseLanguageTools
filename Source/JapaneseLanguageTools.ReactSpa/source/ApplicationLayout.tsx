import { Layout, Space, Typography } from "antd";
import { useContext, useMemo } from "react";
import { Navigate, Route, Routes } from "react-router-dom";

import ApplicationMenu from "@/ApplicationMenu";
import ApplicationBreadcrumb from "@/ApplicationBreadcrumb";
import MobileBrowserContext from "@/contexts/MobileBrowserContext";
import type ApplicationBreadcrumbItemDescriptor from "@/entities/application/ApplicationBreadcrumbItemDescriptor";
import type ApplicationMenuItemDescriptor from "@/entities/application/ApplicationMenuItemDescriptor";
import type ApplicationPageDescriptor from "@/entities/application/ApplicationPageDescriptor";

import styles from "./ApplicationLayout.module.css";

export interface ApplicationLayoutProps {
  applicationPageDescriptors: ApplicationPageDescriptor[];
  applicationMenuItemDescriptors: ApplicationMenuItemDescriptor[];
  applicationBreadcrumbItemDescriptors: ApplicationBreadcrumbItemDescriptor[];
}

const ApplicationLayout = ({ applicationPageDescriptors, applicationMenuItemDescriptors, applicationBreadcrumbItemDescriptors }: ApplicationLayoutProps) => {
  const mobileBrowserMode = useContext(MobileBrowserContext);

  const routes = useMemo(() => {
    const pageRoutes = applicationPageDescriptors
      .filter((applicationPageDescriptor) => applicationPageDescriptor.component)
      .map((applicationPageDescriptor) => {
        const { key, path, component } = applicationPageDescriptor;

        const reactNode: React.ReactNode = <>{component}</>;

        return <Route key={key} path={path} element={reactNode} />;
      });

    const homeRedirectRoute = <Route key="root-redirect" path="/" element={<Navigate to="/home" replace />} />;

    return [homeRedirectRoute, ...pageRoutes];
  }, [applicationPageDescriptors]);

  const footerText = useMemo(() => {
    const space = " ";
    const nonBreakingSpace = "\u00A0";
    const lines = ["Copyright © 2024-2026 Andrey Talanin.", "See the Home page for project details."];
    const joinedLines = lines.map((line) => line.replaceAll(space, nonBreakingSpace)).join(space);
    return joinedLines;
  }, []);

  return (
    <Layout className={styles.application}>
      <Layout.Header className={styles.applicationHeader}>
        <Space className={styles.applicationTitle} direction="horizontal">
          <Typography.Text strong className={styles.applicationTitleText}>
            Japanese Language Tools
          </Typography.Text>
        </Space>
        <ApplicationMenu applicationMenuItemDescriptors={applicationMenuItemDescriptors} applicationPageDescriptors={applicationPageDescriptors} />
      </Layout.Header>
      <Layout>
        <Layout className={mobileBrowserMode ? styles.applicationPageWrapperMobile : styles.applicationPageWrapper}>
          <ApplicationBreadcrumb
            className={mobileBrowserMode ? styles.applicationBreadcrumbMobile : styles.applicationBreadcrumb}
            applicationBreadcrumbItemDescriptors={applicationBreadcrumbItemDescriptors}
            applicationPageDescriptors={applicationPageDescriptors}
          />
          <Layout.Content className={styles.applicationPageWrapperContent}>
            <Routes>{routes}</Routes>
          </Layout.Content>
          <Layout.Footer className={styles.applicationFooter}>{footerText}</Layout.Footer>
        </Layout>
      </Layout>
    </Layout>
  );
};

export default ApplicationLayout;
