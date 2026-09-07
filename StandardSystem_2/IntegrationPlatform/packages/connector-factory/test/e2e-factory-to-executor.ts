// The whole thesis in one test: the AI factory generates a connector from an OpenAPI URL, and the
// executor immediately runs it against the live API — no per-connector code written by hand.
// Run: npx tsx test/e2e-factory-to-executor.ts

import { loadSpec } from "../src/openapi-loader.js";
import { buildManifest } from "../src/manifest-builder.js";
import { executeAction, type Credential } from "../../universal-executor/src/index.js";

async function main() {
  const specUrl = "https://petstore3.swagger.io/api/v3/openapi.json";
  console.log("1. factory: generating connector from", specUrl);
  const { doc } = await loadSpec(specUrl);
  const manifest = buildManifest(doc, { specUrl, baseOrigin: "https://petstore3.swagger.io" });
  console.log(`   generated "${manifest.key}" with ${Object.keys(manifest.actions ?? {}).length} actions`);

  console.log("2. executor: running getpetbyid(petId=10) against the LIVE API");
  const cred: Credential = { fields: { api_key: "demo" } }; // petstore ignores it for GET
  const res = await executeAction(manifest, "getpetbyid", { petId: 10 }, cred);

  const pet = res.data as { id?: number; name?: string };
  const ok = res.outcome === "success" && res.status === 200 && typeof pet?.name === "string";
  console.log(`   outcome=${res.outcome} status=${res.status} pet=${pet?.name ?? "n/a"} (id ${pet?.id ?? "?"})`);

  console.log(ok
    ? "\nPASS — factory-generated connector ran live (path-param templated) with zero hand-coding"
    : "\nFAIL");
  process.exitCode = ok ? 0 : 1;
}

main();
