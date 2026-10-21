# GitHub Actions Setup - Final Checklist

## ✅ What's Already Done For You

- [x] `.github/workflows/publish.yml` created with:
  - Automatic Inno Setup installation
  - x64 and ARM64 build jobs
  - GitHub Release creation
  - WinGet manifest submission
  - Proper error handling

- [x] Documentation created:
  - `GITHUB_ACTIONS_SETUP.md` — Full setup guide
  - `GITHUB_ACTIONS_READY.md` — Quick start guide
  - `PUBLISHING_GUIDE.md` — General publishing reference
  - `STORE_UPDATE_v1.2.9.0.md` — Version-specific update steps

- [x] Version updated to 1.2.9.0:
  - `CmdPal.Ext.Power.csproj` ✓
  - `Package.appxmanifest` ✓

---

## 🎯 What You Need to Do (5 Minutes)

### Step 1: Enable Workflow Permissions (2 min)

**GitHub Web UI:**
1. Go to: https://github.com/JRScott812/CmdPal.Ext.Power/settings/actions
2. Under "Workflow permissions":
   - ✅ Select **Read and write permissions**
   - ✅ Check **Allow GitHub Actions to create and approve pull requests**
3. Click **Save**

---

### Step 2: Verify/Create Required Files (2 min)

Check if these files exist:

```powershell
cd F:\GitHub\jrscott812\CmdPal.Ext.Power
Test-Path "setup-template.iss"
Test-Path "build-exe.ps1"
Test-Path ".github/workflows/publish.yml"
```

**If any return FALSE, see "Missing Files" section below**

---

### Step 3: Commit and Push (1 min)

```powershell
cd F:\GitHub\jrscott812\CmdPal.Ext.Power

# Add all changes
git add .

# Commit
git commit -m "v1.2.9.0: Fixed widget display after restart"

# Push to GitHub
git push origin main
```

---

## 📝 Missing Files?

### Missing `setup-template.iss`?

Create at `F:\GitHub\jrscott812\CmdPal.Ext.Power\setup-template.iss`:

```powershell
$content = @'
#define MyAppName "Power Management for Command Palette"
#define MyAppVersion "1.2.9.0"
#define MyAppPublisher "JRScott812"
#define MyAppURL "https://github.com/JRScott812/CmdPal.Ext.Power"
#define MyAppCLSID "{YOUR-CLSID-HERE}"

[Setup]
AppId={#MyAppCLSID}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
OutputBaseFilename=Power_Management_for_CommandPalette_{#MyAppVersion}_{#SetupSetting("ArchitecturesAllowed")}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
OutputDir=Installer

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Registry]
Root: HKCU; Subkey: "Software\Classes\CLSID\{#MyAppCLSID}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\CLSID\{#MyAppCLSID}\InprocServer32"; ValueType: string; ValueName: ""; ValueData: "{app}\CmdPal.Ext.Power.dll"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\CLSID\{#MyAppCLSID}\InprocServer32"; ValueType: string; ValueName: "ThreadingModel"; ValueData: "Both"; Flags: uninsdeletekey

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
'@

Set-Content -Path "setup-template.iss" -Value $content
```

⚠️ **Important:** Replace `{YOUR-CLSID-HERE}` with your actual CLSID from `PowerCommandsProvider.cs`:

```powershell
# Find your CLSID
(Get-Content "CmdPal.Ext.Power\PowerCommandsProvider.cs" | Select-String '\[Guid\("([^"]+)"\)').Matches[0].Groups[1].Value
```

Then manually edit `setup-template.iss` and replace `{YOUR-CLSID-HERE}` with the actual GUID (keeping the braces).

---

### Missing `build-exe.ps1`?

Create at `F:\GitHub\jrscott812\CmdPal.Ext.Power\build-exe.ps1`:

