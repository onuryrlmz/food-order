FROM node:22-alpine AS deps
WORKDIR /app

# Proje dosyalarını kopyala
COPY front-admin/ ./front-admin/
COPY service/ ./service/

WORKDIR /app/front-admin

# Install dependencies
RUN npm ci --legacy-peer-deps

# service/ dizininin front-admin/node_modules'a erişebilmesi için symlink
RUN ln -s /app/front-admin/node_modules /app/node_modules

ARG NEXT_PUBLIC_API_BASE_URL
ENV NEXT_PUBLIC_API_BASE_URL=${NEXT_PUBLIC_API_BASE_URL}
ENV NEXT_TELEMETRY_DISABLED=1

RUN npm run build

# Production image
FROM node:22-alpine AS runner
WORKDIR /app/front-admin

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

RUN addgroup --system --gid 1001 nodejs && adduser --system --uid 1001 nextjs

COPY --from=deps --chown=nextjs:nodejs /app/ /app/

USER nextjs
EXPOSE 3000
ENV PORT=3000
CMD ["node", "server.js"]
