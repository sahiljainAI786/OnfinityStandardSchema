// The orchestrator: run one manifest action with a credential + input, returning JSON in-flight.
// Handles auth, templating, pagination, rate-limit backoff, and normalized error outcomes.

import type { ConnectorManifest, RateLimit, ErrorRule } from "@ip/shared";
import { buildRequest, sendRequest, type HttpResponse } from "./request.js";
import { paginate, resolvePagination } from "./pagination.js";
import { getPath, type RenderContext } from "./template.js";

export * from "./template.js";
export * from "./request.js";
export * from "./pagination.js";

/** A user's stored credential, scoped per tenant by the vault. */
export interface Credential {
  fields: Record<string, unknown>; // auth.fields values (decrypted)
  tokens?: Record<string, unknown>; // oauth tokens, if any
  connection?: Record<string, unknown>; // connection.variables values
}

export interface ExecuteResult {
  outcome: "success" | "fail" | "ignore" | "reauth";
  status?: number;
  data: unknown;
  pages?: number;
  truncated?: boolean;
  message?: string;
}

const sleep = (ms: number) => new Promise((r) => setTimeout(r, ms));

function matchError(rules: ErrorRule[] | undefined, status: number): ErrorRule | undefined {
  return rules?.find((r) => r.whenStatus?.includes(status));
}

async function sendWithRetry(
  built: ReturnType<typeof buildRequest>,
  rate: RateLimit | undefined,
): Promise<HttpResponse> {
  const retryOn = new Set(rate?.retryOn ?? [429, 503]);
  const maxRetries = rate?.maxRetries ?? 5;
  let attempt = 0;
  // simple exponential backoff, honoring Retry-After when present
  for (;;) {
    const res = await sendRequest(built);
    if (!retryOn.has(res.status) || attempt >= maxRetries) return res;
    const retryAfter = rate?.respectRetryAfterHeader !== false ? Number(res.headers["retry-after"]) : NaN;
    const waitMs = Number.isFinite(retryAfter) ? retryAfter * 1000 : Math.min(2 ** attempt * 250, 8000);
    await sleep(waitMs);
    attempt++;
  }
}

export function buildContext(manifest: ConnectorManifest, cred: Credential, input: unknown): RenderContext {
  return {
    input: input ?? {},
    auth: { fields: cred.fields ?? {}, tokens: cred.tokens ?? {} },
    connection: cred.connection ?? {},
  };
}

export async function executeAction(
  manifest: ConnectorManifest,
  actionKey: string,
  input: unknown,
  cred: Credential,
): Promise<ExecuteResult> {
  const action = manifest.actions?.[actionKey];
  if (!action) throw new Error(`unknown action: ${manifest.key}.${actionKey}`);

  const ctx = buildContext(manifest, cred, input);
  const rate = action.rateLimit ?? manifest.rateLimit;

  // paginated read
  const pag = resolvePagination(manifest, action.pagination);
  if (pag && pag.type !== "none") {
    const result = await paginate(manifest, action.request, pag, ctx);
    return { outcome: "success", data: result.items, pages: result.pages, truncated: result.truncated };
  }

  // single call
  const res = await sendWithRetry(buildRequest(manifest, action.request, ctx), rate);
  if (res.ok) return { outcome: "success", status: res.status, data: res.body };

  const rule = matchError(action.errors, res.status);
  const message = rule?.messagePath ? String(getPath(res.body as RenderContext, rule.messagePath) ?? "") : undefined;
  return { outcome: rule?.outcome ?? "fail", status: res.status, data: res.body, message };
}
