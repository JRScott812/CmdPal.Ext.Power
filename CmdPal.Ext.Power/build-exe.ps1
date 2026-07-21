# Builds self-contained EXE installers (x64 / arm64) for WinGet distribution.
#
# Requires: .NET SDK matching the project TFM, Inno Setup 6

param(
	[string]$ExtensionName = "CmdPal.Ext.Power",
	[string]$Configuration = "Release",
	[string]$Version = "1.2.8.0",
	[string[]]$Platforms = @("x64", "arm64")
)

$ErrorActionPreference = "Stop"

Write-Host "Building $ExtensionName EXE installer..." -ForegroundColor Green
Write-Host "Version: $Version" -ForegroundColor Yellow
Write-Host "Platforms: $($Platforms -join ', ')" -ForegroundColor Yellow

$ProjectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectFile = Join-Path $ProjectDir "$ExtensionName.csproj"

if (-not (Test-Path $ProjectFile)) {
	throw "Project file not found: $ProjectFile"
}

$InnoCandidates = @(
	"${env:ProgramFiles(x86)}\Inno Setup 6\iscc.exe",
	"${env:ProgramFiles}\Inno Setup 6\iscc.exe",
	"${env:ProgramFiles}\Inno Setup 7\iscc.exe",
	"${env:ProgramFiles(x86)}\Inno Setup 7\iscc.exe"
)
$InnoSetupPath = $InnoCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $InnoSetupPath) {
	throw "Inno Setup (6 or 7) not found. Install from https://jrsoftware.org/isdl.php or: winget install JRSoftware.InnoSetup"
}

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "$ProjectDir\bin") {
	Remove-Item -Path "$ProjectDir\bin" -Recurse -Force -ErrorAction SilentlyContinue
}
if (Test-Path "$ProjectDir\obj") {
	Remove-Item -Path "$ProjectDir\obj" -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore $ProjectFile
if ($LASTEXITCODE -ne 0) {
	throw "dotnet restore failed with exit code: $LASTEXITCODE"
}

foreach ($Platform in $Platforms) {
	Write-Host "`n=== Building $Platform ===" -ForegroundColor Cyan

	Write-Host "Building and publishing $Platform application..." -ForegroundColor Yellow
	# Disable MSIX packaging for unpackaged WinGet EXE distribution
	dotnet publish $ProjectFile `
		--configuration $Configuration `
		--runtime "win-$Platform" `
		--self-contained true `
		--output "$ProjectDir\bin\$Configuration\win-$Platform\publish" `
		-p:WindowsPackageType=None `
		-p:GenerateAppxPackageOnBuild=false `
		-p:EnableMsixTooling=false

	if ($LASTEXITCODE -ne 0) {
		throw "Build failed for $Platform with exit code: $LASTEXITCODE"
	}

	$publishDir = "$ProjectDir\bin\$Configuration\win-$Platform\publish"
	$fileCount = (Get-ChildItem -Path $publishDir -Recurse -File).Count
	Write-Host "Published $fileCount files to $publishDir" -ForegroundColor Green

	Write-Host "Creating installer script for $Platform..." -ForegroundColor Yellow
	$setupTemplate = Get-Content "$ProjectDir\setup-template.iss" -Raw

	$setupScript = $setupTemplate -replace '#define AppVersion ".*"', "#define AppVersion `"$Version`""
	$setupScript = $setupScript -replace 'OutputBaseFilename=(.*?)\{#AppVersion\}', "OutputBaseFilename=`$1{#AppVersion}-$Platform"
	$setupScript = $setupScript -replace 'Source: "bin\\Release\\win-x64\\publish', "Source: `"bin\Release\win-$Platform\publish"

	if ($Platform -eq "arm64") {
		$setupScript = $setupScript -replace '(\[Setup\][^\[]*)(MinVersion=)', "`$1ArchitecturesAllowed=arm64`r`nArchitecturesInstallIn64BitMode=arm64`r`n`$2"
	}
	else {
		$setupScript = $setupScript -replace '(\[Setup\][^\[]*)(MinVersion=)', "`$1ArchitecturesAllowed=x64compatible`r`nArchitecturesInstallIn64BitMode=x64compatible`r`n`$2"
	}

	$setupScript | Out-File -FilePath "$ProjectDir\setup-$Platform.iss" -Encoding utf8

	Write-Host "Creating $Platform installer with Inno Setup..." -ForegroundColor Yellow
	& $InnoSetupPath "$ProjectDir\setup-$Platform.iss"
	if ($LASTEXITCODE -ne 0) {
		throw "Inno Setup failed for $Platform with exit code: $LASTEXITCODE"
	}

	$installer = Get-ChildItem "$ProjectDir\bin\$Configuration\installer\*-$Platform.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
	if (-not $installer) {
		throw "Installer file not found for $Platform under bin\$Configuration\installer"
	}

	$sizeMB = [math]::Round($installer.Length / 1MB, 2)
	Write-Host "Created $Platform installer: $($installer.Name) ($sizeMB MB)" -ForegroundColor Green
}

Write-Host "`nBuild completed successfully!" -ForegroundColor Green
Get-ChildItem "$ProjectDir\bin\$Configuration\installer\*.exe" | ForEach-Object {
	Write-Host "  $($_.FullName)" -ForegroundColor Cyan
}
