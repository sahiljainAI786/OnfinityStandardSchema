# Onfinity Smart API — IIS deployment

Hosted as a **separate IIS application with its own app pool**, on the same server as Onfinity
(see README "Deployment"). The published app is in `SmartAPI/publish/`.

## Prerequisite — register the ASP.NET Core Module into IIS (NEEDS ELEVATION)

Status on this box (2026-06-27): the **Hosting Bundle 8.0.28 is installed** (winget
`Microsoft.DotNet.HostingBundle.8`) and the module files exist at
`C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll` — **but it is NOT registered into IIS**
(`C:\Windows\System32\inetsrv\aspnetcorev2.dll` absent; the bundle's IIS-registration step didn't run).
Registration + the site creation below both require an **elevated** PowerShell — they could not be done
from the (non-admin, non-interactive) automation shell.

In an **Administrator** PowerShell, repair the bundle so it wires ANCM into IIS, then reset IIS:

```powershell
# re-run the installed Hosting Bundle in repair mode (registers AspNetCoreModuleV2 into IIS)
winget install Microsoft.DotNet.HostingBundle.8 --force --accept-package-agreements --accept-source-agreements
iisreset
# verify:
Test-Path C:\Windows\System32\inetsrv\aspnetcorev2.dll   # expect True
```
(If `winget --force` still doesn't register it, download the Hosting Bundle EXE from
https://dotnet.microsoft.com/download/dotnet/8.0 and run it — the EXE detects IIS and registers ANCM.)

## Steps (elevated PowerShell, after ANCM is registered)

```powershell
Import-Module WebAdministration
$site = "OnfinitySmartApi"
$path = "C:\VA Standard\StandardSystem\SmartAPI\publish"
$port = 8088   # pick a free port, or bind a host header on 443 with a cert

# dedicated app pool, No Managed Code (ASP.NET Core runs out of the worker process)
New-WebAppPool -Name $site
Set-ItemProperty IIS:\AppPools\$site -Name managedRuntimeVersion -Value ""
# optional: run as a least-privilege identity that may reach VAAPI over loopback

New-Website -Name $site -PhysicalPath $path -ApplicationPool $site -Port $port
Start-Website -Name $site

# grant the app-pool identity read/exec on the folder + write on .\logs
icacls $path /grant "IIS AppPool\$site:(OI)(CI)RX"
icacls "$path\logs" /grant "IIS AppPool\$site:(OI)(CI)M"
```

## Configuration

- **VAAPI endpoint** is set in `publish/web.config` → `Vaapi__BaseUrl`
  (currently `https://erplive.viennaadvantage.com/api/VAAPI/Service`). For a same-box Onfinity, point
  it at the loopback VAAPI instead. **No credentials live here** — callers send `accessKey`/`secretKey`
  as request headers (pass-through; see README "Authorization").
- For HTTPS, bind a cert to the site (recommended since callers send keys): `New-WebBinding` + a cert in
  the IIS store, or front it with the existing Onfinity site under a virtual path.

## Smoke test (after start)

```bash
curl http://localhost:8088/health
curl -H "accessKey: <k>" -H "secretKey: <s>" http://localhost:8088/v1/_entities
curl -X PUT "http://localhost:8088/v1/BusinessPartner?dryRun=true" \
  -H "accessKey: <k>" -H "secretKey: <s>" -H "Content-Type: application/json" \
  -d '{"key":"VAI159","group":"Employees"}'
```

## Republish (after code/manifest changes)

```bash
dotnet publish src/OnfinitySmartApi/OnfinitySmartApi.csproj -c Release -o publish
```
NOTE: publish regenerates `web.config`, dropping the `<environmentVariables>` block above — re-add it,
or move `Vaapi:BaseUrl` into `appsettings.Production.json` so it survives republish. Stop the site/app
pool first if the DLL is locked.
