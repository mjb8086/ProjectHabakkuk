import { defineConfig } from 'vite';
import { resolve } from 'path';
import vue from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  build: {
    manifest: true,
    rollupOptions: {
      input: {
        mynd: resolve(`${__dirname}/src/`, 'mynd.js'),
        client: resolve(`${__dirname}/src/`, 'client.js'),
      }
    },
  },
})

console.warn('DIRNAME IS: ' + __dirname);
