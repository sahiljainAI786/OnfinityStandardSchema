import type { FastifyInstance } from "fastify";
import type { Client, CreateClientInput } from "@ip/shared";
import type { Config } from "./config.js";
import { ClientRegistry } from "./registry.js";
import type { Provisioner } from "./provisioner.js";
import { Catalog } from "./catalog.js";

interface Deps {
  cfg: Config;
  registry: ClientRegistry;
  provisioner: Provisioner;
  catalog: Catalog;
}

const SLUG = /^[a-z][a-z0-9-]{1,48}$/;

export async function registerRoutes(app: FastifyInstance, deps: Deps): Promise<void> {
  const { cfg, registry, provisioner, catalog } = deps;

  app.get("/health", async () => ({ ok: true }));

  // --- clients ---
  app.get("/clients", async () => registry.list());

  app.get("/clients/:id", async (req, reply) => {
    const { id } = req.params as { id: string };
    const c = await registry.get(id);
    return c ?? reply.code(404).send({ error: "not found" });
  });

  app.post("/clients", async (req, reply) => {
    const body = req.body as CreateClientInput;
    if (!body?.id || !SLUG.test(body.id)) {
      return reply.code(400).send({ error: "id must be a slug: ^[a-z][a-z0-9-]{1,48}$" });
    }
    if (await registry.get(body.id)) {
      return reply.code(409).send({ error: `client ${body.id} already exists` });
    }

    const now = new Date().toISOString();
    const client: Client = {
      id: body.id,
      name: body.name ?? body.id,
      domain: body.domain ?? `${body.id}.${cfg.baseDomain}`,
      brandImage: body.brandImage ?? cfg.brandImage,
      appPort: await registry.nextFreePort(cfg.appPortStart),
      status: "provisioning",
      createdAt: now,
      updatedAt: now,
    };
    await registry.upsert(client);

    // provision asynchronously so the request returns immediately
    provisioner
      .provision(client)
      .then((r) => registry.setStatus(client.id, r.ok ? "running" : "error", r.ok ? undefined : r.logs.slice(-2000)))
      .catch((e) => registry.setStatus(client.id, "error", String(e)));

    return reply.code(202).send(client);
  });

  app.delete("/clients/:id", async (req, reply) => {
    const { id } = req.params as { id: string };
    const client = await registry.get(id);
    if (!client) return reply.code(404).send({ error: "not found" });

    const r = await provisioner.deprovision(client);
    await registry.setStatus(id, r.ok ? "deprovisioned" : "error", r.ok ? undefined : r.logs.slice(-2000));
    return reply.send({ ok: r.ok });
  });

  // --- connector catalog (shared across all clients) ---
  app.get("/catalog", async () => catalog.list());
  app.get("/catalog/:key", async (req, reply) => {
    const { key } = req.params as { key: string };
    const m = await catalog.get(key);
    return m ?? reply.code(404).send({ error: "not found" });
  });
}
