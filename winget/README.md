# Publishing Power to community WinGet (`winget-pkgs`)

**Primary distribution for users:** [Microsoft Store](https://apps.microsoft.com/detail/9P5805PFVS3G) (product ID `9P5805PFVS3G`).

This folder is for an optional community WinGet listing. Command Palette discovers extensions through the Windows package catalog (`AppExtensionCatalog`). That means **MSIX with package identity** is required. Unpackaged Inno/EXE installers from older docs are **not** discoverable by CmdPal.

Working community packages (for example `lin-ycv.EverythingCmdPal3`) ship **signed MSIX** on GitHub Releases.

## Package identity

| Field | Value |
|-------|--------|
| PackageIdentifier | `JakeScott.Power` |
| Publisher | Jake Scott |
| PackageName | Power for Command Palette |
| License | MIT |
| Required tag | `windows-commandpalette-extension` |
| WASDK dependency | `Microsoft.WindowsAppRuntime.2` (matches `Microsoft.WindowsAppSDK` 2.2.0) |

## One-time prerequisites

1. Install [WingetCreate](https://github.com/microsoft/winget-create):

   ```powershell
   winget install Microsoft.WingetCreate
   ```

2. A **trusted code-signing certificate** for MSIX (self-signed will fail winget-pkgs install validation).

3. GitHub CLI (`gh`) if you create releases from the command line.

## Release flow

### 1. Build MSIX (x64 + ARM64)

From the repo root:

```powershell
.\scripts\Build-WingetMsix.ps1 -Version 1.2.5.0
```

Outputs land in `artifacts\winget\`.

### 2. Sign both packages

Sign with your code-signing cert (SignTool / Azure Trusted Signing / etc.).

### 3. GitHub Release

```powershell
git tag -a v1.2.5.0 -m "Release v1.2.5.0"
git push origin v1.2.5.0

gh release create v1.2.5.0 `
  "artifacts\winget\JakeScott.Power_1.2.5.0_x64.msix" `
  "artifacts\winget\JakeScott.Power_1.2.5.0_arm64.msix" `
  --title "v1.2.5.0" `
  --notes "Power for Command Palette v1.2.5.0"
```

Or push a `v*` tag and let `.github/workflows/release-winget.yml` build and attach MSIX (you still must sign before WinGet submission if CI produces unsigned packages).

### 4. Submit to winget-pkgs

First publish:

```powershell
.\scripts\Submit-Winget.ps1 `
  -Version 1.2.5.0 `
  -X64Url "https://github.com/JRScott812/CmdPal.Ext.Power/releases/download/v1.2.5.0/JakeScott.Power_1.2.5.0_x64.msix" `
  -Arm64Url "https://github.com/JRScott812/CmdPal.Ext.Power/releases/download/v1.2.5.0/JakeScott.Power_1.2.5.0_arm64.msix"
```

Later versions:

```powershell
.\scripts\Submit-Winget.ps1 -Update -Submit -Version ... -X64Url ... -Arm64Url ...
```

Before the PR is reviewed, confirm:

- Locale YAML includes `Tags: [windows-commandpalette-extension]`
- Installer YAML depends on `Microsoft.WindowsAppRuntime.2`

Template files in this folder (`JakeScott.Power*.yaml`) are reference copies; `wingetcreate` generates the real PR contents.

## CmdPal Extensions Gallery

After the package is live on WinGet:

```json
"installSources": [
  { "type": "winget", "id": "JakeScott.Power" }
]
```

If you also publish to the Microsoft Store, prefer adding `msstore` with your Store product ID as well (or instead).

## Why not Inno Setup?

Microsoft’s WinGet publishing guide still shows unpackaged EXE + registry `LocalServer32` entries. CmdPal only enumerates **packaged** app extensions, so those installers do not appear in Command Palette. Prefer MSIX (this guide) or Store.

