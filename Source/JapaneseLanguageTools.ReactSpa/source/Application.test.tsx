import { expect, test } from "vitest";

import { render, screen } from "@testing-library/react";

import Application from "@/Application";

test("Renders 'Japanese Language Tools' header title", async () => {
  render(<Application />);
  const elements = await screen.findAllByText(new RegExp(/Japanese\sLanguage\sTools/));
  const titleElement = elements
    .filter((element) => element.tagName == "strong".toUpperCase())
    .filter((element) => element.parentElement)
    .find((element) =>
      Array.from(element.parentElement?.classList.entries() ?? [])
        .map(([, value]) => value)
        .find((className) => className.startsWith("_applicationTitleText"))
    );
  expect(titleElement).toBeDefined();
});

test("Renders 'Copyright © 2024 Andrey Talanin' footer paragraph", async () => {
  render(<Application />);
  // RegExp is required here because some of the whitespaces below get programmatically replaced with non-breaking ones.
  const elements = await screen.findAllByText(new RegExp(/Copyright\s©\s2024\sAndrey\sTalanin.\sSee\sthe\sHome\spage\sfor\sproject\sdetails./));
  const titleElement = elements.find((element) =>
    Array.from(element.classList.entries())
      .map(([, value]) => value)
      .find((className) => className.startsWith("_applicationFooter"))
  );
  expect(titleElement).toBeDefined();
});
