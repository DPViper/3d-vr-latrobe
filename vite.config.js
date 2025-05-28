import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'
import { VitePWA } from 'vite-plugin-pwa';
//import { path } from 'express/lib/application'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [VitePWA()],
    server: {
        proxy: {
            '/api': {
                target: 'http://localhost:3001',  // Express backend on the VM
                changeOrigin: true,               // Handles potential CORS issues
            },
        },
    },
    resolve: {
        alias: {
            '@': path.resolve(__dirname, 'src'),
            '@images': path.resolve(__dirname, 'src/images'),
            '@fonts': path.resolve(__dirname, 'public/fonts'),
            '@splats': path.resolve(__dirname, 'public/splats'),
            '@components': path.resolve(__dirname, 'src/components')
        }
    }
})