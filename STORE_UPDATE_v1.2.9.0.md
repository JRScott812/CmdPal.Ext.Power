# Quick Publishing Steps for v1.2.9.0

## ✅ DONE: Version Updated
- [x] `.csproj` updated to 1.2.9.0
- [x] `Package.appxmanifest` updated to 1.2.9.0  
- [x] Code builds successfully

## 📋 NEXT STEPS

### For Microsoft Store (MSIX)

**Option A: Use Visual Studio GUI (Easiest)**
1. Open Visual Studio
2. **Build → Publish CmdPal.Ext.Power** 
3. Or right-click project → **Publish**
4. This will create signed MSIX packages
5. Go to `AppPackages/x64/` and `AppPackages/ARM64/` folders
6. Create bundle using `makeappx`:
   ```powershell
   cd AppPackages
   makeappx bundle /f bundle_mapping.txt /p CmdPal.Ext.Power_1.2.9.0_Bundle.msixbundle
   ```
7. Upload bundle to Partner Center

**Option B: Using Command Line**
```powershell
# Clean and rebuild
dotnet clean
dotnet build --configuration Release

# Visual Studio will prompt for certificate
# Complete the MSIX creation through VS dialogs
```

### For WinGet

**Step 1: Create GitHub Release**
```powershell
git add .
git commit -m "v1.2.9.0: Fixed widget display after restart, improved power plan detection"
git tag v1.2.9.0
git push origin main
git push origin v1.2.9.0
```

**Step 2: Build EXE Installers** (if you have `build-exe.ps1`)
```powershell
.\build-exe.ps1 -Version "1.2.9.0"
```

**Step 3: Create GitHub Release with EXE files**
```powershell
gh release create v1.2.9.0 `
	"Installer\Power_Management_for_CommandPalette_1.2.9.0_x64.exe" `
	"Installer\Power_Management_for_CommandPalette_1.2.9.0_arm64.exe" `
	--title "v1.2.9.0" `
	--notes "**Fixed:** Widget title and icons now update after computer restart`n**Improved:** Added power plan change detection and state synchronization`n**What's new:** Better widget refresh behavior for power settings changes"
```

**Step 4: Update WinGet Manifest**
```powershell
wingetcreate update JRScott812.PowerManagementforCommandPalette `
	--version "1.2.9.0" `
	--urls "https://github.com/JRScott812/CmdPal.Ext.Power/releases/download/v1.2.9.0/Power_Management_for_CommandPalette_1.2.9.0_x64.exe" `
		   "https://github.com/JRScott812/CmdPal.Ext.Power/releases/download/v1.2.9.0/Power_Management_for_CommandPalette_1.2.9.0_arm64.exe" `
	--submit `
	--token $env:GITHUB_TOKEN
```

## 📝 Release Notes to Use

**For Microsoft Store "What's new" section:**
```
v1.2.9.0 - Bug Fixes & Improvements

• Fixed: Widget title and icons now update correctly after computer restart
• Fixed: Power plan changes are now properly detected and refreshed in real-time
• Improved: Added continuous polling for power plan state changes
• Improved: Better synchronization between settings and widget display

This update resolves the issue where the dock widget would show stale power mode/plan information after system restart, even though the actual system settings were updated correctly.
```

**For GitHub Release notes:**
```
## v1.2.9.0 - Bug Fixes & Improvements

### 🐛 Fixes
- Widget display no longer stale after computer restart
- Power plan changes now detected and refreshed automatically
- Improved state synchronization between system and widget display

### ✨ Enhancements
- Added continuous polling for power plan changes
- Better event handling for power state updates

### 📦 What's Inside
- Fixed widget refresh after restart
- Improved power plan detection
- Better state management
```

## 🎯 Timeline

1. **Microsoft Store**: Submit → Certification (1-3 days) → Auto-update users
2. **WinGet**: Submit → Auto-approved (< 1 hour) → Users can `winget upgrade` immediately

## ⚠️ Important

- Keep both stores in sync with the same version
- Update release notes in both places
- Test the update locally before submitting
- Verify extension works after install

---

## Troubleshooting Certificate Issues

If you get certificate errors when building MSIX:

1. **Use Visual Studio's built-in Publish:** Right-click project → Publish
2. **Or regenerate cert:** Build → Publish CmdPal.Ext.Power will prompt for cert
3. **Alternative:** Build in Debug mode locally (for testing), submit from CI/CD that has cert setup

Let me know which step you need help with!
