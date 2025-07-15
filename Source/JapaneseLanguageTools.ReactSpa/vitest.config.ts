import { resolve } from "path";
import { defineConfig } from "vitest/config";

import react from "@vitejs/plugin-react";

// See https://vitest.dev/config/ for help.
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": resolve(__dirname, "source"),
      "#root": resolve(__dirname),
    },
  },
  test: {
    globals: true,
    environment: "jsdom",
  },
});
