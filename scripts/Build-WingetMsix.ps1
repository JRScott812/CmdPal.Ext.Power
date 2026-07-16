<#
.SYNOPSIS
    Builds x64 and ARM64 MSIX packages for GitHub Releases / community WinGet.

.DESCRIPTION
    CmdPal discovers extensions via AppExtensionCatalog (packaged apps only).
    Working community WinGet listings for CmdPal extensions ship signed MSIX
    (not unpackaged Inno EXE).

.PARAMETER Version
    Optional four-part package version (e.g. 1.2.5.0). When omitted, uses the
    Version already set on Package.appxmanifest Identity. When provided, updates
    the manifest before building (commit that change with your release).

.PARAMETER Configuration
    Build configuration. Default: Release.

.PARAMETER OutDir
    Directory that receives copied .msix files. Default: artifacts\winget

.EXAMPLE
    .\scripts\Build-WingetMsix.ps1

.EXAMPLE
    .\scripts\Build-WingetMsix.ps1 -Version 1.2.5.0
#>

param(
    [ValidatePattern('^\d+\.\d+\.\d+\.\d+$')]
    [string]$Version = "",

    [string]$Configuration = "Release",

    [string]$OutDir = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path $PSScriptRoot -Parent
$project = Join-Path $repoRoot "CmdPal.Ext.Power\CmdPal.Ext.Power.csproj"
$manifestPath = Join-Path $repoRoot "CmdPal.Ext.Power\Package.appxmanifest"

if (-not (Test-Path $project)) {
    Write-Error "Project not found: $project"
}

[xml]$manifestXml = Get-Content -Path $manifestPath
if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = $manifestXml.Package.Identity.Version
    Write-Host "Using Package.appxmanifest version: $Version" -ForegroundColor Yellow
}
else {
    $manifestXml.Package.Identity.Version = $Version
    $manifestXml.Save($manifestPath)
    Write-Host "Updated Package.appxmanifest Identity Version -> $Version" -ForegroundColor Yellow
}

if ([string]::IsNullOrWhiteSpace($OutDir)) {
    $OutDir = Join-Path $repoRoot "artifacts\winget"
}

New-Item -ItemType Directory -Path $OutDir -Force | Out-Null

$architectures = @("x64", "ARM64")
$built = @()

foreach ($arch in $architectures) {
    Write-Host "`n=== Building MSIX ($arch) ===" -ForegroundColor Cyan

    $packageDir = Join-Path $repoRoot "CmdPal.Ext.Power\AppPackages\winget-$arch"
    if (Test-Path $packageDir) {
        Remove-Item $packageDir -Recurse -Force
    }

    dotnet build $project `
        --configuration $Configuration `
        -p:Platform=$arch `
        -p:GenerateAppxPackageOnBuild=true `
        -p:AppxPackageDir="$packageDir\" `
        -p:AppxPackageVersion=$Version `
        -p:AppxBundle=Never

    if ($LASTEXITCODE -ne 0) {
        Write-Error "dotnet build failed for $arch"
    }

    $msix = Get-ChildItem -Path $packageDir -Filter "*.msix" -Recurse |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if (-not $msix) {
        Write-Error "No .msix produced under $packageDir"
    }

    $destName = "JakeScott.Power_$Version`_$($arch.ToLowerInvariant()).msix"
    $dest = Join-Path $OutDir $destName
    Copy-Item $msix.FullName $dest -Force
    $built += $dest
    Write-Host "Copied: $dest" -ForegroundColor Green
}

Write-Host "`nMSIX packages ready:" -ForegroundColor Cyan
$built | ForEach-Object { Write-Host "  $_" }

Write-Host @"

Next steps:
  1. Sign the MSIX files with a trusted code-signing certificate (required for winget-pkgs).
  2. Create a GitHub Release and upload both .msix files.
  3. Run:  .\scripts\Submit-Winget.ps1 -X64Url <url> -Arm64Url <url> -Version $Version
     Or manually: wingetcreate new "<x64-url>" "<arm64-url>"
  4. See winget/README.md for PackageIdentifier, tag, and dependency requirements.
"@

