# Registers a local Debug build of CmdPal.Ext.Power for Command Palette (Developer Mode).
#
# Prerequisites:
#   - Windows Developer Mode enabled
#   - Built MSIX package:
#       dotnet build CmdPal.Ext.Power\CmdPal.Ext.Power.csproj -c Debug -p:Platform=x64 -p:GenerateAppxPackageOnBuild=true
#
# Usage (from repo root):
#   .\scripts\Register-Local.ps1
#
# Note: Local registration is unsigned (SignatureKind=None) by design. Store installs are
# signed by Microsoft. You cannot locally sign with the Partner Center publisher CN.

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path $PSScriptRoot -Parent
$appPackages = Join-Path $repoRoot "CmdPal.Ext.Power\AppPackages"

$msix = Get-ChildItem -Path $appPackages -Filter "CmdPal.Ext.Power_*_x64_Debug.msix" -Recurse -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $msix) {
    Write-Error "No Debug x64 MSIX found under $appPackages. Build with GenerateAppxPackageOnBuild=true first."
}

$layout = Join-Path $appPackages "_layout"
if (Test-Path $layout) {
    Remove-Item $layout -Recurse -Force
}

New-Item -ItemType Directory -Path $layout -Force | Out-Null
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($msix.FullName, $layout)

# Loose registration under Developer Mode does not need a trusted signature.
Remove-Item (Join-Path $layout "AppxSignature.p7x") -Force -ErrorAction SilentlyContinue
Remove-Item (Join-Path $layout "AppxBlockMap.xml") -Force -ErrorAction SilentlyContinue

$manifestPath = Join-Path $layout "AppxManifest.xml"
[xml]$manifestXml = Get-Content -Path $manifestPath
$packageName = $manifestXml.Package.Identity.Name

# Remove stale local/dev registrations (old and current identity names).
@(
    $packageName,
    "JRScott812.CmdPal.Ext.Power",
    "JakeScott.PowerControlExtensionforCommandPalette"
) | Select-Object -Unique | ForEach-Object {
    Get-AppxPackage $_ -ErrorAction SilentlyContinue | Remove-AppxPackage -ErrorAction SilentlyContinue
}

Add-AppxPackage -Register $manifestPath

$registered = Get-AppxPackage $packageName
$registered | Format-List Name, PackageFullName, SignatureKind, Status, InstallLocation
if (-not (Test-Path (Join-Path $layout "Public"))) {
    Write-Warning "Public\ folder is missing from the package layout. AppExtension registration may fail."
}

Write-Host ""
Write-Host "Registered from: $($msix.FullName)"
Write-Host "SignatureKind=$($registered.SignatureKind) (None is expected for local Developer Mode registration)."
Write-Host "Restart Command Palette, then search for 'Power'."
Write-Host "Do not launch this app from the Start menu — use Command Palette."
