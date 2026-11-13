; Script do Inno Setup 6 para gerar o instalador unico (PUP-Setup.exe)
; Antes de compilar no ISCC, execute: powershell -ExecutionPolicy Bypass -File ferramentas\empacotar.ps1

#define MyAppName "Portal de Utilidades Publicas (PUP)"
#define MyAppShortName "PUP"
#define MyAppVersion "1.0.0"
#define MyAppExeName "iniciar-pup.bat"
#define SourcePackageDir "..\dist\pup-win-x64"

[Setup]
AppId={{8D6B4F3A-4E1C-4A9F-9B6F-7E92D6D1C201}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={localappdata}\Programs\{#MyAppShortName}
DefaultGroupName={#MyAppShortName}
OutputDir=..\dist
OutputBaseFilename=PUP-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest

[Languages]
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourcePackageDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "appsettings.Production.json"
; Preserva configuracoes editadas em atualizacoes futuras.
Source: "{#SourcePackageDir}\appsettings.Production.json"; DestDir: "{app}"; Flags: onlyifdoesntexist

[Icons]
Name: "{group}\Iniciar {#MyAppShortName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{group}\Configuracao (appsettings.Production.json)"; Filename: "notepad.exe"; Parameters: """{app}\appsettings.Production.json"""
Name: "{group}\Instrucoes de Uso"; Filename: "{app}\LEIA-ME.txt"
Name: "{autodesktop}\{#MyAppShortName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\LEIA-ME.txt"; Description: "Abrir instrucoes de configuracao do banco e uso"; Flags: postinstall shellexec skipifsilent unchecked
