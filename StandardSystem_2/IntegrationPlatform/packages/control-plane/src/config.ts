export interface Config {
  port: number;
  clientComposeFile: string;
  clientsDir: string;
  connectorsDir: string;
  schemaFile: string;
  brandImage: string;
  baseDomain: string;
  /** host port range the provisioner assigns from, one per client */
  appPortStart: number;
}

export function loadConfig(): Config {
  const env = process.env;
  return {
    port: Number(env.CP_PORT ?? 8080),
    clientComposeFile: env.CP_CLIENT_COMPOSE_FILE ?? "../../docker/docker-compose.client.yml",
    clientsDir: env.CP_CLIENTS_DIR ?? "../../clients",
    connectorsDir: env.CP_CONNECTORS_DIR ?? "../../connectors",
    schemaFile: env.CP_SCHEMA_FILE ?? "../../schema/connector-manifest.schema.json",
    brandImage: env.CP_BRAND_IMAGE ?? "yourbrand/automation:latest",
    baseDomain: env.CP_BASE_DOMAIN ?? "platform.example.com",
    appPortStart: Number(env.CP_APP_PORT_START ?? 8200),
  };
}
