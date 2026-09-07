// A tenant. One Client == one isolated stack (app + postgres + redis).

export type ClientStatus =
  | "provisioning"
  | "running"
  | "stopped"
  | "error"
  | "deprovisioned";

export interface Client {
  id: string;          // slug, e.g. "acme" — also the compose project suffix (ap-acme)
  name: string;
  domain: string;      // acme.platform.example.com
  status: ClientStatus;
  appPort: number;     // unique host port assigned by the control plane
  brandImage: string;  // the rebranded AP image this client runs
  createdAt: string;   // ISO
  updatedAt: string;   // ISO
  lastError?: string;
  // NOTE: secrets (AP_ENCRYPTION_KEY, db password, …) are NOT stored on this object.
  // They live only in the generated clients/<id>.env and a secrets manager. See registry.ts.
}

export interface CreateClientInput {
  id: string;
  name: string;
  domain?: string;     // defaults to <id>.<CP_BASE_DOMAIN>
  brandImage?: string; // defaults to CP_BRAND_IMAGE
}
