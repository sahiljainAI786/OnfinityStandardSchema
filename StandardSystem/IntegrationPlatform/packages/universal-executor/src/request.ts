// Build and send one HTTP request from a manifest action + rendered context.

import type { ConnectorManifest, RequestTemplate } from "@ip/shared";
import { render, renderStringMap, renderValue, type RenderContext } from "./template.js";
import { applyAuth } from "./auth.js";

export interface BuiltRequest {
  url: string;
  method: string;
  headers: Record<string, string>;
  body?: string;
}

export interface HttpResponse {
  status: number;
  ok: boolean;
  headers: Record<string, string>;
  body: unknown; // parsed JSON when possible, else text
}

function joinUrl(base: string, path: string): string {
  return `${base.replace(/\/+$/, "")}/${path.replace(/^\/+/, "")}`;
}

function toQueryString(q: Record<string, string>): string {
  const s = new URLSearchParams(q).toString();
  return s ? `?${s}` : "";
}

export function buildRequest(
  manifest: ConnectorManifest,
  req: RequestTemplate,
  ctx: RenderContext,
  extraQuery: Record<string, string> = {},
): BuiltRequest {
  const baseUrl = String(renderValue(manifest.connection.baseUrl, ctx) ?? "");
  const path = String(renderValue(req.path, ctx) ?? "");

  const auth = applyAuth(manifest.auth, ctx);
  const headers: Record<string, string> = {
    ...renderStringMap(manifest.connection.defaultHeaders, ctx),
    ...renderStringMap(req.headers, ctx),
    ...auth.headers,
  };
  const query: Record<string, string> = {
    ...renderStringMap(req.query, ctx),
    ...auth.query,
    ...extraQuery, // pagination params override
  };

  let body: string | undefined;
  const bodyType = req.bodyType ?? "json";
  if (req.body != null && bodyType !== "none") {
    const rendered = render(req.body, ctx);
    if (bodyType === "json") {
      body = typeof rendered === "string" ? rendered : JSON.stringify(rendered);
      headers["Content-Type"] ??= "application/json";
    } else if (bodyType === "form") {
      const flat: Record<string, string> = {};
      for (const [k, v] of Object.entries(rendered as Record<string, unknown>)) {
        if (v == null) continue;
        flat[k] = typeof v === "object" ? JSON.stringify(v) : String(v);
      }
      body = new URLSearchParams(flat).toString();
      headers["Content-Type"] ??= "application/x-www-form-urlencoded";
    }
  }

  return { url: joinUrl(baseUrl, path) + toQueryString(query), method: req.method, headers, body };
}

export async function sendRequest(built: BuiltRequest): Promise<HttpResponse> {
  const res = await fetch(built.url, {
    method: built.method,
    headers: built.headers,
    body: built.body,
  });
  const text = await res.text();
  let parsed: unknown = text;
  try {
    parsed = text ? JSON.parse(text) : null;
  } catch {
    /* leave as text */
  }
  const headers: Record<string, string> = {};
  res.headers.forEach((v, k) => (headers[k] = v));
  return { status: res.status, ok: res.ok, headers, body: parsed };
}
