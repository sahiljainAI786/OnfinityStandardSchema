# Onfinity Smart API (master-data wrapper over VAAPI)

A thin **resolver façade** that lets external apps create/update/read Onfinity master data
using **business identifiers** (search keys, names, ISO codes) instead of primary keys.
It resolves those identifiers to the `*_ID` foreign keys internally and then calls the
existing **VAAPI Core Service** — no raw-SQL DB access, no ERP code changes.

> Decided: **wrapper, not native** (don't modify StandardCode) — and **separate IIS app**
> (own app pool) on the **same application server as Onfinity**, calling VAAPI over loopback.
> One Smart API instance per Onfinity install, mirroring VAAPI. The Integration Platform
> (`onfinity.connector.json`) and any external app both consume it as plain REST.

## Why it exists

VAAPI is SQL-level: every write takes raw columns and `*_ID` PKs, so each integration
re-implements the same "look up the FK, then write" dance (exactly what the Time Tracker
had to do: `GetRecordV2 SELECT C_BPartner WHERE Value='VAI159'` → PK → `InsertRecordM3`).
The Smart API centralizes that resolution once.

## Boundary / topology

```
External app ─┐
Mobile/script ─┼──► Onfinity Smart API ──► VAAPI ──► Onfinity DB
iPaaS (AP)  ───┘     (resolver + upsert)    (6 methods)   [all on the Onfinity host/LAN]
                     IIS app, own app pool   loopback HTTPS
```

The Smart API holds the VAAPI `accessKey`/`secretKey` credential; callers authenticate to
the Smart API with their own key (TBD — reuse VAAPI key model or issue Smart-API keys).

## Endpoints (v1 — master data)

| Method & path | Purpose |
|---|---|
| `PUT  /v1/{entity}` | **Upsert by business key** (insert if key is new, else update) |
| `POST /v1/{entity}` | Insert only (errors if key exists) |
| `PATCH /v1/{entity}/{key}` | Partial update of an existing record by key |
| `GET  /v1/{entity}/{key}` | Read one, **FKs returned as identifiers** not PKs |
| `GET  /v1/{entity}?<filters>` | List/search, identifiers in/out |

`{entity}` is a registered manifest (e.g. `BusinessPartner`, `Product`). Adding an entity =
dropping a manifest JSON in `manifests/`, no code.

**Shape:** headless JSON API + a **Swagger UI at `/swagger`** (browsable docs + try-it-out; send
accessKey/secretKey via the Authorize button). **Stateless — no database**: manifests are files on disk,
the resolver cache is in-memory, all reads/writes go to VAAPI. Deploying = copying files. No connection
string, no migrations, no secrets at rest (callers send keys per request).

### Example

```jsonc
PUT /v1/BusinessPartner
{
  "key": "CUST-0042",            // → C_BPartner.Value (upsert identity)
  "name": "Acme GmbH",
  "isCustomer": true,
  "group": "Employees",          // → C_BP_Group_ID  (resolve via C_BP_Group.Value)
  "priceList": "Gold Partner Price List", // → M_PriceList_ID (resolve via M_PriceList.Name)
  "paymentTerm": "Immediate",    // → C_PaymentTerm_ID (resolve via C_PaymentTerm.Value)
  "taxId": "DE123456789",
  "email": "ap@acme.example"
}
→ 200 { "mode": "created", "key": "CUST-0042", "C_BPartner_ID": 1040912 }
```

## Resolver / upsert flow

```
PUT /v1/BusinessPartner
 1. Load manifest "BusinessPartner".
 2. Validate required api fields are present; coerce scalar types.
 3. Per field:
      scalar → pass through
      list   → map human label → stored code via manifest.list (error if not in LOV)
      ref    → GetRecordV2 {
                 tableName: ref.table,
                 selectQuery: "SELECT <ref.returns> FROM <ref.table>
                               WHERE <ref.matchOn> = :val AND IsActive='Y' AND AD_Client_ID=:client"
               }
               exactly 1 row → use id  ·  0 → 422 MISSING_REF  ·  >1 → 422 AMBIGUOUS_REF
               (cache by (table, matchOn, val))
 4. Existence check: GetRecordV2 SELECT key-PK WHERE <keyColumn> = :key
      0 rows → InsertRecordM3 { tableName, colName[], values:[{resolved}], includeStandardCols:true }
      1 row  → UpdateRecordM3 { tableName, colName[], values:[{resolved}], record_ID: foundId }
      >1     → 409 AMBIGUOUS_KEY
 5. Return { mode, key, <pk> }.
```

- **`includeStandardCols: true`** lets VAAPI populate `AD_Client_ID`/`AD_Org_ID`/audit columns
  from the credential's session, so callers don't pass tenant/org for the common case.
- **Tenant scoping:** every resolver lookup is filtered by the session's `AD_Client_ID`.
- **Reads** map PKs back to identifiers by selecting the referenced table's identifier column
  (or by `JOIN`) so callers never see `*_ID`.

## Dry run

Append `?dryRun=true` to any `PUT`/`POST`/`PATCH`. The service does everything *except* the write —
validates, resolves all refs/lists, runs the existence check — and returns what it *would* do plus the
fully resolved column map. Read-only against VAAPI, safe against production.

```
PUT /v1/BusinessPartner?dryRun=true   → { "mode":"would-update", "recordId":1040878, "dryRun":true,
                                          "resolved": { "C_BP_Group_ID":1000035, "M_PriceList_ID":1000183, ... } }
```

## VAAPI selectQuery gotchas (verified live on erplive 2026-06-27)

`GetRecordV2` runs the raw `selectQuery` but wraps/augments it — so:
- **No `FETCH FIRST … ROWS ONLY` / `LIMIT`** — silently returns `data:null`. The resolver uses plain
  `WHERE` (exactly-one is enforced by counting returned rows), so it's unaffected.
- **No explicit `AD_Client_ID=…`** in resolver lookups — VAAPI already scopes to the session's client;
  adding it can null the result. The resolver omits it by design.
- `IsActive = 'Y'` is fine. Column keys come back **lowercased** (handled — dicts are case-insensitive).

All four `C_BPartner` ref `matchOn` keys verified live: `C_BP_Group.Value`, `M_PriceList.Name`,
`C_PaymentTerm.Value`, `C_Country.CountryCode`.

## Hard rules (non-negotiable)

1. **Exactly-one resolution** — a ref/key must match one active row. Never silently pick on 0/>1.
2. **`IsActive='Y'`** on every resolver lookup.
3. **Missing-ref policy** = error by default; opt-in `createIfMissing` per field later (not v1).
4. **Reuse VAAPI's write path** (`InsertRecordM3`/`UpdateRecordM3`) — never touch the DB directly.
5. **No callout assumptions** — confirm whether `InsertRecordM3` fires model callouts; for master
   data the caller supplies values explicitly, so this rarely bites (it's the trigger to ever go
   native). See open items.

## Authorization

**The Smart API has no authorization of its own — it is authorization-transparent.** Auth *is*
VAAPI's role/session model, preserved by **key pass-through**.

- The caller's `accessKey`/`secretKey` resolve to a `VAAPI_SessionToken` carrying `AD_User_ID`,
  `AD_Client_ID`, `AD_Org_ID`, `AD_Session_ID` — i.e. a full user/role/tenant/org context.
- The Smart API forwards that **same** key pair, verbatim, on every underlying call
  (`GetRecordV2` / `InsertRecordM3` / `UpdateRecordM3`). It holds **no identity of its own**, so it
  **cannot elevate** (only does what the caller's role already could) and **cannot bypass** (VAAPI
  runs the same role checks it would for a direct call). Role enforcement happens *inside* VAAPI,
  downstream of our hand-off, so the wrapper neither adds nor removes any permission.
- This is exactly why pass-through was chosen over Smart-API-issued keys: issuing our own keys mapped
  to one backing credential would collapse every caller onto a single shared role and silently destroy
  per-caller authorization.

Two consequences to know:
1. **We inherit VAAPI's granularity, whatever it is.** Whether VAAPI applies MRole access-SQL to a raw
   `GetRecordV2` select, or enforces table/record access on programmatic writes, is a VAAPI detail —
   it behaves identically through the wrapper as direct. Confirm once against VAAPI itself; not a
   wrapper concern.
2. **Resolution needs read access to reference tables.** The resolver makes *extra* reads the caller
   wouldn't make directly (e.g. `SELECT … FROM C_BP_Group WHERE Value='Standard'`). Those run under
   the caller's role too — so the role must have **read visibility to the lookup tables**
   (C_BP_Group, M_PriceList, C_PaymentTerm…) or resolution returns `MISSING_REF`. Same auth model,
   one new prerequisite.

A future Smart-API-level scope layer (e.g. "this app may touch BusinessPartner but not Product") would
be *additive* — it can only tighten, never loosen, what VAAPI already enforces.

## Manifests

Generated from the AD metadata already in the repo
(`Wiki/Tables/<Table>.md` / `ChatBot_APIServices_Library/VAAPI_TableColumn.xml`):
the `References` column gives each FK's target table; `IsIdentifier`/`Value` gives its match key;
the *List values* section gives label→code maps.

- `manifests/C_BPartner.json` — Business Partner (customer/vendor/prospect), v1 curated subset.

## Open items

1. ~~**Caller auth**~~ — DECIDED: reuse VAAPI accessKey/secretKey (pass-through). See Authorization.
2. **`InsertRecordM3` callouts** — does it run model logic or just column writes? (decides native-later.)
3. **`matchOn` per ref** — defaults below assume `Value`/`Name`/`CountryCode`; verify against each
   referenced table's `IsIdentifier` columns.
4. **Tech stack** — .NET (co-located, easy VAAPI calls) vs Node (reuse IntegrationPlatform TS types).
