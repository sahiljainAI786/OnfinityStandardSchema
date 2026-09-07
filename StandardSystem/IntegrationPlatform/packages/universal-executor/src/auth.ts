// Inject credentials into a request per the manifest's auth type. The credential values live in
// ctx.auth (fields + tokens); custom/multi-header auth is handled via connection.defaultHeaders
// (rendered in request.ts), so this only covers the structured schemes.

import type { Auth } from "@ip/shared";
import { renderValue, type RenderContext } from "./template.js";

export interface AuthedParts {
  headers: Record<string, string>;
  query: Record<string, string>;
}

function b64(s: string): string {
  return Buffer.from(s, "utf8").toString("base64");
}

export function applyAuth(auth: Auth, ctx: RenderContext): AuthedParts {
  const headers: Record<string, string> = {};
  const query: Record<string, string> = {};
  const fields = (ctx.auth as { fields?: Record<string, unknown> })?.fields ?? {};

  switch (auth.type) {
    case "apiKey": {
      if (!auth.apiKey) break;
      const value = String(renderValue(auth.apiKey.valueTemplate, ctx) ?? "");
      if (auth.apiKey.in === "header") headers[auth.apiKey.name] = value;
      else query[auth.apiKey.name] = value;
      break;
    }
    case "basic": {
      if (!auth.basic) break;
      const u = String((fields as Record<string, unknown>)[auth.basic.usernameField] ?? "");
      const p = String((fields as Record<string, unknown>)[auth.basic.passwordField] ?? "");
      headers["Authorization"] = `Basic ${b64(`${u}:${p}`)}`;
      break;
    }
    case "oauth2": {
      const tpl = auth.oauth2?.accessTokenTemplate ?? "Bearer {{auth.tokens.access_token}}";
      headers["Authorization"] = String(renderValue(tpl, ctx) ?? "");
      break;
    }
    case "bearer": {
      const token = (fields as Record<string, unknown>).token ?? (fields as Record<string, unknown>).api_key;
      if (token) headers["Authorization"] = `Bearer ${token}`;
      break;
    }
    case "none":
    case "custom":
    default:
      // none → nothing; custom → headers come from connection.defaultHeaders
      break;
  }

  return { headers, query };
}
