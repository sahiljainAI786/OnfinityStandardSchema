<#
  Onfinity Smart API - one-shot IIS setup.
  Run it: right-click > "Run with PowerShell", or from any PowerShell:
      powershell -ExecutionPolicy Bypass -File "C:\VA Standard\StandardSystem\SmartAPI\iis-setup.ps1"
  It self-elevates (UAC prompt). Idempotent - safe to re-run.

  No credentials are configured here: callers send accessKey/secretKey as request headers
  (pass-through auth). The only deployment setting is Vaapi:BaseUrl, already baked into
  publish\appsettings.Production.json.
#>

param(
  [string]$SiteName = "OnfinitySmartApi",
  [string]$PhysicalPath = "C:\VA Standard\StandardSystem\SmartAPI\publish",
  [int]$Port = 8090
)

# --- self-elevate ----------------------------------------------------------
$id = [Security.Principal.WindowsPrincipal]([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $id.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
  Write-Host "Elevating (UAC)..." -ForegroundColor Yellow
  $a = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" -SiteName `"$SiteName`" -PhysicalPath `"$PhysicalPath`" -Port $Port"
  Start-Process powershell -Verb RunAs -ArgumentList $a
  return
}

$ErrorActionPreference = "Stop"
Write-Host "== Onfinity Smart API IIS setup ==" -ForegroundColor Cyan

Import-Module WebAdministration

# --- 1) ensure the ASP.NET Core Module (ANCM) is registered in IIS ---------
# Correct check = IIS global-module registration, NOT a dll in System32\inetsrv
# (modern Hosting Bundles keep the dll under Program Files\IIS and register that path).
function Test-Ancm { [bool](Get-WebGlobalModule -Name "AspNetCoreModuleV2" -ErrorAction SilentlyContinue) }

if (-not (Test-Ancm)) {
  $dll = "C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
  if (Test-Path $dll) {
    Write-Host "ANCM files present but not registered - registering global module..." -ForegroundColor Yellow
    & "$env:windir\system32\inetsrv\appcmd.exe" install module /name:"AspNetCoreModuleV2" /image:"$dll" | Out-Null
  } else {
    Write-Host "ANCM not present - repairing Hosting Bundle..." -ForegroundColor Yellow
    winget install Microsoft.DotNet.HostingBundle.8 --force --accept-package-agreements --accept-source-agreements
  }
  iisreset | Out-Null
  if (-not (Test-Ancm)) {
    throw "Could not register AspNetCoreModuleV2. Run the Hosting Bundle EXE from https://dotnet.microsoft.com/download/dotnet/8.0 manually (it detects IIS and registers the module), then re-run this script."
  }
}
Write-Host "ANCM: registered" -ForegroundColor Green

# --- port conflict guard (another IIS site/http.sys may own the port) ------
if (Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue) {
  throw "Port $Port is already in use (likely another IIS site). Re-run with a free port, e.g.:  -Port 8091"
}

# --- 2) app pool: No Managed Code (ASP.NET Core runs out-of-CLR) ------------
if (Test-Path "IIS:\AppPools\$SiteName") { Remove-WebAppPool -Name $SiteName }
New-WebAppPool -Name $SiteName | Out-Null
Set-ItemProperty "IIS:\AppPools\$SiteName" -Name managedRuntimeVersion -Value ""
Set-ItemProperty "IIS:\AppPools\$SiteName" -Name startMode -Value "AlwaysRunning"
Write-Host "App pool '$SiteName' (No Managed Code)" -ForegroundColor Green

# --- 3) website ------------------------------------------------------------
if (Get-Website -Name $SiteName -ErrorAction SilentlyContinue) { Remove-Website -Name $SiteName }
New-Website -Name $SiteName -PhysicalPath $PhysicalPath -ApplicationPool $SiteName -Port $Port | Out-Null
Write-Host "Website '$SiteName' on port $Port -> $PhysicalPath" -ForegroundColor Green

# --- 4) folder ACLs for the app-pool identity ------------------------------
& icacls "$PhysicalPath" /grant "IIS AppPool\${SiteName}:(OI)(CI)RX" /T /Q | Out-Null
& icacls "$PhysicalPath\logs" /grant "IIS AppPool\${SiteName}:(OI)(CI)M" /T /Q | Out-Null

Start-Website -Name $SiteName
Start-Sleep 3

# --- 5) smoke test ---------------------------------------------------------
try {
  $h = Invoke-RestMethod "http://localhost:$Port/health" -TimeoutSec 15
  Write-Host "health: $($h.status)" -ForegroundColor Green
  Write-Host ""
  Write-Host "Smart API is live at http://localhost:$Port  (docs: /swagger)" -ForegroundColor Cyan
} catch {
  Write-Host "Started, but /health did not respond: $($_.Exception.Message)" -ForegroundColor Red
  Write-Host "Check stdout log under $PhysicalPath\logs (set stdoutLogEnabled=true in web.config to capture)."
}

Write-Host ""
Write-Host "Done. (HTTPS: bind a cert to the site - recommended since callers send keys.)" -ForegroundColor Cyan
