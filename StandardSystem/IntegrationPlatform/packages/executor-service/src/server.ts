// Executor microservice — exposes the universal-executor over HTTP so the platform (and any caller)
// can run connector manifests without embedding the executor. Zero runtime deps (Node http only);
// bundled to a single file with esbuild for dead-simple deployment.
//
// Routes:
//   GET  /health                       -> { ok: true }
//   GET  /catalog                      -> [{ key, name, actions:[...], triggers:[...] }]
//   POST /run  { connector, action, input, credential } -> ExecuteResult (JSON in-flight)

import { createServer, type IncomingMessage, type ServerResponse } from "node:http";
import { executeAction, type Credential } from "../../universal-executor/src/index.js";
import stripe from "../../../connectors/stripe.connector.json" with { type: "json" };
import onfinity from "../../../connectors/onfinity.connector.json" with { type: "json" };

// bundled catalog — swap for a fetch from the control-plane catalog API later
const CATALOG: Record<string, any> = {
  [stripe.key]: stripe,
  [onfinity.key]: onfinity,
};

const PORT = Number(process.env.EXECUTOR_PORT ?? 3070);
const HOST = process.env.EXECUTOR_HOST ?? "0.0.0.0";

function send(res: ServerResponse, status: number, body: unknown): void {
  const json = JSON.stringify(body);
  res.writeHead(status, { "content-type": "application/json", "content-length": Buffer.byteLength(json) });
  res.end(json);
}

function readBody(req: IncomingMessage): Promise<any> {
  return new Promise((resolve, reject) => {
    let data = "";
    req.on("data", (c) => (data += c));
    req.on("end", () => {
      try { resolve(data ? JSON.parse(data) : {}); } catch (e) { reject(e); }
    });
    req.on("error", reject);
  });
}

function catalogSummary() {
  return Object.values(CATALOG).map((m: any) => ({
    key: m.key,
    name: m.name,
    categories: m.categories ?? [],
    actions: Object.keys(m.actions ?? {}),
    triggers: Object.keys(m.triggers ?? {}),
  }));
}

const server = createServer(async (req, res) => {
  try {
    const url = new URL(req.url ?? "/", `http://${req.headers.host}`);
    if (req.method === "GET" && url.pathname === "/health") return send(res, 200, { ok: true, connectors: Object.keys(CATALOG) });
    if (req.method === "GET" && url.pathname === "/catalog") return send(res, 200, catalogSummary());

    if (req.method === "POST" && url.pathname === "/run") {
      const body = await readBody(req);
      const { connector, action, input, credential } = body ?? {};
      const manifest = CATALOG[connector];
      if (!manifest) return send(res, 404, { error: `unknown connector: ${connector}`, available: Object.keys(CATALOG) });
      if (!action) return send(res, 400, { error: "missing 'action'" });
      const cred: Credential = credential ?? { fields: {} };
      const result = await executeAction(manifest, action, input ?? {}, cred);
      return send(res, 200, result);
    }

    return send(res, 404, { error: "not found", routes: ["GET /health", "GET /catalog", "POST /run"] });
  } catch (e: any) {
    return send(res, 500, { error: String(e?.message ?? e) });
  }
});

server.listen(PORT, HOST, () => {
  // eslint-disable-next-line no-console
  console.log(`executor-service listening on ${HOST}:${PORT} — connectors: ${Object.keys(CATALOG).join(", ")}`);
});
