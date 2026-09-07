# Accounting Foundation — Knowledge Base (consultative)

> Maps to `VAI01_AgentKnowledgeBase` / `VAI01_AgentKBLine` for the Accounting Foundation Supervisor and Advisor.
> This KB is **consultative**: it lets the agents diagnose the customer's business, **propose** a configuration
> with its impact and rationale, and ask only what changes the recommendation — not just collect inputs.
> Verify table/field names against the live data dictionary before writing. These are implementation-guidance
> defaults, not formal accounting advice — material choices should be confirmed with the customer's accountant.

---

## 0. Consultative principle (how to use this KB)
1. **Diagnose first** — establish a light business profile (§2), inferring whatever you can rather than asking.
2. **Propose, don't interrogate** — lead each decision with a recommended value derived from the profile
   (§3–§4), state its **impact** (§5) in business terms, then let the user accept or adjust.
3. **Ask only what changes the recommendation** — if a fact wouldn't alter the proposal, assume the sensible
   default and say so ("I'll assume a single warehouse — tell me if you have more").
4. **Make consequences explicit before high-impact / hard-to-reverse choices** (§5).

---

## 1. What "Accounting Foundation" is and why it comes first
In Onfinity, accounting is **automatic** — nearly every document in every module (invoices, inventory
receipts, payroll, depreciation) posts to the General Ledger. Posting needs three things first: an
**Accounting Schema**, a **Chart of Accounts**, and **posting defaults**. Until these exist, no module can
post. Prerequisites (Platform Foundation): an **Organization**, a **base Currency**, and a **Calendar with
open Periods**.

---

## 2. Business profile — the signals that drive the configuration
Capture these (infer where possible; ask only the few that change the recommendation):

| Signal | Why it matters | Often inferable from |
|---|---|---|
| **What the business does** (services / distribution / manufacturing / retail / projects / mixed) | drives costing method + CoA depth | org name, business-partner setup, the user's words |
| **Holds inventory? makes goods?** | decides if costing matters at all | industry |
| **Single vs multiple legal entities** | one schema + consolidate by org, or multi-schema | # of organizations |
| **Countries of operation** | base currency, tax regime, statutory GAAP | org address/country |
| **Multi-currency transactions?** | conversion rates, FX accounts | countries, customers |
| **Reporting standard** (local GAAP / IFRS / US GAAP) | schema GAAP; possibly a 2nd schema | country, group policy |
| **Scale / complexity** (SMB vs enterprise) | CoA depth, number of warehouses | headcount, org count |

If a signal is unknown and low-impact, **assume the simplest option and state the assumption**.

---

## 3. Decision matrices — business characteristic → recommendation
**Costing method**
| Business type | Recommended | Rationale |
|---|---|---|
| Services / consulting (no inventory) | Standard (nominal) | costing is largely irrelevant; keep it simple |
| Distribution / wholesale | Average (or FIFO if perishable/price-volatile) | stable margins; FIFO when batch/price matters |
| Manufacturing | Standard (with variances) or Average | Standard enables variance analysis; Average if costs are stable |
| Retail (many SKUs) | Average or FIFO | high volume; Average is simplest, FIFO for price-sensitive goods |
| Projects / construction | Standard + project costing | cost tracked per project, not per unit |

**Costing level**: single site → *Client/Organization*; multiple warehouses with genuinely different costs → *Warehouse*. Default to the simplest that fits.

**Number of accounting schemas**: **one** for almost everyone. Consider a second only for true **multi-GAAP** (e.g., local GAAP + IFRS) or where a group needs parallel books. Multi-entity groups usually use **one schema and consolidate by organization**, not multiple schemas.

**Chart of accounts depth**: start from the standard template; add detail only where the business reports on it. Prefer **dimensions** (org, project, cost-center) over exploding the natural-account list.

**Base currency**: the home-country currency of the primary entity. Enable additional currencies + conversion rates only if they actually transact in them.

---

## 4. Business archetypes — recommended starting configurations
Pattern-match the customer to an archetype, then **propose the whole bundle** (and adjust).

- **Services / Consulting firm** — *no inventory.* Schema: home currency, costing **Standard (nominal)**, single schema. CoA: revenue (by service line via dimension), payroll/staff cost, opex; minimal inventory/COGS. Defaults: receivables/payables/bank/tax. *Caution: don't over-build inventory accounts you won't use.*
- **Distributor / Wholesaler** — *buys to resell.* Costing **Average** (or FIFO if perishable), costing level Org (Warehouse if multi-DC with differing costs). CoA: inventory, COGS, purchase price variance, sales. *Caution: pick costing method carefully — it sets how margin is reported.*
- **Manufacturer** — *makes goods.* Costing **Standard** (variance visibility) or Average; costing level Org/Warehouse. CoA: raw materials, WIP, finished goods, manufacturing overhead, variance accounts. *Caution: richest CoA; align with how production reports cost.*
- **Retailer (multi-location)** — Costing **Average/FIFO**; costing level **Warehouse** if store-level cost differs. CoA: sales & COGS by location (via dimension), inventory shrinkage. *Caution: use dimensions for location, not duplicate accounts.*
- **Project / Construction** — Costing **Standard** + **project dimension**; revenue recognition matters. CoA: WIP, project cost/revenue, retention. *Caution: enable the project accounting dimension early.*
- **Multi-entity group** — Multiple organizations under one client; **one schema, consolidate by org**; multi-currency likely; consider a 2nd schema only for multi-GAAP. *Caution: decide consolidation currency + intercompany approach up front.*

