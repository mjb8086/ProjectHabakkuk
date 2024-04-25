import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import VueDevTools from 'vite-plugin-vue-devtools'

// https://vitejs.dev/config/
export default defineConfig({
  root: 'src',
  build: {
    outDir: '../../wwwroot/dist',
    assetsDir: '',
    emptyOutDir: true,
    manifest: true,
    rollupOptions: {
      input: {
        mynd: './src/mynd.js',
        client: './src/client.js'
      }
    },
  },
  server: {
    port: 5173,
    strictPort: true,
    hmr: {
      clientPort: 5173
    }
  },
  plugins: [
    vue(),
    VueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})
