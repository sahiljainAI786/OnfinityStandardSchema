import { readdir, readFile } from "node:fs/promises";
import { join } from "node:path";
import type { ConnectorManifest } from "@ip/shared";

// The shared connector catalog: manifests live centrally and are DISTRIBUTED to client stacks.
// Here we just load + summarize them. Distribution (push to a client's instance) and schema
// validation (ajv against CP_SCHEMA_FILE) are the next steps — marked TODO below.
export class Catalog {
  constructor(private readonly connectorsDir: string, private readonly schemaFile: string) {}

  async list(): Promise<{ key: string; name: string; version?: string; categories?: string[] }[]> {
    const files = (await readdir(this.connectorsDir)).filter((f) => f.endsWith(".connector.json"));
    const out = [];
    for (const f of files) {
      const m = JSON.parse(await readFile(join(this.connectorsDir, f), "utf8")) as ConnectorManifest;
      out.push({ key: m.key, name: m.name, version: m.version, categories: m.categories });
    }
    return out;
  }

  async get(key: string): Promise<ConnectorManifest | undefined> {
    const files = (await readdir(this.connectorsDir)).filter((f) => f.endsWith(".connector.json"));
    for (const f of files) {
      const m = JSON.parse(await readFile(join(this.connectorsDir, f), "utf8")) as ConnectorManifest;
      if (m.key === key) return m;
    }
    return undefined;
  }

  // TODO: validate(manifest) against this.schemaFile with ajv before publishing.
  // TODO: distribute(clientId): push the active catalog to that client's instance.
}
