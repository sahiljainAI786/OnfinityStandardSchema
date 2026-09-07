# Onfinity AI Orchestration — Implementation Guide (grounded to live tables)

> Companion to `Onfinity Ai Orchestration Solution Architecture Blueprint.pdf` and `tools_Guidelines.md`.
> Schemas verified against the refreshed export in
> `ChatBot_APIServices_Library/AIChatBot_APILibrary_*.xml` (2026-06-10). The live runtime model is the
> **`VAI01_OAgent` family** — confirmed by the `VAI01_AIOrchestrationSetup` window.

---

## 0. Authoritative tables (confirmed from window `VAI01_AIOrchestrationSetup`)

| Tab (seq) | Table | Parent FK |
|---|---|---|
| 10 · AI Orchestration | `VAI01_AIOrchestration` | — (root) |
| 20 · AI Agent | **`VAI01_OAgent`** | `VAI01_AIOrchestration_ID` |
| 30 · Agent Tool | **`VAI01_OAgentAPI`** | `VAI01_OAgent_ID` |
| 40 · Agent Delegation | **`VAI01_OAgentDelegation`** | `VAI01_OAgent_ID` |

> The separate `VAI01_Agent` / `VAI01_AgentAPI` / `VAI01_AgentDelegation` tables exist but are **NOT** used
> by the orchestration window — do not use them. The blueprint's §6 names (`VAI01_OAgent`,
> `VAI01_OAgentAPI`, `VAI01_OAgentDelegation`) are correct; the older guide draft that pointed at the
> `VAI01_Agent` family was wrong and has been superseded.

**Runtime/monitoring tables** (populated at execution, not authored by hand):
`VAI01_AIOrchestrationStatus` (running-orchestration state + supervisor command/params),
`VAI01_AgentCommandStatus` (agent→agent command trail: commanding/commanded/parent command + status —
the audit log), `VAI01_AIOrchestration_Access` (role-based access via `AD_Role_ID`).

---

## 1. Live runtime model

```
VAI01_AIOrchestration                         (program / root; Value = search key, VAI01_Task = objective)
   └─ VAI01_OAgent_ID ──► VAI01_OAgent        (the SUPERVISOR agent for this orchestration)
                                               AgentType LOV: SU Supervisor | AC Action | MO Monitoring | QC QC
VAI01_OAgent  (one row per agent; ALL agents are rows here, parent = the orchestration)
   ├─ VAI01_OAgentAPI         (child: agent ↔ tool/method bindings; ordered)
   ├─ VAI01_OAgentDelegation  (child: supervisor → target agent + rule)   [supervisors only]
   ├─ VAI01_AgentKnowledgeBase_ID ─► VAI01_AgentKnowledgeBase → VAI01_AgentKBLine
   ├─ VAI01_CentralKnowledgebase   (shared org KB reference, String)
   └─ VAI01_LLMConfiguration_ID ─► VAI01_LLMConfiguration  (LLM binding; provider OA = Open AI)

Tool Library (capability catalog, agent-independent — create tools here FIRST):
   VA101_API ──► VA101_APIMethod ──► VA101_MethodParameter ──► VAAPI execution
```

Runtime flow (blueprint §12): Orchestration → Supervisor → evaluate delegation rules → select
specialist → resolve tool bindings → VAAPI call → interpret response → validate → approval if needed →
execute → monitor (`*Status` tables) → audit (`VAI01_AgentCommandStatus`).

---

## 2. Column-accurate schemas (audit cols omitted; `*` = mandatory)

### VAI01_AIOrchestration — the program/root
| Column | Type | Notes |
|---|---|---|
| `Value` | String(60) | unique orchestration search key |
| `VAI01_Task` (id) | String(100) | business objective |
| `Description` | String(255) | summary |
| `VAI01_OAgent_ID` | Table Direct → VAI01_OAgent | the **Supervisor** agent |
| `IsActive` * | Yes-No | runtime enablement |

### VAI01_OAgent — runtime agent (supervisor or specialist) — parent `VAI01_AIOrchestration_ID` *
| Column | Type | Notes |
|---|---|---|
| `VAI01_Name` (id) | String(100) | agent identity |
| `VAI01_AgentType` | List | **SU** / AC / MO / QC |
| `VAI01_Prompt` | Text Long | runtime system prompt |
| `Description` | String(255) | |
| `VAI01_CentralKnowledgebase` | String(255) | shared org knowledge reference |
| `VAI01_AgentKnowledgeBase_ID` | → VAI01_AgentKnowledgeBase | domain KB |
| `VAI01_LLMConfiguration_ID` | → VAI01_LLMConfiguration | LLM binding |
| `VAI01_LLMCustomizeParameter` | Text(2000) | LLM tuning (e.g. temperature) |
| `VAI01_OtherCustomizeParameter` | Text(2000) | runtime tuning |
| `VAI01_CopyAgentLibrary` | Button | clone a reusable agent archetype |
| `IsActive` * | Yes-No | |

