import { defineConfig } from "vite";
import * as path from "path";

export default defineConfig({
    build: {
        outDir: path.resolve(__dirname, "../wwwroot/clientapp"),
        emptyOutDir: true,
        sourcemap: false,
        assetsDir: "",
        cssCodeSplit: false,
        rollupOptions: {
            input: {
                "fv-bg-entry": path.resolve(__dirname, "src/fv-bg-entry.tsx"),
                "bowl-spill-entry": path.resolve(__dirname, "src/bowl-spill-entry.tsx"),
            },
            output: {
                entryFileNames: "[name].js",
                chunkFileNames: "chunks/[name].js",
                assetFileNames: "[name][extname]"
            }
        }
    },
    resolve: {
        alias: { "@": path.resolve(__dirname, "src") }
    }
});