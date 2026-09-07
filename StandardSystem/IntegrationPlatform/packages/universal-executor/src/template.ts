// The {{...}} renderer. This is the ONLY "logic" a manifest carries — no arbitrary code — so it must
// be small, predictable, and side-effect free.
//
// Contexts available to expressions: input, auth, connection, cursor, item, webhook, state.
// Whole-string templates ("{{input.params}}") return the RAW resolved value (array/object/number),
// preserving type. Embedded templates ("id-{{input.id}}") coerce to string.

export type RenderContext = Record<string, unknown>;

const EXPR = /\{\{\s*([^}]+?)\s*\}\}/g;
const WHOLE = /^\{\{\s*([^}]+?)\s*\}\}$/;

/** Resolve a dotted/bracketed path like `a.b[0].c` or `data[-1].id` against the context. */
export function getPath(ctx: RenderContext, path: string): unknown {
  const parts: (string | number)[] = [];
  for (const seg of path.split(".")) {
    const m = seg.matchAll(/([^[\]]+)|\[(-?\d+)\]/g);
    for (const g of m) {
      if (g[2] !== undefined) parts.push(Number(g[2]));
      else if (g[1] !== undefined) parts.push(g[1]);
    }
  }
  let cur: unknown = ctx;
  for (const p of parts) {
    if (cur == null) return undefined;
    if (typeof p === "number" && Array.isArray(cur)) {
      cur = p < 0 ? cur[cur.length + p] : cur[p];
    } else if (typeof cur === "object") {
      cur = (cur as Record<string, unknown>)[String(p)];
    } else {
      return undefined;
    }
  }
  return cur;
}

/** Render one template string. Returns a non-string value for whole-string templates. */
export function renderValue(template: string, ctx: RenderContext): unknown {
  const whole = template.match(WHOLE);
  if (whole) return getPath(ctx, whole[1]);
  return template.replace(EXPR, (_, expr) => {
    const v = getPath(ctx, expr.trim());
    return v == null ? "" : typeof v === "object" ? JSON.stringify(v) : String(v);
  });
}

/** Recursively render any template structure (string, object, array). Non-strings pass through. */
export function render<T>(node: T, ctx: RenderContext): unknown {
  if (typeof node === "string") return renderValue(node, ctx);
  if (Array.isArray(node)) return node.map((n) => render(n, ctx));
  if (node && typeof node === "object") {
    const out: Record<string, unknown> = {};
    for (const [k, v] of Object.entries(node)) out[k] = render(v, ctx);
    return out;
  }
  return node;
}

/** Render a string template, dropping keys whose value resolves to null/undefined (for query params). */
export function renderStringMap(
  map: Record<string, string> | undefined,
  ctx: RenderContext,
): Record<string, string> {
  const out: Record<string, string> = {};
  if (!map) return out;
  for (const [k, tpl] of Object.entries(map)) {
    const v = renderValue(tpl, ctx);
    if (v == null || v === "") continue;
    out[k] = typeof v === "object" ? JSON.stringify(v) : String(v);
  }
  return out;
}
