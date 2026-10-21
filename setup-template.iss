#define MyAppName "Power Management for Command Palette"
#define MyAppVersion "1.2.9.0"
#define MyAppPublisher "JRScott812"
#define MyAppURL "https://github.com/JRScott812/CmdPal.Ext.Power"
#define MyAppCLSID "{6D9ABD73-60FC-4BB5-9342-8CD5BFFC17CE}"

[Setup]
AppId={#MyAppCLSID}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
OutputBaseFilename=Power_Management_for_CommandPalette_{#MyAppVersion}_{#SetupSetting("ArchitecturesAllowed")}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
OutputDir=Installer

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Registry]
Root: HKCU; Subkey: "Software\Classes\CLSID\{#MyAppCLSID}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\CLSID\{#MyAppCLSID}\InprocServer32"; ValueType: string; ValueName: ""; ValueData: "{app}\CmdPal.Ext.Power.dll"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\CLSID\{#MyAppCLSID}\InprocServer32"; ValueType: string; ValueName: "ThreadingModel"; ValueData: "Both"; Flags: uninsdeletekey

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
