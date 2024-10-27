import { resolve } from "path";
import { defineConfig } from "vite";

import react from "@vitejs/plugin-react";

// prettier-ignore
const port = process.env.PORT !== undefined && !Number.isNaN(parseInt(process.env.PORT))
  ? parseInt(process.env.PORT)
  : 5238;
const strictPort = process.env.STRICT_PORT !== undefined && String(process.env.STRICT_PORT).toLowerCase() == "true";
const open = process.env.OPEN !== undefined && String(process.env.OPEN).toLowerCase() == "true";

// See https://vitejs.dev/config/ for help.
export default defineConfig({
  assetsInclude: ["public/**/*"],
  build: {
    outDir: "build",
  },
  plugins: [react()],
  resolve: {
    alias: {
      "@": resolve(__dirname, "source"),
      "#root": resolve(__dirname),
    },
  },
  server: {
    port: port,
    strictPort: strictPort,
    open: open,
  },
});
