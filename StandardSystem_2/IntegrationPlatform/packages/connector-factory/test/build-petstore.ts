// Generate a connector from the live Swagger Petstore spec and prove it's schema-valid + usable.
// Run: npx tsx test/build-petstore.ts

import { fileURLToPath } from "node:url";
import { dirname, join } from "node:path";
import { loadSpec } from "../src/openapi-loader.js";
import { buildManifest } from "../src/manifest-builder.js";
import { validateManifest } from "../src/validate.js";

const here = dirname(fileURLToPath(import.meta.url));
const schemaPath = join(here, "../../../schema/connector-manifest.schema.json");

let pass = 0, fail = 0;
const check = (n: string, c: boolean, extra = "") => {
  if (c) { pass++; console.log("  ok  -", n); } else { fail++; console.log("  FAIL-", n, extra); }
};

async function main() {
  const specUrl = "https://petstore3.swagger.io/api/v3/openapi.json";
  console.log("loading", specUrl);
  const { doc } = await loadSpec(specUrl);
  const manifest = buildManifest(doc, { specUrl, baseOrigin: "https://petstore3.swagger.io" });

  const actionKeys = Object.keys(manifest.actions ?? {});
  console.log(`built: key=${manifest.key}, ${actionKeys.length} actions, auth=${manifest.auth.type}`);
  console.log("sample actions:", actionKeys.slice(0, 8).join(", "));

  // structural checks
  check("key is a valid slug", /^[a-z][a-z0-9_]{1,48}$/.test(manifest.key), manifest.key);
  check("baseUrl is absolute", /^https?:\/\//.test(manifest.connection.baseUrl), manifest.connection.baseUrl);
  check("has many actions", actionKeys.length >= 8, String(actionKeys.length));
  check("auth detected (not none)", manifest.auth.type !== "none", manifest.auth.type);
  check("source provenance stamped", manifest.source?.type === "openapi" && manifest.source.reviewed === false);

  // a known operation: addPet (POST /pet) → has json body templated from schema
  const addPet = manifest.actions?.addpet ?? manifest.actions?.add_pet ?? Object.values(manifest.actions ?? {}).find((a) => a.request.path === "/pet" && a.request.method === "POST");
  check("addPet mapped as POST /pet json", !!addPet && addPet.request.method === "POST" && addPet.request.bodyType === "json");
  check("addPet body is templated", !!addPet && typeof addPet.request.body === "object");

  // a path-param op: getPetById (GET /pet/{petId}) → path templated, petId required input
  const getById = Object.values(manifest.actions ?? {}).find((a) => a.request.method === "GET" && a.request.path === "/pet/{{input.petId}}");
  check("path param templated to {{input.petId}}", !!getById, getById ? "" : "not found");
  check("petId is a required input", !!getById && (getById.input as any)?.required?.includes("petId"));

  // THE point: the generated manifest validates against the schema
  const v = validateManifest(manifest, schemaPath);
  check("generated manifest is SCHEMA-VALID", v.valid, v.errors.slice(0, 6).join(" | "));

  console.log(`\n${pass} passed, ${fail} failed`);
  process.exitCode = fail ? 1 : 0;
}

main();
