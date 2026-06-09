import { fileURLToPath } from 'node:url'
import { mergeConfig, defineConfig, configDefaults } from 'vitest/config'
import viteConfig from './vite.config'

export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      environment: 'jsdom',
      exclude: [...configDefaults.exclude, 'e2e/*'],
      root: fileURLToPath(new URL('./', import.meta.url)),
      coverage: {
        provider: 'v8',
        reporter: ['text', 'json', 'html'],
        exclude: [
          'node_modules/**',
          'dist/**',
          '**/*.spec.js',
          '**/*.config.js',
          'src/main.js',
          'src/assets/**',
          'src/layouts/MainLayout.vue',
          'src/components/home/**',
          'src/components/secoes/**',
          'src/views/HomeView.vue',
          'src/views/SystemCampaignsView.vue',
          'src/views/SystemContractsView.vue',
          'src/views/SystemHomeVIew.vue',
          'src/views/SystemOrganizationView.vue',
          'src/views/SystemProfileView.vue'
        ]
      }
    }
  })
)
