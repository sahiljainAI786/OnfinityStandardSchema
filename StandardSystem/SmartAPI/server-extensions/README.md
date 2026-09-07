# Server-side extensions (Onfinity ERP)

Small server-side additions the Smart API needs for operations that VAAPI can't reach as-is.
These go into the Onfinity (ViennaAdvantage) server, not the Smart API.

## InitialClientSetup.cs — make "Initial Client Setup" callable headlessly

**Why:** Initial Client Setup is a UI-wizard model method (`MSetup.CreateClient`), not a registered
`AD_Process`, so VAAPI `RunProcess`/`RunProcessBySearchKey` cannot call it. This wrapper exposes it as
a process so the Smart API action `POST /v1/Tenant/actions/initialSetup` works.

### 1. Add the class
Place `InitialClientSetup.cs` in the framework's process source (e.g. `ModelLibrary/.../Process/`),
in namespace `VAdvantage.Process`, and compile it with the framework (it references
`MSetup`/`TenantInfoM` in `VAdvantage.Model`, `SvrProcess`/`ProcessInfoParameter` in
`VAdvantage.ProcessEngine`). Verify the method names against your framework version
(`GetParameterName()`, `GetParameter()`, `GetCtx()`).

### 2. Register the AD_Process (Application Dictionary)
Create one `AD_Process` (Window: Report & Process):

| Field | Value |
|---|---|
| Search Key / Value | `InitialClientSetup`  (must match the Smart API manifest) |
| Name | Initial Client Setup |
| Class Name | `VAdvantage.Process.InitialClientSetup` |
| Server Process | Yes |

Then 4 `AD_Process_Para` rows, all **String** (AD_Reference = String/10):

| Parameter name | Required |
|---|---|
| `ClientName` | Yes |
| `OrgName` | Yes |
| `AdminUserName` | Yes |
| `NormalUserName` | Yes |

The parameter **names must match exactly** (the Smart API sends them by name).

### 3. Use it
```
POST /v1/Tenant/actions/initialSetup
{ "ClientName": "Acme Inc",
  "OrgName": "Acme HQ",
  "AdminUserName": "AcmeAdmin",
  "NormalUserName": "AcmeUser" }
```
Returns the process message (`"Tenant 'Acme Inc' created: ..."`). No Smart API change is needed once
the process is registered — the `initialSetup` action is already wired to search key `InitialClientSetup`.

### Notes
- `MSetup.CreateClient` runs in its own transaction and provisions client + org + admin/org users +
  roles + accounting schema + calendar + sequences (the full setup the wizard performs).
- Test on a non-production tenant first; client creation is not easily reversible.
- This is the deliberate "native/server-side" boundary: master-data and most documents go through
  VAAPI via manifests, but framework wizards like client setup need a thin process wrapper.
