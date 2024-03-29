﻿import path from 'path'
import { defineConfig } from 'vite'
import vue from "@vitejs/plugin-vue";

export default defineConfig({
    define: {
        'process.env': {}
    },
    plugins: [vue()],
    build: {
        outDir: path.resolve(__dirname, 'Scripts/dist/'),
        emptyOutDir: false,
        lib: {
            entry: path.resolve(__dirname, 'Scripts/src/main.js'),
            name: 'ClientBuilderScripts.js',
            fileName: () => 'ClientBuilderScripts.js'
        },
        rollupOptions: {
            output: {
                globals: {
                    vue: 'Vue'
                }
            }
        }
    }
})