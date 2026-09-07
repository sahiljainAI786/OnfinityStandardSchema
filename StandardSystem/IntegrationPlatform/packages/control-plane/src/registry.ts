import { readFile, writeFile, mkdir } from "node:fs/promises";
import { dirname, join } from "node:path";
import type { Client, ClientStatus } from "@ip/shared";

// Skeleton store: a JSON file. Swap for the cp-postgres registry DB (CP_DATABASE_URL) in production —
// the interface is what the routes depend on, not the backing store.
export class ClientRegistry {
  constructor(private readonly file: string) {}

  private async readAll(): Promise<Client[]> {
    try {
      return JSON.parse(await readFile(this.file, "utf8")) as Client[];
    } catch {
      return [];
    }
  }

  private async writeAll(clients: Client[]): Promise<void> {
    await mkdir(dirname(this.file), { recursive: true });
    await writeFile(this.file, JSON.stringify(clients, null, 2), "utf8");
  }

  list(): Promise<Client[]> {
    return this.readAll();
  }

  async get(id: string): Promise<Client | undefined> {
    return (await this.readAll()).find((c) => c.id === id);
  }

  async upsert(client: Client): Promise<Client> {
    const all = await this.readAll();
    const i = all.findIndex((c) => c.id === client.id);
    if (i >= 0) all[i] = client;
    else all.push(client);
    await this.writeAll(all);
    return client;
  }

  async setStatus(id: string, status: ClientStatus, lastError?: string): Promise<void> {
    const c = await this.get(id);
    if (!c) return;
    c.status = status;
    c.updatedAt = new Date().toISOString();
    if (lastError !== undefined) c.lastError = lastError;
    await this.upsert(c);
  }

  /** lowest free host port at or above appPortStart */
  async nextFreePort(start: number): Promise<number> {
    const used = new Set((await this.readAll()).map((c) => c.appPort));
    let p = start;
    while (used.has(p)) p++;
    return p;
  }
}

export const clientsFile = (clientsDir: string) => join(clientsDir, "registry.json");
