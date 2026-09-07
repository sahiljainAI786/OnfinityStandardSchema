// The AI curation pass — the "mass connectors using AI" step. OPTIONAL and pluggable: the
// deterministic builder already emits a valid manifest; this refines it for usability.
//
// Kept as a typed seam so the pipeline runs without an LLM (deterministic) and gains curation
// when a model is wired in. The LLM's job is narrow and verifiable because every endpoint it sees
// already came from the spec — it labels and trims, it does not invent.

import type { ConnectorManifest } from "@ip/shared";

export interface RefineHooks {
  /** Drop noisy/internal operations; keep the ones worth exposing as actions. */
  selectActions?: (m: ConnectorManifest) => Promise<string[]> | string[];
  /** Friendlier titles/descriptions than raw operationIds. */
  relabel?: (m: ConnectorManifest) => Promise<ConnectorManifest> | ConnectorManifest;
  /** Detect list endpoints + their pagination strategy. */
  detectPagination?: (m: ConnectorManifest) => Promise<ConnectorManifest> | ConnectorManifest;
  /** Assign catalog categories (crm/ecommerce/…). */
  classify?: (m: ConnectorManifest) => Promise<string[]> | string[];
}

export async function refine(manifest: ConnectorManifest, hooks: RefineHooks = {}): Promise<ConnectorManifest> {
  let m = manifest;
  if (hooks.relabel) m = await hooks.relabel(m);
  if (hooks.detectPagination) m = await hooks.detectPagination(m);
  if (hooks.selectActions && m.actions) {
    const keep = new Set(await hooks.selectActions(m));
    m = { ...m, actions: Object.fromEntries(Object.entries(m.actions).filter(([k]) => keep.has(k))) };
  }
  if (hooks.classify) m = { ...m, categories: (await hooks.classify(m)) as ConnectorManifest["categories"] };
  return m;
}
