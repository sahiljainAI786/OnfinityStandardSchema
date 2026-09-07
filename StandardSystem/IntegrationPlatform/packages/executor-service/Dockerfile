# Tiny executor image — just Node + the single bundled file. Built from the esbuild output.
FROM node:22-slim
WORKDIR /app
COPY dist/executor-service.mjs ./
ENV EXECUTOR_HOST=0.0.0.0 \
    EXECUTOR_PORT=3070
EXPOSE 3070
CMD ["node", "executor-service.mjs"]
