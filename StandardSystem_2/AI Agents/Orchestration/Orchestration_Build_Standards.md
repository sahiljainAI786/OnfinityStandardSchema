# Onfinity Orchestration — Build & Runtime Standards

> Canonical rules distilled from **"Onfinity Orchestration Script Guidelines"** (the user's standard for
> generating orchestration setup scripts). Governs BOTH how setup SQL is generated AND how the runtime
> agents must behave. Load the runtime sections into agent knowledge bases; apply the build sections when
> generating `VAI01_*` / `VA101_*` import scripts.

## A. Script-generation standards (for producing import SQL)

**A1. Tenant context — never hardcode IDs.** Resolve dynamically by Name:
`AD_Client_ID` ← `AD_Client.Name`, `AD_Org_ID` ← `AD_Org.Name`, `AD_User_ID` ← `AD_User.Name`.
Default tenant: Client **"Onfinity Demo"**, Org **"Onfinity Demo HQ"**, User **"Onfinity Demo Admin"**.
If any lookup returns nothing, fail with a clear error.

**A2. Primary keys — use `AD_Sequence`, never hardcode.** Fetch `CurrentNext`/`IncrementNo` for the table
(`IsTableID='Y'`, `IsAutoSequence='Y'`); stale-guard `v_new_id := GREATEST(v_current_next, NVL(v_max_existing_id,0)+1)`;
update the sequence **only after** a successful insert.

**A3. Audit columns** on every insert where present: `AD_Client_ID, AD_Org_ID, IsActive, Created, CreatedBy,
Updated, UpdatedBy` (use `SYSDATE` + resolved `v_user_id`).

**A4. Duplicate prevention — read before write.** Check existence first; update if present, insert if not.
Business keys: `Value`, `Name`, `Export_ID`, `AD_Client_ID`, `AD_Org_ID`, parent orchestration ID, parent
agent ID, child agent ID, API method ID.

**A5. Live metadata validation — never assume a column exists.** Before any insert/update/select, validate
the table and every referenced column against live metadata (`AD_Table`/`AD_Column`). Provide helpers
`table_exists`, `column_exists`, `require_column`. Build a **whitelist of active columns**; drop
non-whitelisted optional columns; STOP if a required value maps to no confirmed column or a mandatory
column is missing. Do not assume `Value/SearchKey/DocumentNo/Description/Name/Code/IsDefault` exist
(e.g. `C_Element` may have no `Value`).

**A6. LLM configuration — don't hardcode the ID.** Preferred model **`gpt-4o-mini`**; resolve
`VAI01_LLMConfiguration_ID` dynamically (`IsActive='Y'`, `LOWER(VAI01_ModelName)='gpt-4o-mini'`); fall back
to another active config if allowed; stop with a clear error if none active.

**A7. Oracle specifics.** `SET DEFINE OFF` when prompts/text contain `&`. Scripts must be Oracle-compatible,
re-runnable, tenant-aware, AD_Sequence-compliant, duplicate-safe, metadata-aware, and clear on failure.

**A8. Output package** per script: the SQL file, a README, what it creates/updates, verification queries,
rollback/safe-re-run notes, required preconditions, and an explicit note on what it does **not** change.

## B. Tool Library binding standards (`VA101_*` / `VAI01_OAgentAPI`)

- Tool Library window **"Tool Library"**, API **"VAAPI Services"** (`VA101_API` → `VA101_APIMethod` →
  `VA101_MethodParameter`). **Do not create a new Tool Library or invent methods** — reuse existing.
- Resolve `VA101_API_ID` dynamically. `VA101_AuthRequired` defaults to **`Y`**. `VA101_APIMethod` has **no
  `Value` column** — resolve methods by `Name`.
- **Label limits:** `VA101_API.Label` ≤ **10** chars; `VAI01_OAgentAPI.Label` ≤ **30** chars.
- **Bind tools on `VAI01_OAgentAPI`**, and set `VAI01_CustomParams` with the method's JSON template (don't
  leave blank when the method supports params):

