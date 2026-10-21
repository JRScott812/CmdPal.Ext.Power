# ✅ GitHub Actions Setup Complete

## What Was Done

1. ✅ Created `.github/workflows/publish.yml` — Automated build and publish workflow
2. ✅ Created `GITHUB_ACTIONS_SETUP.md` — Complete setup documentation
3. ✅ Created `build-exe.ps1` template — PowerShell script for building EXE installers
4. ✅ Version bumped to 1.2.9.0 in project files

---

## 🚀 Quick Start: Make Your First Automated Release

### Step 1: Enable GitHub Actions Permissions (ONE-TIME SETUP)

1. Go to: https://github.com/JRScott812/CmdPal.Ext.Power
2. Click **Settings** (top right)
3. Left sidebar → **Actions → General**
4. Under "Workflow permissions":
   - ✅ Select **Read and write permissions**
   - ✅ Check **Allow GitHub Actions to create and approve pull requests**
5. Click **Save**

⏱️ **Time:** 1 minute

---

### Step 2: Verify Required Files

Check these 3 files exist in your repo root:

```powershell
Test-Path "setup-template.iss"           # Should be TRUE
Test-Path "build-exe.ps1"                # Should be TRUE  
Test-Path ".github/workflows/publish.yml" # Should be TRUE
```

If any are missing, see "Missing Files?" section below.

⏱️ **Time:** 1 minute

---

### Step 3: Test with a Version Tag

```powershell
# Commit everything
git add .
git commit -m "v1.2.9.0: Fixed widget display after restart"

# Create and push version tag
git tag v1.2.9.0
git push origin main
git push origin v1.2.9.0
```

⏱️ **Time:** 2 minutes

---

### Step 4: Watch the Magic! ✨

**Go to:** https://github.com/JRScott812/CmdPal.Ext.Power/actions

You'll see a workflow running with 3 jobs:

1. **build** (5-10 min) — Builds x64 and ARM64 EXE installers
2. **release** (1-2 min) — Creates GitHub Release with EXE files
3. **winget-update** (2-3 min) — Submits updated manifest to WinGet

**Total time:** ~15 minutes

---

## 📋 What the Workflow Does

### Job 1: Build (Windows Runner)
```
✓ Checkout code
✓ Setup .NET 10
✓ Install Inno Setup
✓ Publish for x64
✓ Create x64.exe installer
✓ Publish for ARM64
✓ Create arm64.exe installer
```

### Job 2: Release (Ubuntu Runner)
```
✓ Download both EXE files
✓ Create GitHub Release
✓ Attach EXEs to release
```

### Job 3: WinGet Update (Windows Runner)
```
✓ Download EXE files (from release)
✓ Install WingetCreate
✓ Submit updated manifest to WinGet repo
✓ Creates PR to microsoft/winget-pkgs
```

---

## ⚠️ Missing Files?

### Missing `setup-template.iss`?

Create it at repo root:

```iss
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
```

**Replace `{YOUR-CLSID-HERE}` with your actual CLSID from `PowerCommandsProvider.cs`**

### Missing `build-exe.ps1`?

See `GITHUB_ACTIONS_SETUP.md` for the complete template.

---

## 🧪 Manual Test (Before Automatic Releases)

Want to test locally first?

```powershell
# 1. Build locally
.\build-exe.ps1 -Version "1.2.9.0"

# 2. Check Installer folder
dir Installer

# 3. Test one EXE
.\Installer\Power_Management_for_CommandPalette_1.2.9.0_x64.exe

# 4. After installing, verify extension appears in Command Palette
```

---

## 📊 Workflow Status Indicators

| Status | Meaning | Action |
|--------|---------|--------|
| 🟢 **Success** | All jobs passed | Release went live to both stores! |
| 🟡 **In Progress** | Workflow running | Wait for completion |
| 🔴 **Failed** | One or more jobs failed | Check the logs (click job name for details) |

---

## 🔧 Common Issues & Solutions

### Issue: "Inno Setup script not found"
- **Cause:** `setup-template.iss` missing from repo root
- **Fix:** Add the file (see above)

### Issue: "build-exe.ps1 not found"
- **Cause:** Script missing from repo root
- **Fix:** Add the script (see GITHUB_ACTIONS_SETUP.md)

### Issue: WinGet job fails with "Package ID not found"
- **Cause:** Package name doesn't match existing manifest
- **Fix:** Update workflow to use correct package ID: `JRScott812.PowerManagementforCommandPalette`

### Issue: Release job fails with "Permission denied"
- **Cause:** Workflow permissions not set to "Read and write"
- **Fix:** Go to Settings → Actions → General → Set "Workflow permissions" to "Read and write"

### Issue: Workflow doesn't start when I push a tag
- **Cause:** Tag not matching the pattern `v*`
- **Fix:** Use tags like `v1.2.9.0`, not `1.2.9.0`

---

## 📈 Workflow Efficiency

| Step | Duration | Runner |
|------|----------|--------|
| Checkout & Setup | 30s | Windows |
| Install Inno Setup | 2-3 min | Windows |
| Publish x64 | 1-2 min | Windows |
| Build x64.exe | 1 min | Windows |
| Publish ARM64 | 1-2 min | Windows |
| Build ARM64.exe | 1 min | Windows |
| Upload artifacts | 30s | Windows |
| Download artifacts | 1 min | Ubuntu |
| Create release | 1-2 min | Ubuntu |
| WinGet submission | 2-3 min | Windows |
| **TOTAL** | **~15 min** | — |

---

## 🔄 Next Release Process

After v1.2.9.0, future releases are super simple:

```powershell
# 1. Update version in code (if needed)
# 2. Commit changes
git add .
git commit -m "v1.3.0: New features"

# 3. Create release tag
git tag v1.3.0
git push origin main --tags

# 4. That's it! Workflow runs automatically 🚀
```

---

## 📝 Release Notes

When the workflow succeeds, GitHub will auto-generate release notes. You can customize them:

1. Go to Releases → v1.2.9.0
2. Click "Edit"
3. Update the description with release notes
4. Save

**Template:**
```
## v1.2.9.0 - Bug Fixes & Improvements

### 🐛 Fixed
- Widget title and icons now update after computer restart
- Power plan changes properly detected and refreshed
- Improved state synchronization

### ✨ Enhancements
- Added continuous polling for power plan changes
- Better event handling for power state updates

**Installing:**
- Microsoft Store: Auto-update within 24 hours
- WinGet: `winget upgrade JRScott812.PowerManagementforCommandPalette`
```

---

## ✅ Final Verification Checklist

Before your first automated release:

- [ ] Settings → Actions → General: "Workflow permissions" = "Read and write"
- [ ] `setup-template.iss` exists in repo root
- [ ] `build-exe.ps1` exists in repo root  
- [ ] `.github/workflows/publish.yml` exists
- [ ] All files committed and pushed to `main`
- [ ] Version numbers updated (1.2.9.0)
- [ ] You're ready to tag and push!

---

## 🎯 You're Ready!

Go to your repo and run:

```powershell
git tag v1.2.9.0
git push origin main v1.2.9.0
```

Then watch the workflow run at: https://github.com/JRScott812/CmdPal.Ext.Power/actions

✨ **First automated release incoming!** ✨
