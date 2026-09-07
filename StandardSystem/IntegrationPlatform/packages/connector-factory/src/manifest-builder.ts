// Deterministic OpenAPI -> connector-manifest mapping. This is the mechanical 80%.
// An optional AI pass (ai-refine.ts) curates on top: picks which ops to expose, friendly titles,
// trims input/output, detects pagination, assigns categories. The mechanical core is verifiable.

import type { ConnectorManifest, Auth, Action, Field, RequestTemplate } from "@ip/shared";

type AnyObj = Record<string, unknown>;

export interface BuildOptions {
  specUrl?: string;
  /** origin to resolve a relative server url against, e.g. https://api.example.com */
  baseOrigin?: string;
}

const METHODS = ["get", "post", "put", "patch", "delete"] as const;

function slug(s: string, min = 2, max = 48): string {
  let out = s.toLowerCase().replace(/[^a-z0-9]+/g, "_").replace(/^_+|_+$/g, "").slice(0, max);
  if (!/^[a-z]/.test(out)) out = "c_" + out;
  if (out.length < min) out = (out + "_x").slice(0, max);
  return out;
}
const fieldId = (s: string): string => {
  let out = s.toLowerCase().replace(/[^a-z0-9_]+/g, "_").replace(/^_+/, "");
  if (!/^[a-z]/.test(out)) out = "f_" + out;
  return out;
};

function resolveBaseUrl(doc: AnyObj, opts: BuildOptions): string {
  const servers = doc.servers as { url?: string }[] | undefined;
  let url = servers?.[0]?.url ?? opts.baseOrigin ?? "https://example.com";
  if (!/^https?:\/\//.test(url)) {
    const origin = (opts.baseOrigin ?? (opts.specUrl ? new URL(opts.specUrl).origin : "")).replace(/\/+$/, "");
    url = origin + "/" + url.replace(/^\/+/, "");
  }
  return url;
}

function buildAuth(doc: AnyObj): Auth {
  const comps = (doc.components as AnyObj | undefined)?.securitySchemes as Record<string, AnyObj> | undefined;
  const schemes = comps ? Object.entries(comps) : [];
  // preference order: apiKey -> http(bearer/basic) -> oauth2
  const pick = (pred: (s: AnyObj) => boolean) => schemes.find(([, s]) => pred(s));

  const apiKey = pick((s) => s.type === "apiKey");
  if (apiKey) {
    const [, s] = apiKey;
    const id = fieldId((s.name as string) ?? "api_key");
    const field: Field = { id, label: (s.name as string) ?? "API key", type: "password", secret: true, required: true };
    return {
      type: "apiKey",
      fields: [field],
      apiKey: { in: (s.in as "header" | "query") ?? "header", name: (s.name as string) ?? "X-API-Key", valueTemplate: `{{auth.fields.${id}}}` },
    };
  }
  const http = pick((s) => s.type === "http");
  if (http) {
    const [, s] = http;
    if ((s.scheme as string) === "basic") {
      return { type: "basic", fields: [
        { id: "username", label: "Username", type: "string", required: true },
        { id: "password", label: "Password", type: "password", secret: true, required: true },
      ], basic: { usernameField: "username", passwordField: "password" } };
    }
    return { type: "bearer", fields: [{ id: "token", label: "Bearer token", type: "password", secret: true, required: true }] };
  }
  const oauth = pick((s) => s.type === "oauth2");
  if (oauth) {
    const [, s] = oauth;
    const flows = (s.flows as AnyObj) ?? {};
    const flow = (flows.authorizationCode ?? flows.clientCredentials) as AnyObj | undefined;
    const grantType = flows.authorizationCode ? "authorization_code" : "client_credentials";
    return {
      type: "oauth2",
      fields: [
        { id: "client_id", label: "Client ID", type: "string", required: true },
        { id: "client_secret", label: "Client secret", type: "password", secret: true, required: true },
      ],
      oauth2: {
        grantType,
        authUrl: (flow?.authorizationUrl as string) ?? undefined,
        tokenUrl: (flow?.tokenUrl as string) ?? "https://example.com/oauth/token",
        scopes: Object.keys((flow?.scopes as AnyObj) ?? {}),
      },
    };
  }
  return { type: "none" };
}

