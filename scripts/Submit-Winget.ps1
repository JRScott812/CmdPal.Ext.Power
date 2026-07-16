<#
.SYNOPSIS
    Creates (or updates) a community WinGet manifest PR for JakeScott.Power.

.DESCRIPTION
    Wraps wingetcreate and prints the CmdPal-required edits (tag + WASDK dependency).
    Requires Microsoft.WingetCreate:  winget install Microsoft.WingetCreate

.PARAMETER X64Url
    Public HTTPS URL to the x64 MSIX on a GitHub Release (or other host).

.PARAMETER Arm64Url
    Public HTTPS URL to the ARM64 MSIX.

.PARAMETER Version
    Package version matching the MSIX (e.g. 1.2.5.0).

.PARAMETER Submit
    When set, asks wingetcreate to open the PR after generation.

.PARAMETER Update
    Use wingetcreate update instead of new (after the first successful publish).

.EXAMPLE
    .\scripts\Submit-Winget.ps1 `
      -Version 1.2.5.0 `
      -X64Url "https://github.com/JRScott812/CmdPal.Ext.Power/releases/download/v1.2.5.0/JakeScott.Power_1.2.5.0_x64.msix" `
      -Arm64Url "https://github.com/JRScott812/CmdPal.Ext.Power/releases/download/v1.2.5.0/JakeScott.Power_1.2.5.0_arm64.msix"
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$X64Url,

    [Parameter(Mandatory = $true)]
    [string]$Arm64Url,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+\.\d+$')]
    [string]$Version,

    [switch]$Submit,

    [switch]$Update
)

$ErrorActionPreference = "Stop"

$wingetcreate = Get-Command wingetcreate -ErrorAction SilentlyContinue
if (-not $wingetcreate) {
    Write-Error "wingetcreate not found. Install with: winget install Microsoft.WingetCreate"
}

$packageId = "JakeScott.Power"

Write-Host @"

=== WinGet submission for $packageId ===

When prompted / after generation, ensure:

  PackageIdentifier:  $packageId
  PackageVersion:     $Version
  Publisher:          Jake Scott
  PackageName:        Power for Command Palette
  License:            MIT
  ShortDescription:   Switch Windows power modes and power plans from Command Palette

In every *.locale.*.yaml add:

  Tags:
  - windows-commandpalette-extension

In the *.installer.yaml add:

  Dependencies:
    PackageDependencies:
    - PackageIdentifier: Microsoft.WindowsAppRuntime.2

"@ -ForegroundColor Cyan

$argsList = @()
if ($Update) {
    $argsList += @("update", $packageId, "--version", $Version, "--urls", $X64Url, $Arm64Url)
}
else {
    $argsList += @("new", $X64Url, $Arm64Url)
}

if ($Submit) {
    $argsList += "--submit"
}

Write-Host "Running: wingetcreate $($argsList -join ' ')" -ForegroundColor Yellow
& wingetcreate @argsList
if ($LASTEXITCODE -ne 0) {
    Write-Error "wingetcreate exited with code $LASTEXITCODE"
}

Write-Host @"

After the PR is open, double-check the locale tag and WindowsAppRuntime.2 dependency
before requesting review. See winget/README.md.
"@ -ForegroundColor Green

