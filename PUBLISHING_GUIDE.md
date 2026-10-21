# Quick Update Guide - Microsoft Store & WinGet

## Current Situation
- ✅ Microsoft Store listing exists
- ✅ WinGet manifest exists
- 🔄 Need to publish v1.0.1 with bug fixes

## Version Bump Checklist

### 1. Update Version Everywhere
- [ ] `CmdPal.Ext.Power.csproj` → `<AppxPackageVersion>1.0.1.0</AppxPackageVersion>`
- [ ] `Package.appxmanifest` → `<Identity Version="1.0.1.0" />`
- [ ] Inno Setup script `setup-template.iss` → `#define MyAppVersion "1.0.1"`

### 2. Build Updated Packages

```powershell
# Build MSIX for both architectures
dotnet build --configuration Release -p:GenerateAppxPackageOnBuild=true -p:Platform=x64 -p:AppxPackageDir="AppPackages\x64\"
dotnet build --configuration Release -p:GenerateAppxPackageOnBuild=true -p:Platform=ARM64 -p:AppxPackageDir="AppPackages\ARM64\"

# Build EXE installers for WinGet
.\build-exe.ps1 -Version "1.0.1"
```

### 3. Create MSIX Bundle
```powershell
cd AppPackages
makeappx bundle /f bundle_mapping.txt /p CmdPal.Ext.Power_1.0.1.0_Bundle.msixbundle
cd ..
```

### 4. Upload to Microsoft Store
1. Go to Partner Center → Your App
2. Click **Start a new submission**
3. Upload new MSIX bundle to **Packages**
4. Update **What's new in this version** section
5. Click **Submit for certification** (wait 1-3 days)

### 5. Update WinGet

#### Manual (One-time):
```powershell
# Create GitHub Release
git tag v1.0.1
git push origin v1.0.1
gh release create v1.0.1 "Installer\Power_Management_for_CommandPalette_1.0.1_x64.exe" "Installer\Power_Management_for_CommandPalette_1.0.1_arm64.exe"

# Update WinGet manifest
wingetcreate update JRScott812.PowerManagementforCommandPalette `
	--version "1.0.1" `
	--urls "https://github.com/JRScott812/CmdPal.Ext.Power/releases/download/v1.0.1/Power_Management_for_CommandPalette_1.0.1_x64.exe" `
		   "https://github.com/JRScott812/CmdPal.Ext.Power/releases/download/v1.0.1/Power_Management_for_CommandPalette_1.0.1_arm64.exe" `
	--submit `
	--token $env:GITHUB_TOKEN
```

#### Automated (Future releases):
- Just push a version tag: `git tag v1.0.2 && git push origin v1.0.2`
- GitHub Actions will automatically:
  1. Build x64 and ARM64 installers
  2. Create GitHub Release
  3. Submit updated WinGet manifest

## Release Notes Template

For Microsoft Store **What's new in this version**:
```
v1.0.1 - Bug Fixes & Improvements

✅ Fixed: Widget title and icons now update after computer restart
✅ Fixed: Power plan changes are now properly detected and refreshed
✅ Improved: Added continuous polling for power plan state changes
✅ Improved: Better state synchronization between settings and widget display
```

For WinGet (via GitHub release):
```
## v1.0.1 - Bug Fixes & Improvements

- Fixed widget display not updating after computer restart
- Added power plan change detection via polling
- Improved state synchronization
```

## Timeline
- **Microsoft Store**: Submit → Wait 1-3 days for certification → Auto-update to users
- **WinGet**: Submit manifest → Auto-approved (usually < 1 hour) → Users can install immediately

## Troubleshooting

**MSIX Bundle fails to create:**
```powershell
# Make sure you're in the AppPackages directory
cd AppPackages
# Use full path to makeappx if not in PATH
& "C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\makeappx.exe" bundle /f bundle_mapping.txt /p CmdPal.Ext.Power_1.0.1.0_Bundle.msixbundle
```

**WinGet update fails:**
- Verify GitHub token is set: `$env:GITHUB_TOKEN`
- Check manifest package ID matches exactly: `JRScott812.PowerManagementforCommandPalette`
- Verify EXE download URLs are correct and accessible

**Partner Center validation fails:**
- Ensure version numbers match across all files
- Check that identity values haven't changed
- Verify MSIX bundle contains both x64 and ARM64 packages
