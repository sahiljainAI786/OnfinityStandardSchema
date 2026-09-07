// TypeScript mirror of schema/connector-manifest.schema.json.
// The schema is the source of truth (the AI factory validates against it); these types give the
// executor and control plane compile-time safety. Keep them in sync — a CI check should assert that.

export type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
export type ConnectorCategory =
  | "crm" | "ecommerce" | "finance" | "support" | "marketing"
  | "productivity" | "communication" | "hr" | "erp" | "analytics" | "developer" | "other";

export interface Field {
  id: string;
  label: string;
  type: "string" | "password" | "number" | "boolean" | "select" | "url";
  secret?: boolean;
  required?: boolean;
  help?: string;
  placeholder?: string;
  default?: unknown;
  options?: { value: unknown; label: string }[];
}

export interface RequestTemplate {
  method: HttpMethod;
  path: string;
  headers?: Record<string, string>;
  query?: Record<string, string>;
  bodyType?: "json" | "form" | "none";
  body?: Record<string, unknown> | string | null;
}

export interface Pagination {
  type: "none" | "cursor" | "page" | "offset" | "link_header";
  itemsPath: string;
  cursorRequestParam?: string;
  cursorResponsePath?: string;
  hasMorePath?: string;
  pageParam?: string;
  limitParam?: string;
  offsetParam?: string;
  pageSize?: number;
  maxPages?: number;
}

export interface RateLimit {
  requests?: number;
  perSeconds?: number;
  retryOn?: number[];
  maxRetries?: number;
  respectRetryAfterHeader?: boolean;
}

export interface ErrorRule {
  whenStatus?: number[];
  outcome: "retry" | "fail" | "ignore" | "reauth";
  messagePath?: string;
}

export interface Action {
  title: string;
  description?: string;
  request: RequestTemplate;
  input?: Record<string, unknown>;   // JSON Schema
  output?: Record<string, unknown>;  // JSON Schema
  pagination?: Pagination | { $use: string };
  rateLimit?: RateLimit;
  errors?: ErrorRule[];
  sample?: { input?: unknown; output?: unknown };
}

export interface WebhookTriggerConfig {
  subscribe?: RequestTemplate;
  unsubscribe?: RequestTemplate;
  eventTypePath?: string;
  verification?: {
    type: "none" | "hmac" | "shared_secret" | "signature_header";
    header?: string;
    algorithm?: "sha256" | "sha1";
    secretField?: string;
  };
}

export interface PollingTriggerConfig {
  request: RequestTemplate;
  itemsPath: string;
  intervalSeconds?: number;
  cursor?: { type: "timestamp" | "id"; requestParam?: string; responsePath?: string };
}

export interface Trigger {
  title: string;
  description?: string;
  type: "webhook" | "polling";
  input?: Record<string, unknown>;
  output?: Record<string, unknown>;
  dedupeKeyPath?: string;
  sample?: Record<string, unknown>;
  webhook?: WebhookTriggerConfig;
  polling?: PollingTriggerConfig;
}

export interface Auth {
  type: "none" | "apiKey" | "bearer" | "basic" | "oauth2" | "custom";
  fields?: Field[];
  test?: { action: string; input?: Record<string, unknown> };
  apiKey?: { in: "header" | "query"; name: string; valueTemplate: string };
  basic?: { usernameField: string; passwordField: string };
  oauth2?: {
    grantType: "authorization_code" | "client_credentials";
    authUrl?: string;
    tokenUrl: string;
    refreshUrl?: string;
    scopes?: string[];
    scopeSeparator?: string;
    pkce?: boolean;
    clientCredentialsLocation?: "basic_auth_header" | "request_body";
    accessTokenTemplate?: string;
  };
}

export interface Connection {
  baseUrl: string;
  variables?: Field[];
  defaultHeaders?: Record<string, string>;
}

export interface ConnectorManifest {
  manifestVersion: "1.0";
  key: string;
  name: string;
  version?: string;
  description?: string;
  vendor?: string;
  categories?: ConnectorCategory[];
  logoUrl?: string;
  homepage?: string;
  docsUrl?: string;
  source?: {
    type?: "openapi" | "docs" | "manual";
    specUrl?: string;
    specVersion?: string;
    specHash?: string;
    generatedBy?: string;
    reviewed?: boolean;
  };
  connection: Connection;
  auth: Auth;
  rateLimit?: RateLimit;
  paginators?: Record<string, Pagination>;
  actions?: Record<string, Action>;
  triggers?: Record<string, Trigger>;
}
