import Fastify from "fastify";
import { loadConfig } from "./config.js";
import { ClientRegistry, clientsFile } from "./registry.js";
import { DockerComposeProvisioner } from "./provisioner.js";
import { Catalog } from "./catalog.js";
import { registerRoutes } from "./routes.js";

async function main(): Promise<void> {
  const cfg = loadConfig();
  const app = Fastify({ logger: true });

  const registry = new ClientRegistry(clientsFile(cfg.clientsDir));
  const provisioner = new DockerComposeProvisioner(cfg);
  const catalog = new Catalog(cfg.connectorsDir, cfg.schemaFile);

  await registerRoutes(app, { cfg, registry, provisioner, catalog });

  await app.listen({ port: cfg.port, host: "0.0.0.0" });
  app.log.info(`control plane listening on :${cfg.port}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
