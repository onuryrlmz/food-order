FROM node:22-alpine AS base
WORKDIR /app

FROM base AS deps
COPY front-seller/package.json front-seller/package-lock.json* ./front-seller/
RUN cd front-seller && npm ci --legacy-peer-deps

FROM base AS builder
COPY front-seller/ ./front-seller/
COPY service/ ./service/
COPY --from=deps /app/front-seller/node_modules ./front-seller/node_modules
ARG NEXT_PUBLIC_API_BASE_URL
ENV NEXT_PUBLIC_API_BASE_URL=${NEXT_PUBLIC_API_BASE_URL}
WORKDIR /app/front-seller
RUN npm run build

FROM base AS runner
ENV NODE_ENV=production
RUN addgroup --system --gid 1001 nodejs && adduser --system --uid 1001 nextjs
COPY --from=builder /app/front-seller/public ./public
COPY --from=builder --chown=nextjs:nodejs /app/front-seller/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/front-seller/.next/static ./.next/static
USER nextjs
EXPOSE 3000
ENV PORT=3000
CMD ["node", "server.js"]
