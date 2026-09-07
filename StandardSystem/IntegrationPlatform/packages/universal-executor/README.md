# @ip/universal-executor  (skeleton)

The single generic runtime that executes ANY connector manifest. Packaged as one Activepieces
"piece" so the rebranded AP engine can run every connector without code-per-connector.

## What it does

Given a manifest `action` (or `trigger`) + the user's stored credential + the step input, it:

1. **Renders templates** — resolves `{{input.*}}`, `{{auth.fields.*}}`, `{{auth.tokens.*}}`,
   `{{connection.*}}`, `{{cursor.*}}`, `{{item.*}}`, `{{webhook.url}}`, `{{state.*}}`.
2. **Injects auth** — apiKey/bearer/basic/oauth2/custom, per the manifest's `auth` + `connection.defaultHeaders`.
3. **Sends the HTTP request** — method/path/query/headers/body from the action's `request`.
4. **Paginates** — follows the action's pagination strategy, concatenating items.
5. **Maps errors** — applies `errors[]` → retry / fail / ignore / reauth, with rate-limit backoff.
6. **Returns JSON** — the response flows on as JSON in-flight; nothing is persisted unless an action writes it.

## Modules (to build)

- `template.ts`   — the `{{...}}` renderer + the fixed context contexts.
- `auth.ts`       — credential injection per auth type (OAuth refresh handled by AP's vault).
- `request.ts`    — build + send, honoring `bodyType` (json/form/none).
- `pagination.ts` — cursor / page / offset / link_header loops with `maxPages` guard.
- `triggers.ts`   — webhook verify + dedupe; polling cursor advance.

Types come from [`@ip/shared`](../shared/src/manifest.ts) so the executor and the factory agree on the contract.
