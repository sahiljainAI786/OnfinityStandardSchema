import { spawn } from "node:child_process";
import { randomBytes } from "node:crypto";
import { writeFile, mkdir } from "node:fs/promises";
import { join } from "node:path";
import type { Client, ClientStatus } from "@ip/shared";
import type { Config } from "./config.js";

export interface ProvisionResult {
  ok: boolean;
  logs: string;
}

// The seam that makes the tenancy model swappable: on a single host we drive `docker compose`;
// in production you implement the same interface against Kubernetes/Nomad and change nothing else.
export interface Provisioner {
  provision(client: Client): Promise<ProvisionResult>;
  deprovision(client: Client): Promise<ProvisionResult>;
  status(client: Client): Promise<ClientStatus>;
}

const secret = (bytes = 24) => randomBytes(bytes).toString("hex");

export class DockerComposeProvisioner implements Provisioner {
  constructor(private readonly cfg: Config) {}

  private project(client: Client): string {
    return `ap-${client.id}`;
  }

  /** Render and persist this client's env file. Secrets are generated fresh per client. */
  private async writeEnv(client: Client): Promise<string> {
    const env: Record<string, string> = {
      CLIENT_ID: client.id,
      CLIENT_DOMAIN: client.domain,
      BRAND_IMAGE: client.brandImage,
      APP_PORT: String(client.appPort),
      AP_JWT_SECRET: secret(),
      AP_ENCRYPTION_KEY: secret(16), // 32 hex chars = AP's expected key length
      POSTGRES_DB: "activepieces",
      POSTGRES_USER: `ap_${client.id}`,
      POSTGRES_PASSWORD: secret(),
    };
    await mkdir(this.cfg.clientsDir, { recursive: true });
    const path = join(this.cfg.clientsDir, `${client.id}.env`);
    // TODO: persist the generated secrets to a secrets manager; the env file is regenerated, not the source of truth.
    await writeFile(path, Object.entries(env).map(([k, v]) => `${k}=${v}`).join("\n") + "\n", "utf8");
    return path;
  }

  private compose(args: string[], envFile?: string): Promise<ProvisionResult> {
    const base = ["compose", "-f", this.cfg.clientComposeFile];
    if (envFile) base.push("--env-file", envFile);
    return new Promise((resolve) => {
      const child = spawn("docker", [...base, ...args], { env: process.env });
      let logs = "";
      child.stdout.on("data", (d) => (logs += d));
      child.stderr.on("data", (d) => (logs += d));
      child.on("close", (code) => resolve({ ok: code === 0, logs }));
      child.on("error", (e) => resolve({ ok: false, logs: String(e) }));
    });
  }

  async provision(client: Client): Promise<ProvisionResult> {
    const envFile = await this.writeEnv(client);
    return this.compose(["-p", this.project(client), "up", "-d"], envFile);
  }

  async deprovision(client: Client): Promise<ProvisionResult> {
    // pass the client's env-file so compose can interpolate the template vars during `down`
    const envFile = join(this.cfg.clientsDir, `${client.id}.env`);
    // keep volumes by default; add "-v" to also drop the client's data
    return this.compose(["-p", this.project(client), "down"], envFile);
  }

  async status(client: Client): Promise<ClientStatus> {
    const r = await this.compose(["-p", this.project(client), "ps", "--status=running", "-q"]);
    if (!r.ok) return "error";
    return r.logs.trim().length > 0 ? "running" : "stopped";
  }
}