### VAI01_OAgentAPI — agent ↔ tool/method binding — parent `VAI01_OAgent_ID` *
| Column | Type | Notes |
|---|---|---|
| `VA101_API_ID` | Table → VA101_API | **tool/service (create here first)** |
| `VA101_APIMethod_ID` | Table Direct → VA101_APIMethod | executable method |
| `VAI01_ExecutionOrder` | Integer | deterministic pipeline order |
| `VAI01_ResponseHandling` | Text(2000) | how to interpret the response / escalation triggers |
| `VAI01_CustomParams` | Text(2000) | runtime param injection |
| `VA101_APIAuthCredential_ID` / `VA101_APIAgentAuthCredential_ID` | Table Direct | credentials |
| `IsActive` * | Yes-No | |
> Also carries `VIS_API_ID` / `VIS_APIMethod_ID`. **Registry — DECIDED 2026-06-11:** use **`VA101_API`**
> (module "API Library", window `VA101_ToolLibrary`) as the canonical registry for orchestration & AI
> agents. Bind via `VA101_API_ID` + `VA101_APIMethod_ID`, with auth via `VA101_APIAuthCredential_ID` /
> `VA101_APIAgentAuthCredential_ID`. (`VIS_API` is the core/migration target; `VAI01_API` is the AI
> Assistant's own set — neither is used for orchestration binding.)

### VAI01_OAgentDelegation — supervisor routing — parent `VAI01_OAgent_ID` *
| Column | Type | Notes |
|---|---|---|
| `VAI01_DelegateTo` | Table | target agent |
| `VAI01_Rule` | Text(2000) | delegation contract: intent, required inputs, expected outputs |
| `IsActive` * | Yes-No | |

### Supporting
- **VAI01_AgentKnowledgeBase** → **VAI01_AgentKBLine** (`VAI01_DataType`, `VAI01_DataContent`, parent `VAI01_AgentKnowledgeBase_ID`*).
- **VAI01_LLMConfiguration**: `VAI01_ModelName`(id), `VAI01_LLMProvider` (OA=Open AI), `VAI01_APIEndpoint`, `VAI01_APIKey`, `VAI01_Region`, `VAI01_Timeout`, `VAI01_DefaultParameters`, `VAI01_RetryPolicy`; `VAI01_VerifyConfigurations` button.
- **Tool Library**: `VA101_API` (Name, BaseUrl, Protocol, AuthType AK/NA/OA, AuthProvider) → `VA101_APIMethod` (Name, Path, Method P/G, RequestFormat, ResponseForm, RateLimit) → `VA101_MethodParameter` (Name, DataType, Location B/H/P/Q, Required).

---

## 3. VAAPI tool catalog → Tool Library rows (`VA101_*`)

The VAAPI Services doc (84 pp) defines ~49 POST methods at `https://<host>/api/VAAPI/Service/<Method>`
with `accessKey`/`secretKey` headers. Register them as **one `VA101_API`** (e.g. Name="VAAPI Core
Service", BaseUrl=`.../api/VAAPI/Service`, AuthType=AK) + one `VA101_APIMethod` per service. The methods
allowed by `tools_Guidelines.md`:

| `VA101_APIMethod.Name` | VAAPI endpoint | Purpose / key body params |
|---|---|---|
| GetRecordV2 | `/GetRecordV2` | read — `selectQuery`, `tableName` |
| InsertRecordM3 | `/InsertRecordM3` | create (object values) — `colName`,`tableName`,`values`,`includeStandardCols` → PK or `NotInserted` |
| UpdateRecordM3 | `/UpdateRecordM3` | update via M-Class object values |
| UpdateRecordM2 | `/UpdateRecordM2` | update via query |
| GetParameters | `/GetParameters` | `processValue`,`AD_Client_ID`,`AD_Org_ID` → AD_Process_ID + param list |
| RunProcess | `/RunProcess` | `AD_Process_ID`/`processValue`,`Record_ID`,`Param` → job message |

Prerequisite auth chain (register too): `Login` → `GetClients` → `GetOrgs` → `InitSession` →
(`VerifySessionToken`/`CheckAccess`); plus `GetApiToken`/`VerifyToken` for token auth.

---

## 4. Governance binding (tools_Guidelines.md + blueprint §10)

The model **never writes raw INSERT/UPDATE/DELETE SQL** — it calls mapped VAAPI methods only.

- **Read** (GetRecordV2) — no approval; map freely.
- **Validate** — GetRecordV2 + checks, isolated, before any write.
- **Write** (InsertRecordM3 / UpdateRecordM3·M2) — only after validation + impact explanation + confirmation.
- **Process/Post** (RunProcess) — approved processes only; **posting & period-close = mandatory approval**.
- **Delete/Reset** — restricted; don't map to specialists by default.
- Per-binding controls in `VAI01_OAgentAPI`: `VAI01_ExecutionOrder`, `VAI01_ResponseHandling`, `VAI01_CustomParams`.
- **Only Supervisor (SU) agents hold `VAI01_OAgentDelegation` rows.** Specialists execute and return.

---

## 5. Build sequence (per orchestration)

1. **Tool Library** — register VAAPI `VA101_API` + needed `VA101_APIMethod`/`VA101_MethodParameter` (once, shared). **Tools must exist in `VA101_API` before they can be mapped in `VAI01_OAgentAPI`.**
2. **LLM config** — one `VAI01_LLMConfiguration`; press `VAI01_VerifyConfigurations`.
3. **Orchestration** — `VAI01_AIOrchestration` (Value, VAI01_Task, Description).
4. **Supervisor agent** — `VAI01_OAgent` (AgentType=SU, parent = the orchestration); set LLM + KB + governing prompt; then set `VAI01_AIOrchestration.VAI01_OAgent_ID` to it.
5. **Specialist agents** — `VAI01_OAgent` rows (AC/MO/QC) under the same orchestration; focused prompt + KB each.
6. **Tool bindings** — `VAI01_OAgentAPI` rows under each agent → only its allowed `VA101_API`/method, with ExecutionOrder + ResponseHandling.
7. **Delegation** — `VAI01_OAgentDelegation` rows under the Supervisor only: DelegateTo + Rule.
8. Activate; runtime status surfaces in `VAI01_AIOrchestrationStatus` / `VAI01_AgentCommandStatus`.

---

## 6. Worked first example — Finance Supervisor + Posting-Validation specialist

**Orchestration** (`VAI01_AIOrchestration`): `Value`=`FIN_IMPL_001`, `VAI01_Task`="Guided Finance module
implementation & posting readiness", `VAI01_OAgent_ID`=Finance Supervisor, `IsActive`=Y.

**Supervisor** (`VAI01_OAgent`): `VAI01_Name`="Finance Supervisor", `VAI01_AgentType`=`SU`, parent
`VAI01_AIOrchestration_ID`=FIN_IMPL_001, prompt: *"You orchestrate Finance implementation. Discover →
validate → recommend → request approval → delegate execution → verify. Never mutate ERP data directly;
delegate writes to specialists, require user confirmation for any write and mandatory approval for
posting/period-close. Route via delegation rules."*

**Posting Validation specialist** (`VAI01_OAgent`): Type=`MO`, same orchestration parent, prompt scoped
to read+validate accounting schema / period / dimension readiness via GetRecordV2 only; returns a
readiness verdict, no writes.

**Tool bindings** (`VAI01_OAgentAPI`, under Posting Validation): GetRecordV2, ExecutionOrder=1,
ResponseHandling="parse rows; flag missing schema/period/dimension". (A GL Action agent, type AC, would
additionally get InsertRecordM3 / RunProcess with approval gating.)

**Delegation** (`VAI01_OAgentDelegation`, under Supervisor): `VAI01_DelegateTo`=Posting Validation,
`VAI01_Rule`="Use to validate accounting schema & period readiness before any GL setup execution.
Input: target org/client + accounting schema. Output: readiness report (pass/fail + gaps). No mutations."

Full specialist roster to expand into (blueprint §9): Discovery/Blueprint, Tenant&Org, Role&Access, GL,
AP, AR, Tax, Banking, Asset Mgmt, Posting Validation, Period Close, Reconciliation, Reporting,
Migration, Go-Live Validation, Audit&Compliance.

---

## 7. Open items to confirm before inserts
1. **AD_Client_ID / AD_Org_ID** for the target tenant (mandatory on every row).
2. **VAAPI host + access/secret keys**, and the AD_Process_IDs for any RunProcess steps.
3. **Insert path**: VAAPI `InsertRecordM3` vs. direct window entry vs. SQL import templates (blueprint Phase 2).
4. Whether to seed reusable agent archetypes via `VAI01_CopyAgentLibrary` (is an agent library already populated?).
5. Tool-binding API pair: `VA101_*` (assumed) vs `VIS_*` core API columns in `VAI01_OAgentAPI`.
