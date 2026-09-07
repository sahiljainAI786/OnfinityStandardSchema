# @ip/connector-factory  (skeleton)

Turns an OpenAPI/Swagger spec into a connector manifest (`*.connector.json`) that conforms to
[`schema/connector-manifest.schema.json`](../../schema/connector-manifest.schema.json).

## Contract

```
factory(specUrlOrFile) -> ConnectorManifest        # validated against the schema before it returns
```

## Pipeline (to build)

1. **Load** the OpenAPI doc (`@apidevtools/swagger-parser`, deref `$ref`s).
2. **Map** each operation → action/trigger, auth scheme → `auth`, servers → `connection.baseUrl`.
   The LLM does the *labeling* here (which ops are actions vs triggers, friendly titles, input/output
   trimming) — a narrow, verifiable job because the endpoints come from the spec, not the model.
3. **Validate** the emitted manifest against the schema (ajv). Reject on any error — never publish unvalidated.
4. **Stamp** `source` provenance (specUrl, specHash, generatedBy, reviewed:false).

A human reviews (`source.reviewed = true`) before the catalog publishes it.

## Why OpenAPI-first

The model maps a machine-readable spec rather than inventing endpoints — accurate and scalable to
hundreds of apps. Docs-scraping is a later fallback for apps with no spec.
