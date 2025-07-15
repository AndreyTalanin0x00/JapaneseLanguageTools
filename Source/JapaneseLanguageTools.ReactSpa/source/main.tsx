import * as React from "react";
import * as ReactDOM from "react-dom/client";

import Application from "@/Application";

import "antd/dist/reset.css";

import "@/main.css";

const root = document.getElementById("root");

if (root) {
  const application = <Application />;
  const strictModeApplicationWrapper = <React.StrictMode>{application}</React.StrictMode>;
  ReactDOM.createRoot(root).render(strictModeApplicationWrapper);
} else {
  console.error("Unable to find an element by the 'root' ID.");
}