function buildAction(method: string, path: string, op: AnyObj): Action {
  const params = (op.parameters as AnyObj[] | undefined) ?? [];
  const inputProps: AnyObj = {};
  const required: string[] = [];
  const query: Record<string, string> = {};
  const headers: Record<string, string> = {};
  let renderedPath = path;

  for (const p of params) {
    const name = p.name as string;
    if (!name) continue;
    inputProps[name] = (p.schema as AnyObj) ?? { type: "string" };
    if (p.required) required.push(name);
    const tpl = `{{input.${name}}}`;
    if (p.in === "path") renderedPath = renderedPath.replace(`{${name}}`, tpl);
    else if (p.in === "query") query[name] = tpl;
    else if (p.in === "header") headers[name] = tpl;
  }

  const request: RequestTemplate = { method: method.toUpperCase() as RequestTemplate["method"], path: renderedPath };
  if (Object.keys(query).length) request.query = query;
  if (Object.keys(headers).length) request.headers = headers;

  // request body (application/json)
  const jsonSchema = ((op.requestBody as AnyObj)?.content as AnyObj)?.["application/json"] as AnyObj | undefined;
  const bodySchema = jsonSchema?.schema as AnyObj | undefined;
  if (bodySchema) {
    request.bodyType = "json";
    if (bodySchema.type === "object" && bodySchema.properties) {
      const body: AnyObj = {};
      const bodyReq = (bodySchema.required as string[]) ?? [];
      for (const [k, sch] of Object.entries(bodySchema.properties as AnyObj)) {
        inputProps[k] = sch;
        body[k] = `{{input.${k}}}`;
        if (bodyReq.includes(k)) required.push(k);
      }
      request.body = body;
    } else {
      inputProps.body = bodySchema;
      request.body = "{{input.body}}";
    }
  }

  // output: first 2xx json schema
  const responses = (op.responses as AnyObj) ?? {};
  const okKey = ["200", "201", "default"].find((k) => responses[k]);
  const outSchema = okKey
    ? (((responses[okKey] as AnyObj)?.content as AnyObj)?.["application/json"] as AnyObj)?.schema
    : undefined;

  const action: Action = {
    title: (op.summary as string) || (op.operationId as string) || `${method.toUpperCase()} ${path}`,
    request,
  };
  if (op.description) action.description = op.description as string;
  if (Object.keys(inputProps).length) {
    action.input = { type: "object", properties: inputProps, ...(required.length ? { required: [...new Set(required)] } : {}) };
  }
  if (outSchema) action.output = outSchema as AnyObj;
  return action;
}

export function buildManifest(doc: AnyObj, opts: BuildOptions = {}): ConnectorManifest {
  const info = (doc.info as AnyObj) ?? {};
  const title = (info.title as string) ?? "connector";

  const actions: Record<string, Action> = {};
  const usedKeys = new Set<string>();
  const paths = (doc.paths as AnyObj) ?? {};
  for (const [path, item] of Object.entries(paths)) {
    for (const method of METHODS) {
      const op = (item as AnyObj)[method] as AnyObj | undefined;
      if (!op) continue;
      let key = slug((op.operationId as string) || `${method}_${path}`);
      while (usedKeys.has(key)) key = key.replace(/(_\d+)?$/, (m) => `_${Number((m || "_0").slice(1)) + 1}`);
      usedKeys.add(key);
      actions[key] = buildAction(method, path, op);
    }
  }

  const manifest: ConnectorManifest = {
    manifestVersion: "1.0",
    key: slug(title),
    name: title,
    version: "0.1.0",
    connection: { baseUrl: resolveBaseUrl(doc, opts) },
    auth: buildAuth(doc),
    actions,
    source: {
      type: "openapi",
      specUrl: opts.specUrl,
      specVersion: (doc.openapi as string) ?? (doc.swagger as string),
      generatedBy: "connector-factory deterministic v1",
      reviewed: false,
    },
  };
  if (info.description) manifest.description = info.description as string;
  if ((doc.externalDocs as AnyObj)?.url) manifest.docsUrl = (doc.externalDocs as AnyObj).url as string;
  return manifest;
}
