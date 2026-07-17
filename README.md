# Power (Command Palette extension)

Switch Windows **power modes** and **power plans** from [Command Palette](https://learn.microsoft.com/windows/powertoys/command-palette/overview), with dock bands and fallback search.

Originally proposed as a PowerToys built-in ([PR #49027](https://github.com/microsoft/PowerToys/pull/49027)); Microsoft recommended shipping it as a community / Store extension instead.

## Install

- **[Microsoft Store](https://apps.microsoft.com/detail/9P5805PFVS3G)**
- **WinGet** (Microsoft Store source):

```powershell
winget install --id 9P5805PFVS3G --source msstore
```

Requires [PowerToys](https://apps.microsoft.com/detail/XP89DCGQ3K6VLD) with Command Palette enabled (or the standalone Command Palette app).

After install, open Command Palette and search for **Power**, or use the Power dock bands. This extension has no standalone window — launching it from the Start menu only shows a short tip.

Store installs are signed by Microsoft. Local Developer Mode registration (`Register-Local.ps1`) is unsigned by design.

## Features

- Switch **power modes** (Best efficiency, Balanced, Best performance) when supported
- Switch **power plans** (Power saver, Balanced, High performance, Ultimate performance, and custom plans)
- Dock bands for quick mode / plan changes
- Fallback search for “power”, mode, and plan terms

## Build from source

```powershell
dotnet build CmdPal.Ext.Power.slnx -c Debug -p:Platform=x64
.\scripts\Register-Local.ps1
```

Requires Windows Developer Mode for local MSIX registration.

## License

[MIT](LICENSE)
