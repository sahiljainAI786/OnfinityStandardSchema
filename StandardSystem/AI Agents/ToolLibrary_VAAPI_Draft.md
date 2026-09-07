# Tool Library draft — VAAPI Core Service (VA101_API)

> First concrete build step (blueprint Phase 2). Registers the six VAAPI methods allowed by
> `tools_Guidelines.md` into the Tool Library so they can be bound to agents in `VAI01_OAgentAPI`.
> Companion import file: `ToolLibrary_VAAPI_Import.csv` (flattened hierarchical shape per blueprint §7.0.2).
> Parameters verified against `VAAPI_Services Document File Version No 2.0.pdf`.

> ✅ **Registry — DECIDED 2026-06-11 (user):** orchestration & AI agents use **`VA101_API`** (module
> "API Library", window `VA101_ToolLibrary`). This draft targets `VA101_` — final. (`VIS_API` is the
> core/migration target; `VAI01_API` is the AI Assistant's own set — not used for orchestration.)

## 1. Service row — `VA101_API` (one row)

| Column | Value | Note |
|---|---|---|
| `Name` (id) | `VAAPI Core Service` | |
| `Value` | `VAAPI_CORE` | search key |
| `VA101_BaseUrl` | `https://<HOST>/api/VAAPI/Service` | **confirm host** (doc uses `OnfinitySample.com`) |
| `VA101_Protocol` | `HTTPS` | |
| `VA101_AuthType` | `AK` (API Key) | `accessKey` + `secretKey` sent as **headers** on every call |
| `VA101_AuthProvider_ID` | → VA101_AuthProvider | the credential holder for accessKey/secretKey |
| `VA101_IsInternal` | `Y` | internal ERP service |
| `IsActive` | `Y` | |

> `accessKey`/`secretKey` are **auth-level headers**, not per-method parameters — they live in the
> AuthProvider/credential setup (`VA101_APIAuthCredential` / `VA101_APIAgentAuthCredential`), referenced
> from each `VAI01_OAgentAPI` binding. They are intentionally NOT listed as `VA101_MethodParameter` rows.

## 2. Method rows — `VA101_APIMethod` (six rows, all children of VAAPI Core Service)

All are HTTP **POST** (`VA101_Method=P`), `VA101_AuthRequired=Y`, `RequestFormat=JSON`, `ResponseForm=JSON`.

| `Name` (id) | `VA101_Path` | Governance class | Purpose |
|---|---|---|---|
| `GetRecordV2` | `/GetRecordV2` | **Read** (no approval) | SELECT query → rows |
| `GetParameters` | `/GetParameters` | **Read** (no approval) | fetch a process's param list before running |
| `InsertRecordM3` | `/InsertRecordM3` | **Write** (validate + confirm) | insert (object values) → PK or `NotInserted` |
| `UpdateRecordM2` | `/UpdateRecordM2` | **Write** (validate + confirm) | update by `sqlWhere` → count |
| `UpdateRecordM3` | `/UpdateRecordM3` | **Write** (validate + confirm) | update by `record_ID` (object values) |
| `RunProcess` | `/RunProcess` | **Process/Post** (approval; posting/period-close = mandatory) | execute a job |

## 3. Parameter rows — `VA101_MethodParameter`

DataType LOV: STR=string · STA=string[] · OBA=object[] · INT=integer · BOL=boolean. Location: B=Body.

**GetRecordV2**
| Name | DataType | Loc | Required |
|---|---|---|---|
| tableName | STR | B | Y |
| selectQuery | STR | B | Y |

**GetParameters**
| Name | DataType | Loc | Required |
|---|---|---|---|
| processValue | STR | B | Y |
| AD_Client_ID | INT | B | Y |
| AD_Org_ID | INT | B | Y |

**InsertRecordM3**
| Name | DataType | Loc | Required |
|---|---|---|---|
| colName | STA | B | Y |
| tableName | STR | B | Y |
| values | OBA | B | Y |
| includeStandardCols | BOL | B | Y |
| fileName | STR | B | N (pass empty string) |
| saveFileInDisk | BOL | B | N |

**UpdateRecordM2**
| Name | DataType | Loc | Required |
|---|---|---|---|
| tableName | STR | B | Y |
| colName | STA | B | Y |
| values | STA | B | Y |
| sqlWhere | STR | B | Y |

**UpdateRecordM3**
| Name | DataType | Loc | Required |
|---|---|---|---|
| tableName | STR | B | Y |
| colName | STA | B | Y |
| values | OBA | B | Y |
| record_ID | INT | B | Y |

**RunProcess**
| Name | DataType | Loc | Required |
|---|---|---|---|
| AD_Process_ID | INT | B | Y |
| Record_ID | INT | B | Y |
| Param | OBA | B | N (list of {Name, Value}) |

## 4. Binding these to an agent (`VAI01_OAgentAPI`)

Per agent, add a row per allowed method: `VAI01_OAgent_ID`=agent, `VA101_API_ID`=VAAPI Core Service,
`VA101_APIMethod_ID`=the method, `VAI01_ExecutionOrder`=n, `VAI01_ResponseHandling`=interpretation note,
credential FK = the accessKey/secretKey credential. Example (Posting-Validation MO agent): bind only
`GetRecordV2` + `GetParameters` (read-only). A GL Action (AC) agent additionally gets `InsertRecordM3`,
`UpdateRecordM3`, `RunProcess` — the last approval-gated.

## 5. Open items
1. **Host** for `VA101_BaseUrl` (replace `<HOST>`).
2. **accessKey / secretKey** credential — register via the now-available auth windows:
   `VA101_AuthorizationProvider` (`VA101_AuthProvider` + `VA101_AuthScope`; supports OAuth2 Google/Microsoft
   + scopes Calendar/EMail/Profile/Storage/Task), then `VA101_APIAuthCredential`/`VA101_AuthCrediential`,
   and `VA101_APIAgentAuthCredential` for the per-agent credential that `VAI01_OAgentAPI` references. For the
   VAAPI Core Service use AuthType=AK (API Key headers), not OAuth2.
3. **Insert path** — register these via VAAPI `InsertRecordM3` calls, direct window entry, or SQL import (the CSV supports the latter two).
4. ~~Confirm VA101 vs VIS registry~~ — **decided: VA101** (bind via `VA101_API_ID` + `VA101_APIMethod_ID`).
