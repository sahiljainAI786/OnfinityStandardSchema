# Platform Foundation — Knowledge Base

> Maps to `VAI01_AgentKnowledgeBase` / `VAI01_AgentKBLine` for the Foundation Setup Supervisor.
> Plain-language domain knowledge for guiding a user new to Onfinity. Verify table/field names against
> the live data dictionary before writing.

## 1. What "Platform Foundation" is
The minimal platform scaffolding every Onfinity deployment needs before any module (or the Accounting
Foundation) can be configured: the operating Organization(s), currencies, the fiscal calendar, document
numbering, and user roles/access. Set up once, first.

## 2. The tasks (dependency order)
1. **Organization** (needs the Client to exist) — the operating unit(s) the company runs as.
2. **Currency & Conversion Rates** — the currencies used and their exchange rates.
3. **Calendar & Periods** (needs Organization) — the fiscal year and its accounting periods.
4. **Document Types & Number Sequences** (needs Organization) — document kinds and how they're numbered.
5. **Roles & Access** (needs Organization) — security roles, what each can access, and user assignments.
6. **Validation / Readiness** — confirm the foundation is complete before modules proceed.

## 3. Concept glossary (plain language)
- **Client / Tenant** — the company instance. Usually already created; everything below lives under it.
- **Organization** — an operating unit within the client (a company, branch, or legal entity). Transactions
  belong to an organization.
- **Currency** — a unit of money used by the business. The **base currency** is the main one.
- **Conversion Rate** — the exchange rate between two currencies for a date/type; needed for any non-base currency.
- **Calendar / Year / Period** — the fiscal calendar, its year(s), and the accounting periods (usually
  monthly) that documents post into. Periods must be **open** to post.
- **Document Type** — a kind of document (sales order, purchase invoice, etc.); controls behavior and numbering.
- **Number Sequence** — the automatic numbering pattern (prefix + running number) for a document type.
- **Role** — a security profile; defines what a user can see and do, and in which organizations.
- **User / User Role** — a login, and the role(s) assigned to it.

## 4. Recommended defaults (for unsure users)
| Choice | Recommended default | Why |
|---|---|---|
| Organization code | derive a short code from the name (e.g., "Acme India" → ACME-IN) | consistent, readable |
| Base currency | the company's home country currency | matches where the business operates |
| Period frequency | Monthly | standard for most businesses |
| Fiscal year start | the company's actual fiscal year start (often Jan or Apr) | matches statutory reporting |
| Number sequence | prefix from doc type + sensible start number | predictable, human-readable document numbers |
| Roles | start from standard role templates, then tailor | faster and safer than building from scratch |

## 5. Validation rules (check before writing)
- Organization **code** must be unique within the client.
- A **base currency** must be set before conversion rates or the accounting schema.
- Periods must be **generated and open** for the fiscal year before posting.
- Number sequences must not collide with existing ones.
- A user must have at least one **role with organization access** to operate.

## 6. Cautions
- **Sensitive values** (user passwords, API keys) are collected securely, never echoed back, never stored
  in plain text — use the dedicated secure flow.
- **Role/access grants are security-sensitive** — confirm before granting; follow approval rules.
- Don't delete an organization or currency once it has transactions — deactivate instead.

## 7. Expected tables / processes (confirm against the dictionary)
| Task | Primary tables | Notes |
|---|---|---|
| Organization | `AD_Org`, `AD_OrgInfo` | org + its details |
| Currency & Rates | `C_Currency`, `C_Conversion_Rate` | currencies + exchange rates |
| Calendar & Periods | `C_Calendar`, `C_Year`, `C_Period` | periods often created via a "Create Periods" process (`RunProcess`) |
| Doc Types & Sequences | `C_DocType`, `AD_Sequence` | document kinds + numbering |
| Roles & Access | `AD_Role`, `AD_Role_OrgAccess`, `AD_User`, `AD_User_Roles` | roles, access, users, assignments |
Writes go through mapped VAAPI tools (`InsertRecordM3` / `UpdateRecordM3` / `RunProcess`) — never raw SQL.

## 8. Mini-FAQ
- *"What's the difference between Client and Organization?"* — Client = the whole company instance;
  Organization = an operating unit inside it. You configure organizations under the client.
- *"Do I need more than one organization?"* — Only if the business has separate branches/entities that
  need separate books. Start with one if unsure.
- *"Why monthly periods?"* — Most reporting and closing is monthly; it's the safe default.
```
