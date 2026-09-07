// Load an OpenAPI document and fully dereference its local $refs, so the mapper works with
// self-contained schemas. JSON specs only for now (YAML is a TODO — most big SaaS publish JSON).

export interface LoadedSpec {
  doc: Record<string, unknown>;
  specUrl?: string;
}

export async function loadSpec(source: string): Promise<LoadedSpec> {
  let text: string;
  let specUrl: string | undefined;
  if (/^https?:\/\//.test(source)) {
    specUrl = source;
    const res = await fetch(source);
    if (!res.ok) throw new Error(`fetch spec failed: ${res.status} ${source}`);
    text = await res.text();
  } else {
    const { readFileSync } = await import("node:fs");
    text = readFileSync(source, "utf8");
  }
  let parsed: Record<string, unknown>;
  try {
    parsed = JSON.parse(text);
  } catch {
    throw new Error("only JSON OpenAPI specs are supported in this build (YAML TODO)");
  }
  return { doc: deref(parsed), specUrl };
}

/** Resolve "#/a/b/c" JSON pointers within the same document and inline them (cycle-guarded). */
export function deref(root: Record<string, unknown>): Record<string, unknown> {
  const resolve = (ref: string): unknown => {
    const parts = ref.replace(/^#\//, "").split("/").map((p) => p.replace(/~1/g, "/").replace(/~0/g, "~"));
    let cur: unknown = root;
    for (const p of parts) cur = (cur as Record<string, unknown>)?.[p];
    return cur;
  };
  const walk = (node: unknown, stack: string[]): unknown => {
    if (Array.isArray(node)) return node.map((n) => walk(n, stack));
    if (node && typeof node === "object") {
      const ref = (node as Record<string, unknown>).$ref;
      if (typeof ref === "string") {
        if (stack.includes(ref)) return { description: "circular $ref omitted" };
        const target = resolve(ref);
        if (target === undefined) return {};
        return walk(target, [...stack, ref]);
      }
      const out: Record<string, unknown> = {};
      for (const [k, v] of Object.entries(node)) out[k] = walk(v, stack);
      return out;
    }
    return node;
  };
  return walk(root, []) as Record<string, unknown>;
}
