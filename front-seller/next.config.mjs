import { resolve, dirname } from 'path';
import { fileURLToPath } from 'url';
import { readFileSync, existsSync } from 'fs';

// Local dev: root .env'den NEXT_PUBLIC_ değişkenlerini oku
// Docker: env zaten ARG/ENV ile geçirilir, bu blok atlanır
const __dirname = dirname(fileURLToPath(import.meta.url));
const rootEnv = resolve(__dirname, '..', '.env');
if (existsSync(rootEnv) && !process.env.NEXT_PUBLIC_API_BASE_URL) {
  const lines = readFileSync(rootEnv, 'utf-8').split('\n');
  for (const line of lines) {
    const match = line.match(/^(NEXT_PUBLIC_\w+)=['"]*([^'"\n]*)['"]*$/);
    if (match) process.env[match[1]] = match[2];
  }
}

/** @type {import('next').NextConfig} */
const nextConfig = {
  output: 'standalone',
  reactCompiler: true,
};

export default nextConfig;
