param(
	[string]$Configuration = "Release",
	[string]$Version = "1.2.9.0"
)

Write-Host "Building Power Management for Command Palette v$Version" -ForegroundColor Cyan

# Publish for x64
Write-Host "Publishing for win-x64..." -ForegroundColor Yellow
dotnet publish CmdPal.Ext.Power/CmdPal.Ext.Power.csproj `
	-c $Configuration `
	-r win-x64 `
	--no-self-contained `
	-p:WindowsPackageType=None `
	-o publish/x64

if ($LASTEXITCODE -ne 0) { Write-Error "x64 publish failed"; exit 1 }

# Publish for arm64
Write-Host "Publishing for win-arm64..." -ForegroundColor Yellow
dotnet publish CmdPal.Ext.Power/CmdPal.Ext.Power.csproj `
	-c $Configuration `
	-r win-arm64 `
	--no-self-contained `
	-p:WindowsPackageType=None `
	-o publish/arm64

if ($LASTEXITCODE -ne 0) { Write-Error "arm64 publish failed"; exit 1 }

# Build installer for x64
Write-Host "Building x64 installer with Inno Setup..." -ForegroundColor Yellow
$ISCCPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (-not (Test-Path $ISCCPath)) {
	Write-Error "Inno Setup not found at $ISCCPath"
	exit 1
}

& $ISCCPath /DMyAppVersion=$Version /DArchitecturesAllowed=x64 setup-template.iss
if ($LASTEXITCODE -ne 0) { Write-Error "x64 installer build failed"; exit 1 }

# Build installer for arm64
Write-Host "Building arm64 installer with Inno Setup..." -ForegroundColor Yellow
& $ISCCPath /DMyAppVersion=$Version /DArchitecturesAllowed=arm64 setup-template.iss
if ($LASTEXITCODE -ne 0) { Write-Error "arm64 installer build failed"; exit 1 }

# Cleanup publish directories
Remove-Item -Recurse -Force publish/x64, publish/arm64 -ErrorAction SilentlyContinue

Write-Host "✓ Build complete. Installers in Installer/" -ForegroundColor Green