---

## 5. Impact / consequence model (state this before deciding)
| Choice | What it affects downstream | Reversible? |
|---|---|---|
| **Base / accounting currency** | every report, all FX, consolidation | ❌ effectively set-once after transactions |
| **Costing method** | inventory valuation, COGS, reported margin | ❌ very hard to change once transactions exist |
| **Costing level** | whether cost is shared or per-location | ⚠ hard to change later |
| **# of accounting schemas** | reporting complexity, performance, parallel books | ⚠ adding later is costly |
| **Chart of accounts structure** | every posting and report | ⚠ accounts deletable only while unused; deactivate, don't delete |
| **Posting defaults** | whether documents post at all | ✅ changeable, but affects live postings — confirm/approve |

The first three are the **high-stakes, hard-to-reverse** decisions — lead with a recommendation, explain impact, and get a clear confirmation.

---

## 6. Concept glossary (plain language)
- **Accounting Schema** — the rulebook for the books: currency, costing, GAAP, calendar.
- **Base / Accounting Currency** — the currency the books are kept in (usually the home currency).
- **Costing Method** — how inventory cost is valued: *Standard* (planned, predictable), *Average* (moving average of actual), *FIFO* (first-in-first-out).
- **Costing Level** — the level cost is tracked at: *Client*, *Organization*, or *Warehouse*.
- **GAAP / Accounting Standard** — the reporting standard the books follow.
- **Chart of Accounts** — the master list of accounts every transaction posts to (the Account element + its values).
- **Dimensions** — extra reporting axes (org, project, cost-center) that avoid exploding the account list.
- **Posting / Account Defaults** — the accounts used automatically when documents post; required for posting to succeed.

---

## 7. Validation rules (check before writing)
- Prerequisites present: Organization + base Currency + Calendar with open Periods.
- One primary accounting schema per client unless multi-GAAP is explicitly required.
- Costing method + level set on the schema before any transactions.
- CoA contains the mandatory natural accounts before posting defaults map.
- All mandatory posting defaults mapped before the Validation task passes.

---

## 8. Pitfalls / anti-patterns
- ❌ Asking the user to pick costing method cold → instead recommend from their business type and explain margin impact.
- ❌ Multiplying accounting schemas when one + dimensions would do → adds complexity and slows posting.
- ❌ Building a huge CoA by duplicating accounts per location/project → use dimensions.
- ❌ Deleting an account that already has postings → deactivate instead.
- ❌ Treating currency/costing as changeable → they are effectively permanent; confirm carefully.

---

## 9. Expected tables / processes (confirm against the dictionary)
| Task | Primary tables | Notes |
|---|---|---|
| Accounting Schema | `C_AcctSchema` (+ `C_AcctSchema_GL`, `C_AcctSchema_Element`) | schema + GL settings + dimensions |
| Chart of Accounts | `C_Element`, `C_ElementValue` | Account element + values; often imported |
| Posting Defaults | schema default-account tables (`C_AcctSchema_Default` / `*_Acct`) | mandatory accounts must be set |
| Validation | (read only) | confirm a test document posts; mandatory defaults present |
Writes go through mapped VAAPI tools (`InsertRecordM3`/`UpdateRecordM3`/`RunProcess`) — never raw SQL.

**Tools per task** (which VAAPI method each Specialist uses; params in Build Standards §B):
| Task / agent | Read & validate | Write |
|---|---|---|
| Discovery (MO) | `GetRecordV2`, `GetParameters` | — |
| Accounting Schema Specialist (AC) | `GetRecordV2` | `InsertRecordM3`, `UpdateRecordM3` |
| Chart of Accounts Specialist (AC) | `GetRecordV2` | `InsertRecordM3`, `InsertMultipleRecordM3` (bulk), `UpdateRecordM3` |
| Account Defaults Specialist (AC) | `GetRecordV2` | — (delegates) |
| Account Defaults Execution Subagent (AC) | `GetRecordV2` | `InsertRecordM3`, `UpdateRecordM3` (gated) |
| Validation (QC) | `GetRecordV2` | — |
Tool Library = **VAAPI Services** (`VA101_API`); do not create new tools or methods.

---

## 10. Mini-FAQ (just-in-time help)
- *"Do I need accounting if I only run HR/Payroll?"* — Yes; payroll posts to the GL.
- *"Can I change the currency/costing later?"* — Practically no, once transactions exist. Choose now.
- *"We operate in 3 countries — multiple schemas?"* — Usually one schema, consolidate by organization, multi-currency on; a 2nd schema only for multi-GAAP.
- *"Standard vs Average vs FIFO?"* — Recommend from the business type (§3); start simple unless they need actual-cost or batch tracking.