| Method | `VAI01_CustomParams` template |
|---|---|
| GetRecordV2 | `{ "selectQuery":"", "tableName":"" }` |
| InsertRecordM3 | `{ "colName":"", "tableName":"", "fileName":"", "values":"", "includeStandardCols":true, "saveFileInDisk":false }` |
| UpdateRecordM3 | `{ "tableName":"", "colName":"", "values":"", "record_ID":null }` |
| GetParameters | `{ "processValue":"", "AD_Client_ID":null, "AD_Org_ID":null }` |
| InsertMultipleRecordM3 | `{ "tableName":"", "colName":"", "values":"", "includeStandardCols":true, "saveFileInDisk":false, "fileName":"", "ApplyDBTrx":false }` |
| UpdateORDeleteRecord | `{ "pid":"integer", "selectQuery":"string", "tableName":"string" }` |

- **Method parameter datatype** (`VA101_DataType`): String→STR · Integer/Number→INT · Boolean→BOL ·
  Object→OBJ · Object[]→OBA · String[]/byte→STA. Include name, location, datatype, mandatory flag, sequence.

## C. Agent structure standards

- **Agent type** (`VAI01_OAgent.VAI01_AgentType`, List): resolve codes from the live LOV — current values
  **SU=Supervisor, AC=Action, MO=Monitoring, QC=QC**. Supervisor → SU; other agents → AC or MO by behavior.
- **Supervisor header binding:** `VAI01_AIOrchestration.VAI01_OAgent_ID` = the Supervisor OAgent; it must
  belong to the same orchestration, have type SU, and be active.
- **Runtime hierarchy:** `AURA / User → Supervisor → First-level Specialist → Specialist Subagent → Mapped VAAPI Tools`.
  Don't bypass the first-level specialist unless explicitly required.

## D. Runtime behavior standards (load into agent KBs / prompts)

**D1. Supervisor agent — orchestrate, don't do** (§9). The Supervisor MUST:
- Understand the request, preserve context, and determine the **request phase**:
  `readiness → option discovery → scope collection → proposal → confirmation → execution → verification → failure handling → handover`.
- Route to the correct first-level specialist; enforce **read-before-write, validate-before-create,
  confirm-before-create, duplicate-prevention, verify-after-create**; keep responses business-friendly.
The Supervisor MUST NOT: collect specialist field-level values; recommend specialist-specific configuration
values; create/update records; call write tools; expose internal IDs or tool names; ask users for internal IDs.

**D2. Specialist agent — implementation consultant** (§10). For each setup flow the specialist explains
*what it means, why the value is needed, its business impact, the recommended value (if safe), the risk of a
wrong value, and the confirmation required*, following the flow:
`collect → explain → validate → recommend → propose → confirm → execute → verify`.

**D3. References & LOVs — show labels, store codes, never ask for IDs** (§14).
- For **List/LOV** fields: resolve the reference dynamically via `AD_Table → AD_Column.AD_Reference_Value_ID
  → AD_Reference → AD_Ref_List`. Show only `AD_Ref_List.Name` to the user; store only `AD_Ref_List.Value`.
  Map a user's label to its Value before saving; validate a user-supplied Value exists. Never infer the
  reference from table/column name.
- For **Table / Table Direct / Search** fields: never ask the user for the internal ID. Resolve the
  referenced table/key from metadata, show the business identifier (Name/Value), and store the resolved ID.
  If a reference can't be resolved, stop and report a metadata blocker.

**D4. Readable responses** (§18): business language first. For numbered options, put each option on its own
line. Never expose to business users: internal IDs, tool names, raw payloads, SQL, process IDs, table IDs,
or routing details.

**D5. Error handling — no silent retries** (§19). On failure report: failed phase, failed table, failed
operation, exact tool/API/DB error, verification result, whether partial records exist, recommended admin
handover, and whether retry is safe. Don't recommend retry unless the exact error is transient.

## E. How our Foundation agents map to this standard
- **Supervisors** (Foundation Setup, Accounting Foundation) = §9 routers/phase-managers + gate enforcers +
  shared business-profile intake. They do NOT recommend field config or collect field-level values.
- **Specialists** (the per-domain agents — Organization, Currency, Accounting Schema, Chart of Accounts, …)
  = §10 consultants running `collect→explain→validate→recommend→propose→confirm→execute→verify`. **This is
  where the business-type-based recommendations live.**
- **Discovery / Validation** = MO/QC read-only agents.
- **Tool bindings** follow §B (VAAPI Services methods, CustomParams templates, label limits, AuthRequired=Y).
