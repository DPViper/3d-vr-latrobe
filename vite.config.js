import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'
import { VitePWA } from 'vite-plugin-pwa'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        react({
            jsxRuntime: 'automatic'
        }),
        VitePWA({
            registerType: 'autoUpdate',
            includeAssets: ['favicon.ico', 'robots.txt', 'apple-touch-icon.png'],
            manifest: {
                name: 'Latrobe 3D VR',
                short_name: 'Latrobe3D',
                description: '3D VR Experience for La Trobe University',
                theme_color: '#ffffff',
                icons: [
                    {
                        src: 'pwa-192x192.png',
                        sizes: '192x192',
                        type: 'image/png'
                    },
                    {
                        src: 'pwa-512x512.png',
                        sizes: '512x512',
                        type: 'image/png'
                    }
                ]
            }
        })
    ],
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
    },
    build: {
        target: 'esnext'
    }
})