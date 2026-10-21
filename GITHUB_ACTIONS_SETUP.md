# GitHub Actions Setup Checklist

## 🔑 Required Setup Steps

### 1. **Secrets** (GitHub Repository Settings)
Go to: **Settings → Secrets and variables → Actions**

**Add these secrets:**

- `GITHUB_TOKEN` — ⚠️ **Already built-in** (auto-provided by GitHub)
  - No setup needed, GitHub provides this automatically
  - Allows workflow to create releases and push to repos

- `WINGET_PAT` (Optional, for future manual submissions)
  - Create a Personal Access Token with `public_repo` scope
  - Use if you want to manually submit to WinGet outside of CI/CD

**Current setup:** Your workflow uses `${{ secrets.GITHUB_TOKEN }}` which is automatic ✅

---

### 2. **Repository Configuration**

#### A. **Allow GitHub Actions to create pull requests**
1. Go to **Settings → Actions → General**
2. Under "Workflow permissions", select:
   - ✅ **Read and write permissions**
   - ✅ **Allow GitHub Actions to create and approve pull requests**
3. Click **Save**

#### B. **Allow workflows to create releases**
1. Go to **Settings → Actions → General**
2. Verify "Workflow permissions" is set to "Read and write"

#### C. **Branch Protection Rules** (if you have them)
If you have branch protection on `main`:
1. Go to **Settings → Branches → Branch protection rules**
2. Edit rule for `main`
3. Under "Allow force pushes", check **Workflows**
4. This allows the publish workflow to tag releases

---

### 3. **Install Required Tools** (on Windows runners)

The workflow needs these to be pre-installed:

- ✅ **.NET 10 SDK** — workflow uses `actions/setup-dotnet@v4` ✅
- ✅ **GitHub CLI** — comes with `ubuntu-latest` and `windows-latest` runners ✅
- ⚠️ **Inno Setup 6** — Must be installed on Windows runner
- ⚠️ **WingetCreate** — Must be installed on Windows runner

**Update your workflow to install missing tools:**

See updated `publish.yml` below (add to your workflow file)

---

### 4. **Required Files in Your Repository**

Verify these files exist in your repo root:

- ✅ `setup-template.iss` — Inno Setup script for EXE creation
  - If missing, workflow will fail at "Create installer" step

- ✅ `build-exe.ps1` — PowerShell script to build installers
  - If missing, workflow will fail

- ✅ `.github/workflows/publish.yml` — The workflow file
  - Already created for you ✅

**Missing `build-exe.ps1`?** See the template below.

---

### 5. **GitHub Token Permissions**

✅ **Default `GITHUB_TOKEN` has these permissions:**
- ✅ Create releases
- ✅ Upload release assets
- ✅ Create tags
- ✅ Push to repository

**No additional setup needed** — GitHub Actions handles this automatically.

---

## 📋 Step-by-Step Setup

### Step 1: Enable Workflow Permissions

1. Go to your repo: https://github.com/JRScott812/CmdPal.Ext.Power
2. Click **Settings** (top right)
3. Left sidebar → **Actions → General**
4. Under "Workflow permissions":
   - ✅ Select **Read and write permissions**
   - ✅ Check **Allow GitHub Actions to create and approve pull requests**
5. Click **Save**

### Step 2: Verify Required Files Exist

In your repo root, you need:

```
CmdPal.Ext.Power/
├── .github/
│   └── workflows/
│       └── publish.yml ✅ (already created)
├── setup-template.iss ⚠️ (need to verify)
├── build-exe.ps1 ⚠️ (need to verify)
└── CmdPal.Ext.Power.csproj
```

**Check if these exist:**
```powershell
Test-Path "setup-template.iss"
Test-Path "build-exe.ps1"
Test-Path ".github/workflows/publish.yml"
```

### Step 3: Update `.github/workflows/publish.yml`

Replace the workflow file with this improved version that installs required tools:

---

## 📄 Updated `publish.yml` with Tool Installation

Create/update `.github/workflows/publish.yml` with this:

