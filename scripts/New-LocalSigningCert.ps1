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
$pwdFile = Join-Path $projectDir "CmdPal.Ext.Power_TemporaryKey.password.txt"
$subject = "CN=JRScott812"

Get-ChildItem Cert:\CurrentUser\My |
    Where-Object { $_.Subject -eq $subject -and $_.FriendlyName -eq "CmdPal.Ext.Power Dev" } |
    ForEach-Object { Remove-Item "Cert:\CurrentUser\My\$($_.Thumbprint)" -Force -ErrorAction SilentlyContinue }

# Random local-only password (file is gitignored via *.pfx sibling + *.txt under project if needed)
$plain = -join ((48..57 + 65..90 + 97..122) | Get-Random -Count 24 | ForEach-Object { [char]$_ })
$password = ConvertTo-SecureString -String $plain -Force -AsPlainText
$cert = New-SelfSignedCertificate `
    -Type Custom `
    -Subject $subject `
    -KeyUsage DigitalSignature `
    -FriendlyName "CmdPal.Ext.Power Dev" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")

Export-PfxCertificate -Cert $cert -FilePath $pfx -Password $password | Out-Null
Export-Certificate -Cert $cert -FilePath $cer | Out-Null
# utf8NoBOM is PowerShell 7+; Windows PowerShell 5.1 "utf8" writes a BOM.
if ($PSVersionTable.PSVersion.Major -ge 7) {
    Set-Content -LiteralPath $pwdFile -Value $plain -Encoding utf8NoBOM
} else {
    [System.IO.File]::WriteAllText($pwdFile, $plain)
}

Write-Host "Created: $pfx"
Write-Host "Created: $cer"
Write-Host "Created: $pwdFile (local only - do not commit)"
Write-Host "Thumbprint=$($cert.Thumbprint)"
Write-Host ""
Write-Host "Import the .cer into TrustedPeople (and Root if needed) before Add-AppxPackage of a signed MSIX."
Write-Host "Prefer .\scripts\Register-Local.ps1 for day-to-day development (no cert trust required)."
