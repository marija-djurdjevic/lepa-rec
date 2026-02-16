import { defineConfig } from 'vitest/config';
import angular from '@angular/build:vitest';

export default defineConfig({
  plugins: [angular()],
  test: {
    globals: true,
    environment: 'jsdom',
    include: ['src/**/*.spec.ts'],
  },
});