```yaml
name: Build and Publish

on:
  push:
	tags:
	  - "v*"

env:
  PROJECT_NAME: Power_Management_for_CommandPalette
  PUBLISHER: JRScott812
  PACKAGE_ID: PowerManagementforCommandPalette

jobs:
  build:
	runs-on: windows-latest
	strategy:
	  matrix:
		arch: [x64, arm64]

	steps:
	  - uses: actions/checkout@v4

	  - name: Setup .NET
		uses: actions/setup-dotnet@v4
		with:
		  dotnet-version: "10.0.x"

	  - name: Install Inno Setup
		run: |
		  choco install innosetup -y
		  refreshenv

	  - name: Install WingetCreate
		run: |
		  winget install Microsoft.WingetCreate

	  - name: Detect version
		id: version
		run: |
		  $tag = "${{ github.ref_name }}" -replace '^v', ''
		  echo "VERSION=$tag" >> $env:GITHUB_OUTPUT

	  - name: Publish
		run: |
		  dotnet publish -c Release -r win-${{ matrix.arch }} -o publish --self-contained=false

	  - name: Create installer
		run: |
		  $issFile = "setup-template.iss"
		  if (-not (Test-Path $issFile)) {
			  Write-Error "Inno Setup script not found: $issFile"
			  exit 1
		  }
		  & "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" `
			/DMyAppVersion="${{ steps.version.outputs.VERSION }}" `
			/DArchitecturesAllowed="${{ matrix.arch }}" `
			$issFile

	  - name: Upload artifact
		uses: actions/upload-artifact@v4
		with:
		  name: installer-${{ matrix.arch }}
		  path: Installer/*.exe
		  retention-days: 1

  release:
	needs: build
	runs-on: ubuntu-latest
	permissions:
	  contents: write

	steps:
	  - uses: actions/checkout@v4

	  - name: Download all artifacts
		uses: actions/download-artifact@v4
		with:
		  path: artifacts
		  merge-multiple: true

	  - name: Detect version
		id: version
		run: |
		  tag="${{ github.ref_name }}"
		  version="${tag#v}"
		  echo "VERSION=$version" >> $GITHUB_OUTPUT

	  - name: Create GitHub Release
		uses: softprops/action-gh-release@v2
		with:
		  files: artifacts/*.exe
		  tag_name: ${{ github.ref_name }}
		  generate_release_notes: true
		env:
		  GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}

  winget-update:
	needs: release
	runs-on: windows-latest
	permissions:
	  contents: read

	steps:
	  - name: Install WingetCreate
		run: |
		  winget install Microsoft.WingetCreate -e

	  - name: Detect version
		id: version
		run: |
		  $tag = "${{ github.ref_name }}" -replace '^v', ''
		  echo "VERSION=$tag" >> $env:GITHUB_OUTPUT

	  - name: Update WinGet manifest
		run: |
		  $baseUrl = "https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}"
		  $x64Url = "$baseUrl/${{ env.PROJECT_NAME }}_${{ steps.version.outputs.VERSION }}_x64.exe"
		  $arm64Url = "$baseUrl/${{ env.PROJECT_NAME }}_${{ steps.version.outputs.VERSION }}_arm64.exe"

		  Write-Host "Updating WinGet manifest..."
		  Write-Host "X64 URL: $x64Url"
		  Write-Host "ARM64 URL: $arm64Url"

		  wingetcreate update ${{ env.PUBLISHER }}.${{ env.PACKAGE_ID }} `
			--version "${{ steps.version.outputs.VERSION }}" `
			--urls $x64Url $arm64Url `
			--submit `
			--token "${{ secrets.GITHUB_TOKEN }}"
```

---

## 🔧 Create `build-exe.ps1` (if missing)

If you don't have `build-exe.ps1` in your repo root, create it:

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

---

## 🧪 Test Your Setup

### Test 1: Check if tools are installed locally
```powershell
# Check if Inno Setup is installed
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" --version

# Check if WingetCreate is available
wingetcreate --version

# Check .NET
dotnet --version
```

### Test 2: Manually run the build script
```powershell
.\build-exe.ps1 -Version "1.2.9.0"
```

### Test 3: Create a test tag to trigger workflow
```powershell
git tag v-test-1.0.0
git push origin v-test-1.0.0
```

Then monitor the workflow at: 
`https://github.com/JRScott812/CmdPal.Ext.Power/actions`

---

## ⚠️ Troubleshooting

| Problem | Solution |
|---------|----------|
| **Workflow doesn't start** | Make sure you pushed a tag: `git tag v1.2.9.0 && git push origin v1.2.9.0` |
| **"Inno Setup not found"** | Ensure `setup-template.iss` exists in repo root |
| **"build-exe.ps1 not found"** | Add the script to repo root (see template above) |
| **WinGet submission fails** | Check `$env:GITHUB_TOKEN` has proper permissions (should auto-work) |
| **Release creation fails** | Verify "Read and write permissions" is enabled in Settings → Actions |
| **Artifacts not found** | Ensure the Inno Setup script creates files in `Installer/` directory |

---

## ✅ Final Checklist

- [ ] Repository permissions set to "Read and write" (Settings → Actions → General)
- [ ] `setup-template.iss` exists in repo root
- [ ] `build-exe.ps1` exists in repo root (or use template above)
- [ ] `.github/workflows/publish.yml` updated with tool installation
- [ ] Files are committed and pushed to `main`
- [ ] Test by creating a tag: `git tag v-test && git push origin v-test`
- [ ] Monitor workflow execution at Actions tab
- [ ] Delete test tag when done: `git tag -d v-test && git push origin :v-test`

---

## 🚀 Ready to Test?

Once everything is set up, just run:

```powershell
git tag v1.2.9.0
git push origin v1.2.9.0
```

Then watch the magic happen! ✨

Check progress at: https://github.com/JRScott812/CmdPal.Ext.Power/actions
