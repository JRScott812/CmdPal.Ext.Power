# Power (Command Palette extension)

Switch Windows **power modes** and **power plans** from [Command Palette](https://learn.microsoft.com/windows/powertoys/command-palette/overview), with dock bands and fallback search.

Originally proposed as a PowerToys built-in ([PR #49027](https://github.com/microsoft/PowerToys/pull/49027)); Microsoft recommended shipping it as a community / Store extension instead.

## Install

- **[Microsoft Store](https://apps.microsoft.com/detail/9P5805PFVS3G)**
- **WinGet** (Microsoft Store source):

```powershell
winget install --id 9P5805PFVS3G --source msstore
```

- **WinGet** (community / winget-pkgs — enables Command Palette “Search WinGet” discovery): coming soon as `JakeScott.PowerControlExtensionforCommandPalette`

Requires [PowerToys](https://apps.microsoft.com/detail/XP89DCGQ3K6VLD) with Command Palette enabled (or the standalone Command Palette app).

After install, open Command Palette and search for **Power**, or use the Power dock bands. This extension has no standalone window — launching it from the Start menu only shows a short tip.

Store installs are signed by Microsoft. Local Developer Mode registration (`Register-Local.ps1`) is unsigned by design.

## Release EXE installers (WinGet)

Build unpackaged installers locally (requires [Inno Setup](https://jrsoftware.org/isdl.php) 6 or 7):

```powershell
cd CmdPal.Ext.Power
.\build-exe.ps1 -Version "1.2.7.0"
Get-ChildItem bin\Release\installer\
```

Or run the **CmdPal Extension - Build EXE Installer** GitHub Action (`workflow_dispatch`) to publish a GitHub Release with x64 and ARM64 setup EXEs.

### First community WinGet submission

After the release assets exist:

```powershell
winget install Microsoft.WingetCreate
wingetcreate new "<x64-setup-exe-url>" "<arm64-setup-exe-url>"
```

Use package id `JakeScott.PowerControlExtensionforCommandPalette`. Before submitting the PR to [winget-pkgs](https://github.com/microsoft/winget-pkgs):

1. Add to each `.locale.*.yaml`:

```yaml
Tags:
- windows-commandpalette-extension
```

2. Add to the `.installer.yaml` dependencies (matches `Microsoft.WindowsAppSDK` 2.2.0):

```yaml
Dependencies:
  PackageDependencies:
  - PackageIdentifier: Microsoft.WindowsAppRuntime.2
```

Subsequent versions: `wingetcreate update JakeScott.PowerControlExtensionforCommandPalette --version <ver> --urls "<x64>" "<arm64>" --submit`

## Features

- Switch **power modes** (Best efficiency, Balanced, Best performance) when supported
- Switch **power plans** (Power saver, Balanced, High performance, Ultimate performance, and custom plans)
- Dock bands for quick mode / plan changes
- Fallback search for “power”, mode, and plan terms

## Build from source

```powershell
dotnet build CmdPal.Ext.Power.slnx -c Debug -p:Platform=x64
```

Debug x64 builds package the MSIX and run `scripts/Register-Local.ps1` automatically. Requires Windows Developer Mode. Opt out with `-p:RegisterLocalOnBuild=false`.

## License

[MIT](LICENSE)
