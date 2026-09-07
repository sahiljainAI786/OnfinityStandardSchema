// CLI: generate a connector manifest from an OpenAPI spec.
//   npx tsx src/index.ts <specUrlOrFile> [--out file.json] [--base https://api.host] [--schema path]
//
// Loads → maps → validates → writes. Refuses to emit an invalid manifest.

import { writeFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { dirname, join } from "node:path";
import { loadSpec } from "./openapi-loader.js";
import { buildManifest } from "./manifest-builder.js";
import { assertValid } from "./validate.js";

export { loadSpec } from "./openapi-loader.js";
export { buildManifest } from "./manifest-builder.js";
export { validateManifest, assertValid } from "./validate.js";
export { refine, type RefineHooks } from "./ai-refine.js";

function arg(name: string): string | undefined {
  const i = process.argv.indexOf(`--${name}`);
  return i >= 0 ? process.argv[i + 1] : undefined;
}

async function main(): Promise<void> {
  const source = process.argv[2];
  if (!source || source.startsWith("--")) {
    console.error("usage: connector-factory <specUrlOrFile> [--out file] [--base origin] [--schema path]");
    process.exit(2);
  }
  const here = dirname(fileURLToPath(import.meta.url));
  const schemaPath = arg("schema") ?? join(here, "../../../schema/connector-manifest.schema.json");

  const { doc, specUrl } = await loadSpec(source);
  const manifest = buildManifest(doc, { specUrl, baseOrigin: arg("base") });
  assertValid(manifest, schemaPath); // throws on invalid

  const out = arg("out");
  const json = JSON.stringify(manifest, null, 2);
  if (out) {
    writeFileSync(out, json + "\n");
    console.error(`wrote ${out} — ${manifest.key}, ${Object.keys(manifest.actions ?? {}).length} actions, auth=${manifest.auth.type}`);
  } else {
    console.log(json);
  }
}

main().catch((e) => {
  console.error(String(e));
  process.exit(1);
});
