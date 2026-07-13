# Registers a local Debug build of CmdPal.Ext.Power for Command Palette (Developer Mode).
#
# Prerequisites:
#   - Windows Developer Mode enabled
#   - Built MSIX package:
#       dotnet build CmdPal.Ext.Power\CmdPal.Ext.Power.csproj -c Debug -p:Platform=x64 -p:GenerateAppxPackageOnBuild=true
#
# Usage (from repo root):
#   .\scripts\Register-Local.ps1

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

Get-AppxPackage "JRScott812.CmdPal.Ext.Power" -ErrorAction SilentlyContinue | Remove-AppxPackage -ErrorAction SilentlyContinue
Add-AppxPackage -Register (Join-Path $layout "AppxManifest.xml")

Get-AppxPackage "JRScott812.CmdPal.Ext.Power" | Format-List Name, PackageFullName, Status, InstallLocation
Write-Host ""
Write-Host "Registered from: $($msix.FullName)"
Write-Host "Restart Command Palette, then search for 'Power'."
