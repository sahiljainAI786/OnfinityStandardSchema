# Build the control plane from the workspace root (context: .. in the compose file).
FROM node:20-slim AS build
WORKDIR /app
RUN corepack enable
COPY package.json pnpm-workspace.yaml ./
COPY packages/shared/package.json packages/shared/
COPY packages/control-plane/package.json packages/control-plane/
RUN pnpm install --frozen-lockfile || pnpm install
COPY packages/shared packages/shared
COPY packages/control-plane packages/control-plane
RUN pnpm --filter @ip/shared build && pnpm --filter @ip/control-plane build

FROM node:20-slim
WORKDIR /app
# docker CLI so the compose provisioner can drive the host daemon (dev). Drop in k8s deployments.
RUN apt-get update && apt-get install -y --no-install-recommends docker.io && rm -rf /var/lib/apt/lists/*
COPY --from=build /app /app
ENV CP_PORT=8080
EXPOSE 8080
CMD ["node", "packages/control-plane/dist/index.js"]
