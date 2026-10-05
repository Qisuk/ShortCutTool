; Inno Setup script for ShortCut Tool (per-user install, no admin rights needed).
; Build it with installer\Build-Installer.ps1, which publishes the app and passes the defines below.
; Works with Inno Setup 6.3+ and 7.x.

#ifndef AppVersion
  #error AppVersion must be defined, e.g. ISCC /DAppVersion=1.1.0 ShortCutTool.iss
#endif
#ifndef AppFileVersion
  #define AppFileVersion AppVersion
#endif
#ifndef PublishDir
  #define PublishDir "..\artifacts\publish"
#endif
#ifndef OutputDir
  #define OutputDir "..\artifacts"
#endif

#define AppName "ShortCut Tool"
#define AppExeName "ShortCutTool.exe"
#define AppPublisher "Qisuk"
#define AppUrl "https://github.com/Qisuk/ShortCutTool"
#define RuntimeUrl "https://dotnet.microsoft.com/download/dotnet/10.0"

; Never change AppGuid: it identifies the installation for upgrades and uninstall.
#define AppGuid "681BDEBB-6E4C-451C-A509-91C4D9F71A02"

; These must match the app: Program.SingleInstanceMutexName and StartupRegistration.cs.
#define MutexName "ShortCutTool.SingleInstance"
#define RunKey "Software\Microsoft\Windows\CurrentVersion\Run"
#define RunValueName "ShortCutTool"

[Setup]
AppId={{{#AppGuid}}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppUrl}
AppSupportURL={#AppUrl}/issues
AppUpdatesURL={#AppUrl}/releases
VersionInfoVersion={#AppFileVersion}
; With PrivilegesRequired=lowest, {autopf} is %LOCALAPPDATA%\Programs
DefaultDirName={autopf}\ShortCutTool
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
OutputDir={#OutputDir}
OutputBaseFilename=ShortCutTool-{#AppVersion}-win-x64-setup
SetupIconFile=..\Assets\Icons\app.ico
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}
LicenseFile=..\LICENSE
WizardStyle=modern
Compression=lzma2/max
SolidCompression=yes
; The tray app is normally running during an upgrade. Restart Manager closes it (the app
; exits cleanly on the session-end message) and [Run] starts it again afterwards.
CloseApplications=force
RestartApplications=no

[Tasks]
; Only offered on a fresh install, so an upgrade never re-enables startup after the user
; turned it off from the tray menu.
Name: "startup"; Description: "Start {#AppName} when I sign in to Windows"; Check: not IsUpgrade

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"

[Registry]
Root: HKCU; Subkey: "{#RunKey}"; ValueType: string; ValueName: "{#RunValueName}"; ValueData: """{app}\{#AppExeName}"""; Tasks: startup

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; Flags: nowait postinstall; Check: ShouldLaunch

[UninstallRun]
Filename: "{sys}\taskkill.exe"; Parameters: "/f /im {#AppExeName} /fi ""USERNAME eq {username}"""; Flags: runhidden; RunOnceId: "StopApp"

[Code]
var
  Upgrading: Boolean;
  WasRunning: Boolean;

function IsUpgrade: Boolean;
begin
  Result := Upgrading;
end;

{ Launch after a fresh install, or after an upgrade if the app was running beforehand. }
function ShouldLaunch: Boolean;
begin
  Result := (not Upgrading) or WasRunning;
end;

function IsDesktopRuntimeInstalled: Boolean;
var
  FindRec: TFindRec;
begin
  Result := FindFirst(ExpandConstant('{commonpf64}\dotnet\shared\Microsoft.WindowsDesktop.App\10.*'), FindRec);
  if Result then
    FindClose(FindRec);
end;

function InitializeSetup: Boolean;
var
  ErrorCode: Integer;
begin
  Result := True;
  Upgrading := RegKeyExists(HKCU, 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{' + '{#AppGuid}' + '}_is1');
  WasRunning := CheckForMutexes('{#MutexName}');

  { WinGet installs the runtime as a declared dependency; this covers manual installs. }
  if not IsDesktopRuntimeInstalled then
  begin
    Log('.NET 10 Desktop Runtime (x64) not found');
    if not WizardSilent then
      if MsgBox('{#AppName} needs the .NET 10 Desktop Runtime (x64), which was not found.' + #13#10#13#10 +
                'Setup will continue, but the app will not start until the runtime is installed.' + #13#10#13#10 +
                'Open the download page now?', mbConfirmation, MB_YESNO) = IDYES then
        ShellExec('open', '{#RuntimeUrl}', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  { The entry may have been added by the installer task or by the tray menu, so always remove it. }
  if CurUninstallStep = usPostUninstall then
    RegDeleteValue(HKCU, '{#RunKey}', '{#RunValueName}');
end;