```powershell
<#
.SYNOPSIS
	Builds EXE installers for x64 and ARM64 using dotnet publish and Inno Setup.
#>

param(
	[string]$Configuration = "Release",
	[string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

Write-Host "Building EXE installers v$Version" -ForegroundColor Cyan

$architectures = @("x64", "arm64")

foreach ($arch in $architectures) {
	Write-Host "`n=== Building $arch ===" -ForegroundColor Cyan

	# Publish
	Write-Host "Publishing for $arch..."
	dotnet publish -c $Configuration -r "win-$arch" -o "publish" --self-contained=false
	if ($LASTEXITCODE -ne 0) {
		Write-Error "dotnet publish failed for $arch"
		exit 1
	}

	# Create installer
	Write-Host "Creating installer for $arch..."
	$issFile = "setup-template.iss"
	if (-not (Test-Path $issFile)) {
		Write-Error "Inno Setup script not found: $issFile"
		exit 1
	}

	$archFlag = if ($arch -eq "arm64") { "arm64" } else { "x64" }
	& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" `
		/DMyAppVersion="$Version" `
		/DArchitecturesAllowed="$archFlag" `
		$issFile

	if ($LASTEXITCODE -ne 0) {
		Write-Error "Inno Setup failed for $arch"
		exit 1
	}

	# Clean publish directory for next architecture
	Remove-Item -Recurse -Force "publish" -ErrorAction SilentlyContinue

	Write-Host "=== $arch complete ===" -ForegroundColor Green
}

Write-Host "`nInstallers created in the 'Installer' directory:" -ForegroundColor Cyan
Get-ChildItem -Path "Installer" -Filter "*.exe" | ForEach-Object { Write-Host "  $_" }
```

Then run:
```powershell
git add build-exe.ps1
git commit -m "Add build-exe.ps1 script"
git push origin main
```

---

## 🚀 First Automated Release

Once everything above is done:

```powershell
cd F:\GitHub\jrscott812\CmdPal.Ext.Power

# Create and push the v1.2.9.0 tag
git tag v1.2.9.0
git push origin v1.2.9.0
```

**Monitor at:** https://github.com/JRScott812/CmdPal.Ext.Power/actions

---

## ⚡ What Happens Next

1. **~5 min** - Build job creates x64 and ARM64 .exe installers
2. **~2 min** - Release job creates GitHub Release with EXE files attached
3. **~3 min** - WinGet job submits updated manifest to microsoft/winget-pkgs
4. **~24 hours** - WinGet PR is merged and approved
5. **~48 hours** - Users can `winget upgrade` or find in WinGet search

---

## 📊 Workflow Jobs Explained

| Job | Duration | What It Does |
|-----|----------|-------------|
| **build** | 5-8 min | Installs Inno Setup, publishes for both architectures, creates .exe installers |
| **release** | 1-2 min | Creates GitHub Release and attaches the EXE files |
| **winget-update** | 2-3 min | Submits updated WinGet manifest to microsoft/winget-pkgs repo |

---

## 🔐 Secrets (Already Working)

- ✅ `GITHUB_TOKEN` — Auto-provided by GitHub, no setup needed
  - Used by workflow to create releases and submit to WinGet
  - No manual configuration required

---

## ✅ Final Verification

Run this to verify everything is set up:

```powershell
cd F:\GitHub\jrscott812\CmdPal.Ext.Power

# Check files exist
Write-Host "Checking files..."
Write-Host "setup-template.iss: $(Test-Path 'setup-template.iss')"
Write-Host "build-exe.ps1: $(Test-Path 'build-exe.ps1')"
Write-Host ".github/workflows/publish.yml: $(Test-Path '.github/workflows/publish.yml')"

# Check version is 1.2.9.0
Write-Host "`nChecking versions..."
(Get-Content CmdPal.Ext.Power/CmdPal.Ext.Power.csproj | Select-String '<AppxPackageVersion>' | Select-Object -First 1).Line
(Get-Content CmdPal.Ext.Power/Package.appxmanifest | Select-String 'Version=' | Select-Object -First 1).Line

# Check git status
Write-Host "`nGit status:"
git status
```

All checks should pass ✅

---

## 🎯 You're Ready to Release!

```powershell
git tag v1.2.9.0
git push origin v1.2.9.0
```

Then go watch the action at: **https://github.com/JRScott812/CmdPal.Ext.Power/actions**

🎉 **Automation engaged!** 🎉
