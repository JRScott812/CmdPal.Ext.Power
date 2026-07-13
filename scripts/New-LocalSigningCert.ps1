# Creates a local self-signed cert for MSIX packaging (optional).
# Loose registration via Register-Local.ps1 does not require this.
#
# Usage (from repo root):
#   .\scripts\New-LocalSigningCert.ps1
#
# Then build with signing:
#   dotnet build CmdPal.Ext.Power\CmdPal.Ext.Power.csproj -c Debug -p:Platform=x64 `
#     -p:GenerateAppxPackageOnBuild=true -p:AppxPackageSigningEnabled=true `
#     -p:PackageCertificateThumbprint=<thumbprint printed below>

$ErrorActionPreference = "Stop"
$projectDir = Join-Path (Split-Path $PSScriptRoot -Parent) "CmdPal.Ext.Power"
$pfx = Join-Path $projectDir "CmdPal.Ext.Power_TemporaryKey.pfx"
$cer = Join-Path $projectDir "CmdPal.Ext.Power_TemporaryKey.cer"
$subject = "CN=JRScott812"

Get-ChildItem Cert:\CurrentUser\My |
    Where-Object { $_.Subject -eq $subject -and $_.FriendlyName -eq "CmdPal.Ext.Power Dev" } |
    ForEach-Object { Remove-Item "Cert:\CurrentUser\My\$($_.Thumbprint)" -Force -ErrorAction SilentlyContinue }

$password = ConvertTo-SecureString -String "CmdPalPowerLocal!" -Force -AsPlainText
$cert = New-SelfSignedCertificate `
    -Type Custom `
    -Subject $subject `
    -KeyUsage DigitalSignature `
    -FriendlyName "CmdPal.Ext.Power Dev" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")

Export-PfxCertificate -Cert $cert -FilePath $pfx -Password $password | Out-Null
Export-Certificate -Cert $cert -FilePath $cer | Out-Null

Write-Host "Created: $pfx"
Write-Host "Created: $cer"
Write-Host "Thumbprint=$($cert.Thumbprint)"
Write-Host ""
Write-Host "Import the .cer into TrustedPeople (and Root if needed) before Add-AppxPackage of a signed MSIX."
Write-Host "Prefer .\scripts\Register-Local.ps1 for day-to-day development (no cert trust required)."
