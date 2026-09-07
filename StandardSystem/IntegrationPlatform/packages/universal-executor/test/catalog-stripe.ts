// Proves the executor runs a REAL catalog manifest against a REAL API end-to-end.
// Loads connectors/stripe.connector.json and calls create_customer with a bogus key:
// Stripe returns 401 → the manifest's error rule should map it to outcome "reauth".
// Run: npx tsx test/catalog-stripe.ts

import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { dirname, join } from "node:path";
import { executeAction, type Credential } from "../src/index.js";
import type { ConnectorManifest } from "@ip/shared";

const here = dirname(fileURLToPath(import.meta.url));
const manifestPath = join(here, "../../../connectors/stripe.connector.json");
const stripe = JSON.parse(readFileSync(manifestPath, "utf8")) as ConnectorManifest;

const cred: Credential = { fields: { secret_key: "sk_test_bogus_key_for_test" } };

const res = await executeAction(stripe, "create_customer", { email: "test@example.com", name: "Test" }, cred);

console.log("manifest:", stripe.key, "action: create_customer");
console.log("status:", res.status, "outcome:", res.outcome);
console.log("mapped message:", res.message);

const ok = res.status === 401 && res.outcome === "reauth";
console.log(ok ? "\nPASS — real manifest + real Stripe API + error mapping all worked" : "\nFAIL");
process.exit(ok ? 0 : 1);
