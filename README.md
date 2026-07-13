# Power (Command Palette extension)

A community [Command Palette](https://learn.microsoft.com/windows/powertoys/command-palette/overview) extension for switching Windows **power modes** and **power plans**, with dock bands and fallback search.

Originally proposed as a PowerToys built-in ([PR #49027](https://github.com/microsoft/PowerToys/pull/49027)); Microsoft recommended shipping it as a community / Store extension instead.

## Features

- Switch **power modes** (Best efficiency, Balanced, Best performance) when supported
- Switch **power plans** (Power saver, Balanced, High performance, Ultimate performance, and custom plans)
- Dock bands for quick mode / plan changes
- Fallback search for “power”, mode, and plan terms

## Requirements

- Windows 10 version 19041+ or Windows 11
- [PowerToys Command Palette](https://learn.microsoft.com/windows/powertoys/command-palette/overview)
- For local development: **Developer Mode**, .NET 10 SDK, and Visual Studio (or Build Tools) with Windows App SDK / MSIX workloads

## Build

```powershell
dotnet build CmdPal.Ext.Power.sln -c Debug -p:Platform=x64
```

Create an MSIX (needed for local install):

```powershell
dotnet build CmdPal.Ext.Power\CmdPal.Ext.Power.csproj -c Debug -p:Platform=x64 -p:GenerateAppxPackageOnBuild=true
```

## Tests

```powershell
dotnet test CmdPal.Ext.Power.UnitTests\CmdPal.Ext.Power.UnitTests.csproj -c Debug -p:Platform=x64
```

## Install / run locally

With Developer Mode enabled:

```powershell
.\scripts\Register-Local.ps1
```

Then restart Command Palette and search for **Power**, or pin the Power Mode / Power Plan dock bands.

Optional signed packaging (not required for `Register-Local.ps1`):

```powershell
.\scripts\New-LocalSigningCert.ps1
```

## Project layout

| Path | Purpose |
|------|---------|
| `CmdPal.Ext.Power/` | MSIX extension host + Power feature code |
| `CmdPal.Ext.Power.UnitTests/` | MSTest unit tests |
| `scripts/` | Local register / signing helpers |
| `.github/skills/` | CmdPal extension authoring skills (from the official template) |

## Identity

| Item | Value |
|------|--------|
| Package | `JRScott812.CmdPal.Ext.Power` |
| Provider Id | `com.jrscott812.cmdpal.power` |
| Publisher (dev) | `CN=JRScott812` |
| COM ClassId | `6D9ABD73-60FC-4BB5-9342-8CD5BFFC17CE` |

For Microsoft Store publishing, replace the Publisher identity with your Partner Center value and use your Store signing certificate. See `.github/skills/publish-extension/`.

## License

MIT — see [LICENSE](LICENSE).
