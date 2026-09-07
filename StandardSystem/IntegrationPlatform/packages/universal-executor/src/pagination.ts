// Walk all pages of a list response per the action's pagination strategy, concatenating items.
// Bounded by maxPages so a runaway list can't loop forever (the truncation is logged by the caller).

import type { ConnectorManifest, Pagination, RequestTemplate } from "@ip/shared";
import { buildRequest, sendRequest, type HttpResponse } from "./request.js";
import { getPath, type RenderContext } from "./template.js";

export interface PageResult {
  items: unknown[];
  pages: number;
  truncated: boolean;
}

/** Resolve `{ $use: "name" }` references against manifest.paginators. */
export function resolvePagination(
  manifest: ConnectorManifest,
  pag: Pagination | { $use: string } | undefined,
): Pagination | undefined {
  if (!pag) return undefined;
  if ("$use" in pag) return manifest.paginators?.[pag.$use];
  return pag;
}

const itemsAt = (res: HttpResponse, path: string): unknown[] => {
  const v = getPath(res.body as RenderContext, path);
  return Array.isArray(v) ? v : [];
};

export async function paginate(
  manifest: ConnectorManifest,
  req: RequestTemplate,
  pag: Pagination,
  ctx: RenderContext,
): Promise<PageResult> {
  const items: unknown[] = [];
  const size = pag.pageSize ?? 100;
  const maxPages = pag.maxPages ?? 50;
  let pages = 0;

  let cursor: string | undefined;
  let page = 1;
  let offset = 0;

  while (pages < maxPages) {
    const extra: Record<string, string> = {};
    if (pag.limitParam) extra[pag.limitParam] = String(size);
    if (pag.type === "cursor" && cursor && pag.cursorRequestParam) extra[pag.cursorRequestParam] = cursor;
    if (pag.type === "page" && pag.pageParam) extra[pag.pageParam] = String(page);
    if (pag.type === "offset" && pag.offsetParam) extra[pag.offsetParam] = String(offset);

    const res = await sendRequest(buildRequest(manifest, req, ctx, extra));
    pages++;
    const batch = itemsAt(res, pag.itemsPath);
    items.push(...batch);

    // decide whether to continue
    const hasMoreFlag = pag.hasMorePath ? Boolean(getPath(res.body as RenderContext, pag.hasMorePath)) : undefined;
    if (pag.type === "cursor") {
      const next = pag.cursorResponsePath ? getPath(res.body as RenderContext, pag.cursorResponsePath) : undefined;
      cursor = next == null ? undefined : String(next);
      if (!cursor || hasMoreFlag === false || batch.length === 0) break;
    } else if (pag.type === "page") {
      page++;
      if (hasMoreFlag === false || batch.length < size || batch.length === 0) break;
    } else if (pag.type === "offset") {
      offset += size;
      if (hasMoreFlag === false || batch.length < size || batch.length === 0) break;
    } else {
      break; // none / link_header (link_header TODO)
    }
  }

  return { items, pages, truncated: pages >= maxPages };
}
