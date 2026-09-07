# Connector manifest — design notes

The manifest is the load-bearing contract of the platform. The **AI factory produces** it from an
OpenAPI spec; the **Universal REST Executor consumes** it at runtime with a user's stored credential.
A connector is *data conforming to a schema*, not generated/deployed code — adding one is an insert,
not a build. Onfinity is described with the same schema (actions → VAAPI, triggers → outbound events).

- Schema: [`connector-manifest.schema.json`](connector-manifest.schema.json) (JSON Schema draft 2020-12)
- Worked example: [`../connectors/stripe.connector.json`](../connectors/stripe.connector.json) — validates clean.

## The shape

```
manifest
├─ identity        key, name, version, vendor, categories, docs/logo
├─ source          provenance: spec URL/hash, generator, reviewed flag  → regeneration & audit
├─ connection      baseUrl (+ per-credential variables), default headers
├─ auth            type + credential-form fields + how the secret is injected + a test action
├─ rateLimit       enforced per stored credential, not globally
├─ paginators      reusable named pagination strategies
├─ actions{}       things the platform INVOKES on the app  (workflow "do" steps)
└─ triggers{}      events the app EMITS into the platform   (workflow starts) — webhook | polling
```

## Decisions worth knowing

**JSON in-flight, no DB binding.** Actions/triggers carry `input`/`output` **JSON Schemas** purely to
drive the step-config form and the mapping UI's field picker. The payload is never persisted to a
relational model — it only lands somewhere if an action explicitly writes it (e.g. an Onfinity
"insert record" action calling VAAPI). This is what lets data flow through without modeling it first.

**Templating is the only "logic."** Every request string can contain `{{...}}` resolved against a fixed
runtime context: `input.*`, `auth.fields.*`, `auth.tokens.*`, `connection.*`, `cursor.*`, `item.*`,
`webhook.url`, `state.*`. No arbitrary code — keeps AI-generated manifests reviewable and safe to run.

**Auth is declarative + conditional.** `auth.type` switches required sub-objects (`apiKey` / `basic` /
`oauth2`). `auth.fields` is what the user fills when connecting; `secret: true` fields are vault-encrypted
and never returned to the UI. `auth.test` names a cheap action for the "Test connection" button.

**Triggers are first-class both ways.** A trigger is `webhook` (subscribe/unsubscribe requests +
signature verification + dedupe key) or `polling` (request + cursor + interval). Stripe's example ships
both: real-time `payment_intent.succeeded` via webhook, and a `created[gte]` polling fallback. The same
model expresses Onfinity's outbound event calls as a webhook trigger.

**Built-in safety rails.** `rateLimit` (per-credential backoff, honors `Retry-After`), `errorMap`
(normalizes HTTP responses to `retry|fail|ignore|reauth` so workflows handle all connectors uniformly),
`pagination.maxPages` (logged truncation, no runaway loops), `source.reviewed` (gate AI output before publish).

## Known gotcha (already handled)

JSON Schema's `additionalProperties: false` does **not** see properties introduced only inside
`if/then`/`allOf`. So `webhook`/`polling` are declared as base properties on `trigger` and *refined* by
the conditionals. Same pattern applies if you add conditional keys to `auth` or `action`.

## Validate

```bash
pip install jsonschema
python -c "import json,io; from jsonschema import Draft202012Validator as V; \
s=json.load(io.open('schema/connector-manifest.schema.json',encoding='utf-8')); \
d=json.load(io.open('connectors/stripe.connector.json',encoding='utf-8')); \
V.check_schema(s); print('errors:', list(V(s).iter_errors(d)) or 'none')"
```

## Open questions this surfaces for the next pass

1. **Mapping expression language** — `{{...}}` covers field references; do we need transforms
   (formatting, defaults, simple conditionals) inside the manifest, or are those a separate workflow step?
2. **OAuth token storage** — `auth.tokens.*` implies the executor manages refresh; confirm Activepieces'
   built-in OAuth handler covers this so the manifest only *declares* the flow.
3. **Onfinity manifest** — draft `onfinity.connector.json` next: VAAPI actions (insert record, trigger
   workflow, call AI agent) + an outbound-event webhook trigger, to prove the symmetric end of the model.
4. **Stateful webhook ids** — `{{state.webhook_id}}` in unsubscribe implies per-subscription state the
   platform persists. Confirm where that lives (Activepieces stores trigger state per flow).
