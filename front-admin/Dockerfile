FROM node:22-alpine AS deps
WORKDIR /app

COPY front-admin/ ./front-admin/
COPY service/ ./service/

WORKDIR /app/front-admin

RUN npm ci --legacy-peer-deps
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

# standalone output — outputFileTracingRoot parent'a ayarlı olduğunda
# server.js .next/standalone/front-admin/ altında olur
COPY --from=deps --chown=nextjs:nodejs /app/front-admin/public ./public
COPY --from=deps --chown=nextjs:nodejs /app/front-admin/.next/standalone/ /app/
COPY --from=deps --chown=nextjs:nodejs /app/front-admin/.next/static ./.next/static

USER nextjs
EXPOSE 3000
ENV PORT=3000
CMD ["node", "server.js"]
