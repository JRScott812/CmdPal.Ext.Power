; Inno Setup script for CmdPal.Ext.Power (Command Palette extension)
;
; build-exe.ps1 copies this template per architecture and injects
; version, platform source path, and ArchitecturesAllowed.

#define AppVersion "1.2.8.0"

[Setup]
AppId={{6D9ABD73-60FC-4BB5-9342-8CD5BFFC17CE}}
AppName=Power Control for Command Palette
AppVersion={#AppVersion}
AppPublisher=Jake Scott
DefaultDirName={autopf}\CmdPal.Ext.Power
OutputDir=bin\Release\installer
OutputBaseFilename=CmdPal.Ext.Power-Setup-{#AppVersion}
Compression=lzma
SolidCompression=yes
MinVersion=10.0.19041
PrivilegesRequired=lowest

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "bin\Release\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Power Control for Command Palette"; Filename: "{app}\CmdPal.Ext.Power.exe"

[Registry]
; Double braces so Inno emits a literal {CLSID} (single { starts a constant).
Root: HKCU; Subkey: "SOFTWARE\Classes\CLSID\{{6D9ABD73-60FC-4BB5-9342-8CD5BFFC17CE}}"; ValueType: string; ValueData: "CmdPal.Ext.Power"; Flags: uninsdeletekey
Root: HKCU; Subkey: "SOFTWARE\Classes\CLSID\{{6D9ABD73-60FC-4BB5-9342-8CD5BFFC17CE}}\LocalServer32"; ValueType: string; ValueData: """{app}\CmdPal.Ext.Power.exe"" -RegisterProcessAsComServer"; Flags: uninsdeletekey
