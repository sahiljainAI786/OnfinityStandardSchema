// Validate a generated manifest against schema/connector-manifest.schema.json (draft 2020-12).
// The factory NEVER returns an unvalidated manifest — a bad mapping must fail loudly.

import { readFileSync } from "node:fs";
import Ajv2020 from "ajv/dist/2020.js";
import type { ConnectorManifest } from "@ip/shared";

export interface ValidationResult {
  valid: boolean;
  errors: string[];
}

export function validateManifest(manifest: unknown, schemaPath: string): ValidationResult {
  const schema = JSON.parse(readFileSync(schemaPath, "utf8"));
  // strict:false so unknown formats (uri/email) are ignored rather than throwing
  const ajv = new Ajv2020({ strict: false, allErrors: true });
  const validate = ajv.compile(schema);
  const valid = validate(manifest) as boolean;
  const errors = (validate.errors ?? []).map((e) => `${e.instancePath || "/"} ${e.message}`);
  return { valid, errors };
}

export function assertValid(manifest: ConnectorManifest, schemaPath: string): void {
  const r = validateManifest(manifest, schemaPath);
  if (!r.valid) {
    throw new Error(`generated manifest failed schema validation:\n  ${r.errors.join("\n  ")}`);
  }
}
